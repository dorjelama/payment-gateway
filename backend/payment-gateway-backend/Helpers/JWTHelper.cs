using Microsoft.IdentityModel.Tokens;
using payment_gateway_backend.Entities;
using payment_gateway_backend.Repositories.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace payment_gateway_backend.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<JwtHelper> _logger;
        private readonly IUserRepository _userRepository;
        public JwtHelper(IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<JwtHelper> logger,
            IUserRepository userRepository)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _userRepository = userRepository;
        }

        public string GenerateToken(string username)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            var expirationInMinutes = int.Parse(jwtSettings["ExpirationInMinutes"]);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(expirationInMinutes),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Guid GetUserIdFromToken()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var username = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = _userRepository.GetUserByUsernameAsync(username).GetAwaiter().GetResult().UserId;
            return userId;
        }

    }
}
