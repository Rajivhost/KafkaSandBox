open System
open Jet.ConfluentKafka.FSharp
open Confluent.Kafka
open Confluent.Kafka.Admin
open Serilog
open NETCore.Encrypt

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    let logger =
        LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger()
    let topicName = "test-messages"
    let broker = "localhost:9092"

    let conf =
        new AdminClientConfig()
        |> fun conf ->
            conf.BootstrapServers <- broker
            conf

    use adminClient =
        conf
        |> AdminClientBuilder
        |> fun builder -> builder.Build()
        
//    let createTopicIfNotExist (producer : IProducer<_,_>, topicName: string) =
//        producer.Get

    let meta = adminClient.GetMetadata(TimeSpan.FromSeconds(20.))
    //    let topicSpecification = new TopicSpecification()
    //    topicSpecification.Name <- topicName
    //    topicSpecification.ReplicationFactor <- 1 |> int16
    //    topicSpecification.NumPartitions <- 2
    //
    //    [|topicSpecification|] |> adminClient.CreateTopicsAsync |> Async.AwaitTask |> Async.RunSynchronously |> ignore
    let cfg =
        KafkaProducerConfig.Create
            ("fnstack-producer", Uri broker, Confluent.Kafka.Acks.Leader,
             Confluent.Kafka.CompressionType.Lz4, maxInFlight = 1_000_000)
    let message = "test"
    let key = message |> EncryptProvider.Sha256
    use producer = (logger, cfg, topicName) |> KafkaProducer.Create
    (key, message)
    |> producer.ProduceAsync
    |> Async.RunSynchronously
    |> ignore
    //    use c = CustomConsumer.start cfg args.Parallelism
    //    c.AwaitCompletion() |> Async.RunSynchronously
    0 // return an integer exit code
