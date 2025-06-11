namespace WebhookApp

open System.Net.Http
open System.Text
open System.Text.Json
open System.Threading.Tasks

module WebhookForwarder =

    let private httpClient = new HttpClient()

    let postJson (url: string) (payload: obj) : Task =
        task {
            try
                let json = JsonSerializer.Serialize(payload)
                printfn "Sending POST to %s with body: %s" url json
                let content = new StringContent(json, Encoding.UTF8, "application/json")
                let! response = httpClient.PostAsync(url, content)
                printfn "Response from %s: %d" url (int response.StatusCode)
                return ()
            with ex ->
                printfn "Failed to POST to %s: %s" url ex.Message
                return ()
        }
