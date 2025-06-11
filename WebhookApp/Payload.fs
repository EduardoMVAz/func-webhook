namespace WebhookApp

open System
open System.Globalization

type WebhookPayload = {
    event: string
    transaction_id: string
    amount: float
    currency: string
    timestamp: string
}

type WebhookRaw = {
    event: string
    transaction_id: string
    amount: string
    currency: string
    timestamp: string
}

module Payload =
    let tryMapToPayload (raw: WebhookRaw) : WebhookPayload option =
        match Double.TryParse(raw.amount, NumberStyles.Float, CultureInfo.InvariantCulture) with
        | true, parsedAmount ->
            Some {
                event = raw.event
                transaction_id = raw.transaction_id
                amount = parsedAmount
                currency = raw.currency
                timestamp = raw.timestamp
            }
        | _ -> None