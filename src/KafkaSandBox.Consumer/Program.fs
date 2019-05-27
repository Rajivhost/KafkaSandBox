open System
open Jet.ConfluentKafka.FSharp
open Serilog
open Newtonsoft.Json
open Figgle

[<EntryPoint>]
let main argv =
    let logger =
        LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger()
    let banner = FiggleFonts.Starwars.Render("FnStack")
    printfn "%s" banner
    logger.Information("Kafka Consumer started")
    let topicName = "test-messages"
    let broker = "localhost:9092"
    let cfg =
        KafkaConsumerConfig.Create
            ("fnstack-consumer", Uri broker, [ topicName ], "test-service")

    let decoder (x : Confluent.Kafka.ConsumeResult<_, _>) =
        let span = JsonConvert.DeserializeObject<string>(x.Value)
        ""

    let consume msgs =
        async {
            let t = msgs |> Seq.collect decoder
            ()
        }

    use c = BatchedConsumer.Start(logger, cfg, consume)
    c.AwaitCompletion() |> Async.RunSynchronously
    0 // return an integer exit code
