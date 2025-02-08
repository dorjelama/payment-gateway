import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HttpClientModule } from "@angular/common/http";
import { RouterModule } from "@angular/router";
import { CommonModule } from "@angular/common"; // Import CommonModule
// Import your components
import { AppComponent } from "./app.component";
import { LoginComponent } from "./login/login.component";
import { PaymentFormComponent } from "./payment-form/payment-form.component";
import { TransactionHistoryComponent } from "./transaction-history/transaction-history.component";
import { EventLogsComponent } from "./event-logs/event-logs.component";

// Import routing module
import { AppRoutingModule } from "./app-routing.module";
import { provideAnimationsAsync } from "@angular/platform-browser/animations/async";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";

// Angular Material Modules
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatNativeDateModule } from '@angular/material/core';
import { MatCardModule } from '@angular/material/card'; // Add this line
import { MatButtonModule } from '@angular/material/button'; // Optional: For buttons
import { MatTableModule } from '@angular/material/table'; // Optional: For tables
import { MatSelectModule } from '@angular/material/select'; // For mat-select
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner'; // For mat-progress-spinner
import { FlexLayoutModule } from '@angular/flex-layout';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    PaymentFormComponent,
    TransactionHistoryComponent,
    EventLogsComponent,
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    AppRoutingModule, // Include the routing module
    CommonModule,
    MatDatepickerModule,
    MatInputModule,
    MatFormFieldModule,
    MatNativeDateModule,
    MatCardModule, // Add this line
    MatButtonModule, // Optional: For buttons
    MatTableModule, // Optional: For tables
    MatSelectModule, // Add this line
    MatProgressSpinnerModule, // Add this line
    FlexLayoutModule, // Add this line
  ],
  providers: [provideAnimationsAsync()],
  bootstrap: [AppComponent], // Bootstrap the root component
})
export class AppModule {}
