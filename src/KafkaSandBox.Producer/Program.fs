open System
open Jet.ConfluentKafka.FSharp
open Confluent.Kafka
open Serilog
open NETCore.Encrypt
open KafkaSandBox

[<EntryPoint>]
let main argv =
    let logger =
        LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger()
    logger.Information("Kafka Producer started")
    let topicName = kafkaConfig.Topic
    let broker = kafkaConfig.Broker

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
            ("fnstack-producer", Uri broker, Acks.All, CompressionType.Lz4,
             maxInFlight = 5, retries = 10_000_000,
             customize = (fun config ->
             config.EnableIdempotence <- true |> Nullable.op_Implicit))
    let message = "Hi everyone"
    let key = message |> EncryptProvider.Sha256
    use producer = (logger, cfg, topicName) |> KafkaProducer.Create
    (key, message)
    |> producer.ProduceAsync
    |> Async.RunSynchronously
    |> ignore
    0 // return an integer exit code
