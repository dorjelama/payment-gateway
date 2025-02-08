import { Component, OnInit } from "@angular/core";
import { PaymentService } from "../services/payment.service";

@Component({
  selector: "app-transaction-history",
  templateUrl: "./transaction-history.component.html",
  styleUrls: ["./transaction-history.component.css"],
})
export class TransactionHistoryComponent implements OnInit {
  transactions: any[] = [];
  loading = true;
  error = "";

  // Filters
  startDate: Date | null = null; // Use Date type for datepicker
  endDate: Date | null = null; // Use Date type for datepicker
  status: string = ""; // Default status

  // Table Columns
  displayedColumns: string[] = [
    "transactionId",
    "amount",
    "currency",
    "status",
    "createdAt",
  ];

  constructor(private paymentService: PaymentService) {}

  ngOnInit(): void {
    this.fetchTransactions();
  }

  fetchTransactions(): void {
    this.loading = true;

    // Convert dates to ISO format before sending
    const formattedStartDate = this.startDate ? this.formatDate(this.startDate) : null;
    const formattedEndDate = this.endDate ? this.formatDate(this.endDate) : null;


    this.paymentService
    .getTransactions(formattedStartDate, formattedEndDate, this.status)
    .subscribe({
      next: (transactions) => {
        this.transactions = transactions;
        this.loading = false;
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to load transaction history.';
        this.loading = false;
        console.error('Error fetching transactions:', err);
      },
    });
  }

  onFilterSubmit(): void {
    this.fetchTransactions();
  }

  private formatDate(date: Date): string {
    // Convert Date object to ISO 8601 format (e.g., "2025-02-01T00:00:00Z")
    return date.toISOString();
  }
}
