namespace WebhookApp

open System.Collections.Concurrent

module TransactionStore =
    let private processed = ConcurrentDictionary<string, bool>()

    let isDuplicate (id: string) =
        processed.ContainsKey(id)

    let register (id: string) =
        processed.TryAdd(id, true) |> ignore
