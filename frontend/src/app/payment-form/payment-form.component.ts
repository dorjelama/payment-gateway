import { Component } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { PaymentService } from "../services/payment.service";
import { Router } from "@angular/router";

@Component({
  selector: "app-payment-form",
  templateUrl: "./payment-form.component.html",
  styleUrls: ["./payment-form.component.css"],
})
export class PaymentFormComponent {
  paymentForm: FormGroup;
  processing = false;
  successMessage = "";
  errorMessage = "";
  transactionId = ""; // Store the transaction ID

  constructor(
    private fb: FormBuilder,
    private paymentService: PaymentService,
    private router: Router
  ) {
    this.paymentForm = this.fb.group({
      amount: ["", [Validators.required, Validators.min(0.01)]],
      currency: ["USD", Validators.required],
      customerName: ["", Validators.required],
      customerEmail: ["", [Validators.required, Validators.email]],
      paymentMethod: ["CARD", Validators.required],
      cardOrAccountNumber: [
        "",
        [Validators.required, Validators.pattern(/^\d{16}$/)],
      ],
      cvv: ["", [Validators.required, Validators.pattern(/^\d{3,4}$/)]],
    });
  }

  onSubmit() {
    if (this.paymentForm.valid) {
      this.processing = true;
      const paymentData = this.paymentForm.value;

      this.paymentService.processPayment(paymentData).subscribe({
        next: (response) => {
          this.processing = false;
          this.transactionId = response.transactionId; // Save the transaction ID
          this.successMessage = `Payment is being processed. Track your transaction using ID: ${this.transactionId}`;
          this.errorMessage = "";
          this.paymentForm.reset({ currency: "USD" }); // Reset form with default currency
        },
        error: (err) => {
          this.processing = false;
          this.errorMessage =
            err.error?.message || "Payment failed. Please try again.";
          this.successMessage = "";
        },
      });
    }
  }
}
