import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "../../environments/environment";
import { AuthService } from "./auth.service";

@Injectable({
  providedIn: "root",
})
export class PaymentService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient, private authService: AuthService) {}

  processPayment(paymentData: any): Observable<any> {
    const token = this.authService.getToken();
    const headers = new HttpHeaders()
      .set("Authorization", `Bearer ${token}`)
      .set("Content-Type", "application/json");

    return this.http.post(`${this.baseUrl}/api/Payments/Process`, paymentData, {
      headers,
    });
  }

  getTransactions(
    startDate: string | null, // Accepts formatted date strings
    endDate: string | null, // Accepts formatted date strings
    status: string
  ): Observable<any[]> {
    const token = this.authService.getToken();
    const headers = new HttpHeaders().set("Authorization", `Bearer ${token}`);

    // Construct query parameters dynamically
    const params: Record<string, string> = {};
    if (startDate) params["StartDate"] = startDate;
    if (endDate) params["EndDate"] = endDate;
    if (status) params["Status"] = status;

    return this.http.get<any[]>(`${this.baseUrl}/api/Transactions`, {
      headers,
      params,
    });
  }
}
