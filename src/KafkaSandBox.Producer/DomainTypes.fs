namespace KafkaSandBox.Domain

open FSharp.UMX
open KafkaSandBox

[<Measure>] type productCategoryId
type ProductCategoryId = Guid<productCategoryId>
module ProductCategoryId =
    let toStringN (value:ProductCategoryId) = Guid.toStringN %value
    let tryParse (x:string) = Guid.tryParse x |> Option.map UMX.tag<productCategoryId>

[<Measure>] type productCategoryName
type ProductCategoryName = string<productCategoryName>

