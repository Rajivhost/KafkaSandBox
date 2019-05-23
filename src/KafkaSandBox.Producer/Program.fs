open System
open Jet.ConfluentKafka.FSharp
open Confluent.Kafka
open Confluent.Kafka.Admin
open Serilog
open NETCore.Encrypt

[<EntryPoint>]
let main argv =
    let logger =
        LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger()
    logger.Information("Kafka Producer started")
    let topicName = "test-messages"
    let broker = "localhost:9092"

    use adminClient =
        new AdminClientConfig()
        |> fun conf ->
            conf.BootstrapServers <- broker
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
    let message = "Hi everyone"
    let key = message |> EncryptProvider.Sha256
    use producer = (logger, cfg, topicName) |> KafkaProducer.Create
    (key, message)
    |> producer.ProduceAsync
    |> Async.RunSynchronously
    |> ignore
    0 // return an integer exit code
