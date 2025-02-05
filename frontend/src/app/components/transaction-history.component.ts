import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PaymentService } from '../services/payment.service';
import { Transaction } from '../models/payment.model';

@Component({
  selector: 'app-transaction-history',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="transaction-history">
      <h2>Transaction History</h2>
      <div class="transactions">
        <div *ngFor="let transaction of transactions" class="transaction-item">
          <div class="transaction-details">
            <span class="amount">{{ transaction.amount }} {{ transaction.currency }}</span>
            <span class="card">**** {{ transaction.last4 }}</span>
          </div>
          <div class="transaction-meta">
            <span class="timestamp">{{ transaction.timestamp | date:'short' }}</span>
            <span class="status" [class]="transaction.status">
              {{ transaction.status }}
            </span>
          </div>
        </div>
        <div *ngIf="transactions.length === 0" class="no-transactions">
          No transactions yet
        </div>
      </div>
    </div>
  `
})
export class TransactionHistoryComponent {
  transactions: Transaction[] = [];

  constructor(private paymentService: PaymentService) {
    this.paymentService.getTransactions().subscribe(
      transactions => this.transactions = transactions
    );
  }
}