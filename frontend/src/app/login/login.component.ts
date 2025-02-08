import { Component } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { AuthService } from "../services/auth.service";
import { Router } from "@angular/router";

@Component({
  selector: "app-login",
  templateUrl: "./login.component.html",
  styleUrls: ["./login.component.css"],
})
export class LoginComponent {
  loginForm: FormGroup;
  processing = false;
  errorMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      username: ["", Validators.required],
      password: ["", Validators.required],
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.processing = true;
      const credentials = this.loginForm.value;

      this.authService.login(credentials).subscribe({
        next: (response) => {
          this.processing = false;
          console.log("Login successful:", response);

          // Save the token
          this.authService.saveToken(response.token);

          // Redirect to the payment page
          this.router.navigate(["/payment"]);
        },
        error: (err) => {
          this.processing = false;
          this.errorMessage = err.message || "Login failed. Please try again.";
        },
      });
    }
  }
}
