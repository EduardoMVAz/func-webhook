namespace WebhookApp

open System
open Microsoft.Data.Sqlite
open WebhookApp

module Database =

    let connectionString = "Data Source=transactions.db"

    let initDatabase () =
        use connection = new SqliteConnection(connectionString)
        connection.Open()

        let dropCmd = connection.CreateCommand()
        dropCmd.CommandText <- "DROP TABLE IF EXISTS transactions;"
        dropCmd.ExecuteNonQuery() |> ignore

        let cmd = connection.CreateCommand()
        cmd.CommandText <- """
            CREATE TABLE IF NOT EXISTS transactions (
                transaction_id TEXT PRIMARY KEY,
                event TEXT NOT NULL,
                amount REAL NOT NULL,
                currency TEXT NOT NULL,
                timestamp TEXT NOT NULL
            );
        """
        cmd.ExecuteNonQuery() |> ignore

    let insertTransaction (tx: WebhookPayload) =
        use connection = new SqliteConnection(connectionString)
        connection.Open()

        let cmd = connection.CreateCommand()
        cmd.CommandText <- """
            INSERT OR IGNORE INTO transactions (transaction_id, event, amount, currency, timestamp)
            VALUES ($id, $event, $amount, $currency, $timestamp)
        """

        cmd.Parameters.AddWithValue("$id", tx.transaction_id) |> ignore
        cmd.Parameters.AddWithValue("$event", tx.event) |> ignore
        cmd.Parameters.AddWithValue("$amount", tx.amount) |> ignore
        cmd.Parameters.AddWithValue("$currency", tx.currency) |> ignore
        cmd.Parameters.AddWithValue("$timestamp", tx.timestamp) |> ignore

        cmd.ExecuteNonQuery() |> ignore
    
    let transactionExists (id: string) =
        use connection = new SqliteConnection(connectionString)
        connection.Open()

        let cmd = connection.CreateCommand()
        cmd.CommandText <- "SELECT 1 FROM transactions WHERE transaction_id = $id LIMIT 1"
        cmd.Parameters.AddWithValue("$id", id) |> ignore

        use reader = cmd.ExecuteReader()
        reader.Read()