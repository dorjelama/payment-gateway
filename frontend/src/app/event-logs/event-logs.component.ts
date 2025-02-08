import { Component, OnInit } from '@angular/core';
import { EventService } from '../services/event.service';

@Component({
  selector: 'app-event-logs',
  templateUrl: './event-logs.component.html',
  styleUrls: ['./event-logs.component.css'],
})
export class EventLogsComponent implements OnInit {
  allEventLogs: any[] = [];
  paginatedLogs: any[] = [];
  loading = true;
  error = '';
  totalLogs = 0;
  currentPage = 1;
  pageSize = 10;

  // Table Columns
  displayedColumns: string[] = ['eventType', 'eventData', 'createdAt'];

  constructor(private eventService: EventService) {}

  ngOnInit(): void {
    this.fetchAllEventLogs();
  }

  fetchAllEventLogs(): void {
    this.loading = true;
    this.eventService.getAllEventLogs().subscribe({
      next: (logs) => {
        this.allEventLogs = logs;
        this.totalLogs = logs.length;
        this.paginateLogs();
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load event logs.';
        this.loading = false;
        console.error('Error fetching event logs:', err);
      },
    });
  }

  paginateLogs(): void {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.paginatedLogs = this.allEventLogs.slice(startIndex, endIndex);
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.paginateLogs();
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.paginateLogs();
    }
  }

  get totalPages(): number {
    return Math.ceil(this.totalLogs / this.pageSize);
  }
}