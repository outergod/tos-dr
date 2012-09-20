module TOSCheckerTrayApp.Service.TOSDR

open System
open System.Net
open System.Runtime.Serialization.Json

open TOSCheckerTrayApp.Model

let deserialize<'T> (stream : IO.Stream) =
    let serializer = new DataContractJsonSerializer(typeof<'T>)
    serializer.ReadObject(stream) :?> 'T

let loadService name callback =
    let client = new WebClient()
    let uri = sprintf "http://tos-dr.info/services/%s.json" name
    client.OpenReadCompleted.Add(fun args -> deserialize<ServiceItem> args.Result |> callback)
    client.OpenReadAsync(new Uri(uri))
