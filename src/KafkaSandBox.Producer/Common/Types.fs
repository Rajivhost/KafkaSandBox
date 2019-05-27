namespace KafkaSandBox

open System
open System.Text
open FSharp.UMX

module DateTime =
    let tryParse (s:string) =
        match DateTime.TryParse(s) with
        | (true, d) -> d.ToUniversalTime() |> Some
        | _ -> None
    let replaceDay (day:int) (d:DateTime) = DateTime(d.Year, d.Month, day)
    let addMonths (months:int) (d:DateTime) = d.AddMonths(months)
    let addDays (days:int) (d:DateTime) = d.AddDays(days |> float)
    let toMonthEnd (d:DateTime) = d |> replaceDay 1 |> addMonths 1 |> addDays -1
    let isMonthEnd (d:DateTime) = d = (d |> toMonthEnd)
    let monthRange (startDate:DateTime) (endDate:DateTime) =
        let addDaysToStartDate i = startDate |> addDays i
        let atOrBeforeEndDate d = d <= endDate
        let isMonthEnd d = d |> isMonthEnd
        Seq.initInfinite id
        |> Seq.map addDaysToStartDate
        |> Seq.takeWhile atOrBeforeEndDate
        |> Seq.filter isMonthEnd
    let toUtc (dt:DateTime) = DateTime.SpecifyKind(dt, DateTimeKind.Utc)
//    let toTimestamp (dt:DateTime) = Timestamp.FromDateTime(dt)

module Decimal =
    let round (decimalPlaces:int) (value:decimal<'u>) =
        Decimal.Round(%value, decimalPlaces) |> UMX.tag<'u>

module String =
    let toBytes (s:string) = s |> Encoding.UTF8.GetBytes
    let fromBytes (bytes:byte []) = bytes |> Encoding.UTF8.GetString
    let lower (s:string) = s.ToLower()

module Guid =
    let inline toStringN (x: Guid) = x.ToString "N"
    let tryParse (s:string) =
        match Guid.TryParse(s) with
        | (true, d) -> Some d
        | _ -> None

module Int =
    let tryParse (s:string) =
        match Int32.TryParse(s) with
        | (true, i) -> Some i
        | _ -> None

