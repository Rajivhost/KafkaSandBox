[<AutoOpen>]
module KafkaSandBox.Config

open FsConfig

let getConfig<'TConfig when 'TConfig : not struct>() =
    match EnvConfig.Get<'TConfig>() with
    | Ok config -> config
    | Error error ->
        match error with
        | NotFound envVarName ->
            failwithf "Environment variable %s not found" envVarName
        | BadValue(envVarName, value) ->
            failwithf "Environment variable %s has invalid value %s" envVarName
                value
        | NotSupported msg -> failwith msg

[<Convention("EVENTSTORE")>]
type EventStoreConfig =
    { [<DefaultValue("admin")>]
      UserName : string
      [<DefaultValue("changeit")>]
      Password : string
      [<DefaultValue("localhost")>]
      Host : string
      [<DefaultValue("1113")>]
      Port : string
      [<DefaultValue("tcp")>]
      Protocol : string }

[<Convention("KAFKA")>]
type KafkaConfig =
    { [<DefaultValue("localhost:9092")>]
      Broker : string
      [<DefaultValue("test-messages")>]
      Topic : string }

let eventStoreConfig = getConfig<EventStoreConfig>()
let kafkaConfig = getConfig<KafkaConfig>()
