module TOSCheckerTrayApp.Service.TOSDR

open System
open System.Net
open System.Runtime.Serialization.Json
open System.Diagnostics

open TOSCheckerTrayApp.Model

let deserialize<'T> (stream : IO.Stream) =
    let serializer = new DataContractJsonSerializer(typeof<'T>)
    serializer.ReadObject(stream) :?> 'T

let loadService name callback =
    let client = new WebClient()
    let uri = sprintf "http://tos-dr.info/services/%s.json" name
    client.OpenReadCompleted.Add 
        (fun args -> 
            try deserialize<ServiceItem> args.Result |> callback with
            | :? Reflection.TargetInvocationException as e -> 
                let response = (e.InnerException :?> WebException).Response :?> HttpWebResponse
                match response.StatusCode with
                | HttpStatusCode.NotFound -> sprintf "Service [%s] could not be found" name |> Debug.WriteLine
                | _ -> sprintf "Unexpected HTTP error trying to load service [%s]: %s" name e.InnerException.Message |> Debug.WriteLine
            | e -> sprintf "Unexpected exception trying to load and parse data for service [%s]: %s" name e.Message |> Debug.WriteLine)
    client.OpenReadAsync(new Uri(uri))
