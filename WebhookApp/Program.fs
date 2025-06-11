open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open System.Text.Json
open System.Threading.Tasks
open WebhookApp
open WebhookApp.Validation
open WebhookApp.TransactionStore
open WebhookApp.WebhookForwarder
open WebhookApp.Payload


let handleWebhook (ctx: HttpContext) : Task =
    task {
        use reader = new System.IO.StreamReader(ctx.Request.Body)
        let! body = reader.ReadToEndAsync()

        try
            let payloadOption =
                try
                    let raw = JsonSerializer.Deserialize<WebhookRaw>(body, JsonSerializerOptions(PropertyNameCaseInsensitive = true))
                    tryMapToPayload raw
                with ex ->
                    printfn "Deserialization failed: %s" ex.Message
                    None

            let token =
                match ctx.Request.Headers.TryGetValue("X-Webhook-Token") with
                | true, values ->
                    let tokenValue = values.ToString()
                    printfn "Token header received"
                    tokenValue
                | _ ->
                    printfn "Authorization header not present"
                    ""

            match isTokenValid token, payloadOption with
            | false, _ ->
                ctx.Response.StatusCode <- 401
                return! ctx.Response.WriteAsync("Unauthorized")

            | true, None ->
                ctx.Response.StatusCode <- 400
                return! ctx.Response.WriteAsync("Invalid JSON format")

            | true, Some payload when not (requiredFields payload) || not (validateAmount payload) ->
                ctx.Response.StatusCode <- 400
                do! ctx.Response.WriteAsync("Payload validation failed")
                do! postJson "http://localhost:5001/cancelar" payload
                return ()

            | true, Some payload when isDuplicate payload.transaction_id ->
                ctx.Response.StatusCode <- 409
                return! ctx.Response.WriteAsync("Duplicate transaction")

            | true, Some payload ->
                register payload.transaction_id
                ctx.Response.StatusCode <- 200
                do! ctx.Response.WriteAsync("OK")
                do! postJson "http://localhost:5001/confirmar" payload
                return ()

        with ex ->
            ctx.Response.StatusCode <- 400
            return! ctx.Response.WriteAsync("Invalid JSON")
    }



[<EntryPoint>]
let main args =
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(fun webBuilder ->
            webBuilder
                .Configure(fun app ->
                    app.UseRouting()
                       .UseEndpoints(fun endpoints ->
                           endpoints.MapPost("/webhook", System.Func<HttpContext, Task>(handleWebhook)) |> ignore
                       )
                    |> ignore
                )
                .ConfigureServices(fun services ->
                    services.AddRouting() |> ignore
                )
                .UseUrls("http://localhost:5000")
            |> ignore
        )
        .Build()
        .Run()
    0