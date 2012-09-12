namespace TOSCheckerService.Contracts

open System.Runtime.Serialization
open System.ServiceModel

[<ServiceContract>]
type ITOSCheckerService =
    [<OperationContract>]
    abstract LoadDomain : value:string -> unit
