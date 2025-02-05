import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { BehaviorSubject, Observable, interval } from "rxjs";
import { switchMap } from "rxjs/operators";
import { Payment, Transaction } from "../models/payment.model";

@Injectable({
  providedIn: "root",
})
export class PaymentService {
  private transactions = new BehaviorSubject<Transaction[]>([]);

  // Simulated transactions for demo
  private mockTransactions: Transaction[] = [];

  constructor(private http: HttpClient) {
    // Start polling for transaction updates
    this.startPolling();
  }

  processPayment(payment: Payment): Observable<any> {
    // Simulate API call
    const mockTransaction: Transaction = {
      id: Math.random().toString(36).substr(2, 9),
      amount: payment.amount,
      currency: payment.currency,
      status: "pending",
      timestamp: new Date(),
      last4: payment.cardNumber.slice(-4),
    };

    this.mockTransactions.push(mockTransaction);
    this.transactions.next(this.mockTransactions);

    return new Observable((subscriber) => {
      setTimeout(() => {
        mockTransaction.status = Math.random() > 0.5 ? "completed" : "failed";
        this.transactions.next(this.mockTransactions);
        subscriber.next({ success: true });
        subscriber.complete();
      }, 2000);
    });
  }

  getTransactions(): Observable<Transaction[]> {
    return this.transactions.asObservable();
  }

  private startPolling() {
    interval(5000).subscribe(() => {
      // In a real app, this would fetch updates from the server
      this.transactions.next(this.mockTransactions);
    });
  }
}
