# Webhook Server Project - Eduardo Mendes Vaz

## Summary

This project implements a webhook server in F# that simulates the backend logic of a payment gateway integration. When a payment is confirmed, the gateway sends a POST request to the webhook with transaction details. The server is responsible for validating and then confirming or canceling the transaction using endpoints to do so.

The project follows a functional programming approach and covers the following webhook responsibilities:
- Handling HTTP POST requests with JSON payloads
- Verifying token authenticity
- Validating payload structure and transaction data
- Guaranteeing transaction idempotency
- Sending confirmation/cancellation callbacks
- Persisting valid transactions in a database

---

## Project Specification

The webhook receives a payload in the format:

```json
{
  "event": "payment_success",
  "transaction_id": "abc123",
  "amount": 49.90,
  "currency": "BRL",
  "timestamp": "2025-05-11T16:00:00Z"
}
```

The server must:
- The service must verify that the payload is correct.

- The service must check that the payment is actually valid.

- The service must ensure the uniqueness of the payment.

- If a transaction is valid, it must return 200 and make a request to a confirmation URL.

- If a transaction is invalid, it must not return 400.

- If any information is incorrect (e.g., amount), the transaction must be canceled by making a request.

- If any required information is missing (except transaction_id), the transaction must be canceled by making a request.

- If the token is incorrect, it is a fake transaction and must be ignored.

---

## Architecture and Modules

- `Program.fs`: Main entry point. Sets up the HTTP server and routes.
- `Payload.fs`: Defines data types (`WebhookRaw`, `WebhookPayload`) and the mapping logic.
- `Validation.fs`: Provides utility functions to validate token and payload content.
- `Database.fs`: Handles database creation, cleanup, insertion, and existence checks.
- `WebhookForwarder.fs`: Sends confirmation or cancellation POST requests to external endpoints.

---

## How to Run

In the root of the dotnet project:

1. **Install dependencies**:
   ```bash
   dotnet add package Microsoft.Data.Sqlite
   ```

2. **Build the project**:
   ```bash
   dotnet build
   ```

3. **Run the server**:
   ```bash
   dotnet run
   ```

   The webhook server will start on:
   ```
   http://localhost:5000/webhook
   ```

---

## Testing the Webhook

To test the webhook functionality, use the provided Python script (`test_webhook.py`):

```bash
pip install fastapi uvicorn requests
python test_webhook.py
```

This will:
- Start a mock `/confirmar` and `/cancelar` FastAPI server on port 5001
- Send various test payloads to the F# webhook

---

## Optional Features

| Feature                                              | Status |
|------------------------------------------------------|--------|
| Validate payload fields and formats                  | Implemented   |
| Token verification (`X-Webhook-Token` / `Authorization`) | Implemented     |
| Cancel transaction on invalid values                 | Implemented     |
| Confirm transaction on valid values                  | Implemented     |
| Persist transaction in SQLite                        | Implemented     |
| Implement an HTTPS service                        | Not Implemented     |

---

## AI Usage

Generative AI (ChatGPT) was used to:
- Understand how to use F# for web applications
- Refactor code for most of the modules
- Implement the SQLite data persistence logic in idiomatic F#
- Revise the project's README and make it more concise

---
