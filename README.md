## Payment Gateway Solution

This repository contains a Payment Gateway solution built using an event-driven architecture . The system consists of a .NET backend API, RabbitMQ for messaging, SQL Server as the database, and an Angular frontend. The solution can be run locally using Docker Compose.

## Overview

The Payment Gateway solution is designed to handle payment processing in a scalable and decoupled manner using an event-driven architecture .

## Event-Driven Architecture
The system leverages RabbitMQ as the message broker to enable asynchronous communication between services. This architecture allows:

- Decoupling : The backend API publishes events (e.g., payment requests) to RabbitMQ, while other services (e.g., notification services) can consume these events independently.
- Scalability : Each service can scale independently based on demand.
- Resilience : If one service fails, others can continue operating without interruption.

Key Components:
- .NET Backend API : Handles payment requests and publishes events to RabbitMQ.
- RabbitMQ : Acts as the message broker for event-driven communication.
- SQL Server : Stores transactional data and payment records.
- Angular Frontend : Provides the user interface for interacting with the payment gateway.

## Key Events in the System
The system processes payments and handles failures through a series of well-defined events. Below is a summary of the key events and their roles:

- Payment Initiation
  
  - Event Name : payment.initiated
  - Description : Published when a payment request is received by the PaymentProcessService.
  - Payload :
  ```
  {
    "TransactionId": "guid",
    "Amount": 100.00,
    "Currency": "USD",
    "CustomerEmail": "customer@example.com" 
  }
  ```
  - Consumer : The Consumer listens to this event, simulates payment processing, and updates the transaction status.
- Payment Status Update
  - Event Name : payment.status.updated
  - Description : Published after the payment simulation completes, indicating the updated status of the transaction.
  - Payload :
  ```
  {
    "TransactionId": "guid",
    "Status": "Success|Failed|Pending",
    "UpdatedAt": "timestamp"
  }
  ```
  - Consumer : Downstream services (e.g., notification systems) can consume this event to notify users or trigger further actions.
- Payment Failure
  - Event Name : payment.failure
  - Description : Published when an error occurs during payment processing (e.g., invalid data, external service failure).
  - Payload :
  ```
  {
    "TransactionId": "guid",
    "ErrorMessage": "Invalid currency format",
    "Timestamp": "timestamp",
    "CorrelationId": "guid"
  }
  ```
  - Consumer : The FailureConsumer listens to this event, logs the failure, notifies administrators, and retries the operation if necessary.
- Retry Mechanism
  - Failed operations are retried using a retry policy (e.g. Polly). The FailureConsumer ensures that retries are logged and monitored.
- Monitoring and Logging
  - All events are logged in the database using the IEventLogService for auditing and debugging.
  - Tools like Kibana or Grafana can be used to visualize and monitor events in real-time.

## Prerequisites
Before setting up the project, ensure you have the following installed:

- Docker Desktop (with Linux containers enabled)
- Docker Compose (included with Docker Desktop)
- .NET SDK (optional, for local development outside containers)
- Node.js and npm (if running Angular commands locally)
- Git
- Angular CLI


## Setup Instructions

Backend Configuration

1. Clone Repository
```
git clone <repository-url>
cd project
```

2. RabbitMQ Installation
```
docker pull rabbitmq:3-management
docker run -d --name rabbitmq \
   -p 5672:5672 \
   -p 15672:15672 \
   -e RABBITMQ_DEFAULT_USER=guest \
   -e RABBITMQ_DEFAULT_PASS=guest \
   rabbitmq:3-management
```

3. Environment Variables
Update the appsettings.json file in the backend folder with the following settings:
```
"DefaultConnection": "Server=xxxxxx;Database=PaymentGateway;User Id=xxxxxxx;Password=xxxxxx;TrustServerCertificate=True;"
```

4. Running Backend
```
cd backend
docker-compose up --build
```

Frontend Configuration

1. Navigate to the frontend folder:
```
cd frontend
```

2. Install Dependencies:
```
npm install
```

3. Configure Environment Variables:

Update the src/environments/environment.ts file with the backend API URL:
```
apiUrl: 'http://localhost:8081'
```
4. Start the Frontend:
```
npm start
```
