namespace TOSCheckerTrayApp.Model

open System
open System.Runtime.Serialization
open System.Runtime.Serialization.Json

[<DataContract>]
type DataPointItemTosDr =
    { [<DataMember(Name = "topic")>] mutable Topic : string
      [<DataMember(Name = "point")>] mutable Point : string
      [<DataMember(Name = "score")>] mutable Score : uint16
      [<DataMember(Name = "tldr")>] mutable TlDr : string }

[<DataContract>]
type DataPointItemSource = { [<DataMember(Name = "terms")>] mutable Terms : Uri }

[<DataContract>]
type DataPointItemMeta = 
    { [<DataMember(Name = "license-for-this-file")>] mutable LicenseForThisFile : string 
      [<DataMember(Name = "author")>] mutable Author: string
      [<DataMember(Name = "contributors")>] mutable Contributors: string[] }

[<DataContract>]
type DataPointItem =
    { [<DataMember(Name = "id")>] mutable Id : string
      [<DataMember(Name = "name")>] mutable Name : string
      [<DataMember(Name = "service")>] mutable Service : string
      [<DataMember(Name = "tosdr")>] mutable TosDr : DataPointItemTosDr
      [<DataMember(Name = "discussion")>] mutable Discussion : Uri
      [<DataMember(Name = "source")>] mutable Source : DataPointItemSource
      [<DataMember(Name = "meta")>] mutable Meta : DataPointItemMeta }
