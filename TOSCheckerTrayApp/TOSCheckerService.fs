namespace TOSCheckerService

open System
open System.ServiceModel
open TOSCheckerService.Contracts

[<ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)>]
type TOSCheckerService (loader : string -> unit) =
    interface ITOSCheckerService with
        member x.LoadDomain value =
            loader value
