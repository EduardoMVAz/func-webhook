namespace WebhookApp

module Validation =
    let requiredFields (payload: WebhookPayload) =
        not (System.String.IsNullOrWhiteSpace payload.event) &&
        not (System.String.IsNullOrWhiteSpace payload.currency) &&
        not (System.String.IsNullOrWhiteSpace payload.timestamp) &&
        payload.amount > 0.0
        payload.event = "payment_success"

    let isTokenValid (token: string) =
        token = "meu-token-secreto"

    let validateAmount (payload: WebhookPayload) =
        payload.amount > 0.0
