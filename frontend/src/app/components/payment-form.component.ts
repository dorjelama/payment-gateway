import { Component } from "@angular/core";
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from "@angular/forms";
import { CommonModule } from "@angular/common";
import { PaymentService } from "../services/payment.service";

@Component({
  selector: "app-payment-form",
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="payment-form">
      <h2>Make a Payment</h2>
      <form [formGroup]="paymentForm" (ngSubmit)="onSubmit()">
        <div class="form-group">
          <label for="amount">Amount</label>
          <input type="number" id="amount" formControlName="amount" />
        </div>

        <div class="form-group">
          <label for="currency">Currency</label>
          <select id="currency" formControlName="currency">
            <option value="USD">USD</option>
            <option value="EUR">EUR</option>
            <option value="GBP">GBP</option>
          </select>
        </div>

        <div class="form-group">
          <label for="cardNumber">Card Number</label>
          <input type="text" id="cardNumber" formControlName="cardNumber" />
        </div>

        <div class="form-group">
          <label for="cardHolder">Card Holder</label>
          <input type="text" id="cardHolder" formControlName="cardHolder" />
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="expiryDate">Expiry Date</label>
            <input
              type="text"
              id="expiryDate"
              formControlName="expiryDate"
              placeholder="MM/YY"
            />
          </div>

          <div class="form-group">
            <label for="cvv">CVV</label>
            <input type="text" id="cvv" formControlName="cvv" />
          </div>
        </div>

        <button type="submit" [disabled]="!paymentForm.valid || processing">
          {{ processing ? "Processing..." : "Submit Payment" }}
        </button>
      </form>
    </div>
  `,
})
export class PaymentFormComponent {
  paymentForm: FormGroup;
  processing = false;

  constructor(private fb: FormBuilder, private paymentService: PaymentService) {
    this.paymentForm = this.fb.group({
      amount: ["", [Validators.required, Validators.min(0.01)]],
      currency: ["USD", Validators.required],
      cardNumber: [
        "",
        [Validators.required, Validators.pattern("^[0-9]{16}$")],
      ],
      cardHolder: ["", Validators.required],
      expiryDate: [
        "",
        [
          Validators.required,
          Validators.pattern("^(0[1-9]|1[0-2])/([0-9]{2})$"),
        ],
      ],
      cvv: ["", [Validators.required, Validators.pattern("^[0-9]{3,4}$")]],
    });
  }

  onSubmit() {
    if (this.paymentForm.valid) {
      this.processing = true;
      this.paymentService.processPayment(this.paymentForm.value).subscribe({
        next: () => {
          this.processing = false;
          this.paymentForm.reset({ currency: "USD" });
        },
        error: () => {
          this.processing = false;
        },
      });
    }
  }
}
