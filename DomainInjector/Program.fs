module App

open System.ServiceModel
open TOSCheckerService.Contracts

[<EntryPoint>]
let main argv =
    let factory = new ChannelFactory<ITOSCheckerService>(new BasicHttpBinding(), "http://localhost:8123")
    let channel = factory.CreateChannel()

    match argv.Length with
    | 1 ->
        let domain = argv.[0]
        channel.LoadDomain domain
        0
    | _ -> 1
