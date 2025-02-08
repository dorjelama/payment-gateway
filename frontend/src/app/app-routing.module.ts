import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { LoginComponent } from "./login/login.component";
import { PaymentFormComponent } from "./payment-form/payment-form.component";
import { TransactionHistoryComponent } from "./transaction-history/transaction-history.component";
import { AuthGuard } from "./guards/auth.guard";
import { EventLogsComponent } from "./event-logs/event-logs.component";
import { TrackTransactionComponent } from "./track-transaction/track-transaction.component";

const routes: Routes = [
  { path: "", redirectTo: "/login", pathMatch: "full" }, // Default route
  { path: "login", component: LoginComponent }, // Login page
  {
    path: "payment",
    component: PaymentFormComponent,
    canActivate: [AuthGuard],
  }, // Protected route
  {
    path: "history",
    component: TransactionHistoryComponent,
    canActivate: [AuthGuard],
  }, // Protected route
  {
    path: "event-logs",
    component: EventLogsComponent,
    canActivate: [AuthGuard],
  }, // Event Logs page
  { path: 'track', component: TrackTransactionComponent }, // Add this route
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
