module TOSCheckerTrayApp.Service.TOSDR

open System
open System.Net
open System.Diagnostics
open System.Text.RegularExpressions
open Newtonsoft.Json

open TOSCheckerTrayApp.Model

let deserialize<'T> (data : string) =
//    let serializer = new DataContractJsonSerializer(typeof<'T>)
//    serializer.ReadObject(stream) :?> 'T
    JsonConvert.DeserializeObject<'T>(data)

let loadService name =
    let client = new WebClient()
    let uri = sprintf "http://tos-dr.info/services/%s.json" name
    try client.DownloadString(uri) |> deserialize<ServiceItem> |> Some with
    | :? Reflection.TargetInvocationException as e -> 
        let response = (e.InnerException :?> WebException).Response :?> HttpWebResponse
        match response.StatusCode with
        | HttpStatusCode.NotFound -> sprintf "Service [%s] could not be found" name |> Debug.WriteLine; None
        | _ -> sprintf "Unexpected HTTP error trying to load service [%s]: %s" name e.InnerException.Message |> Debug.WriteLine; None
    | e -> sprintf "Unexpected exception trying to load and parse data for service [%s]: %s" name e.Message |> Debug.WriteLine; None

let mergeService (KeyValue (k : string, v : ServicesItem)) = async {
    match loadService k with
    | Some(service) ->
        if service.Url <> null then
            let domain = match service.Url.IsAbsoluteUri with
                | true -> service.Url.Host
                | false -> service.Url.OriginalString
            service.UrlRegex <- new Regex("https?://(?:[^:]\.)?" + domain + "(?:/.*)?")
        service.Points <- v.Points
        service.Links <- v.Links
        return Some(service)
    | None -> return None
}

let loadServices callback =
    let client = new WebClient()
    let uri = new Uri("http://tos-dr.info/index/services.json")
    try
        let services = client.DownloadString(uri) |> deserialize<Services>
        Seq.map mergeService services |> Async.Parallel |> Async.RunSynchronously |> Seq.choose id
    with
    | :? Reflection.TargetInvocationException as e -> 
        let response = (e.InnerException :?> WebException).Response :?> HttpWebResponse
        match response.StatusCode with
        | HttpStatusCode.NotFound -> sprintf "Service list could not be found" |> Debug.WriteLine; raise e
        | _ -> sprintf "Unexpected HTTP error trying to load service list: %s" e.InnerException.Message |> Debug.WriteLine; raise e
    | e -> sprintf "Unexpected exception trying to load and parse services: %s" e.Message |> Debug.WriteLine; raise e
