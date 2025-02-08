import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private baseUrl = environment.apiUrl; // Use the base URL from environment

  constructor(private http: HttpClient) {}

  login(credentials: { username: string; password: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/auth/login`, credentials).pipe(
      catchError((error) => {
        console.error('Login failed:', error);
        return throwError(() => new Error('Invalid username or password'));
      })
    );
  }

  saveToken(token: string): void {
    localStorage.setItem('authToken', token); // Save token to localStorage
  }

  getToken(): string | null {
    return localStorage.getItem('authToken'); // Retrieve token from localStorage
  }

  logout(): void {
    localStorage.removeItem('authToken'); // Remove token on logout
  }

  isAuthenticated(): boolean {
    return !!this.getToken(); // Check if token exists
  }
}