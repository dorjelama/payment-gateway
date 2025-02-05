import { Component } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { PaymentFormComponent } from './app/components/payment-form.component';
import { TransactionHistoryComponent } from './app/components/transaction-history.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [PaymentFormComponent, TransactionHistoryComponent],
  template: `
    <div class="app-container">
      <h1>Payment System</h1>
      <div class="content">
        <app-payment-form></app-payment-form>
        <app-transaction-history></app-transaction-history>
      </div>
    </div>
  `
})
export class App {
  name = 'Payment System';
}

bootstrapApplication(App, {
  providers: [
    provideHttpClient()
  ]
});