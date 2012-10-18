namespace TOSCheckerTrayApp.Model

open System
open System.Runtime.Serialization
open System.Runtime.Serialization.Json
open System.Text.RegularExpressions

[<DataContract>]
type ServiceItemTosBackPrivacy =
    { [<DataMember(Name = "name")>] mutable Name : string
      [<DataMember(Name = "url")>] mutable Url : Uri }

[<DataContract>]
type ServiceItemTosBackTerms =
    { [<DataMember(Name = "name")>] mutable Name : string
      [<DataMember(Name = "url")>] mutable Url : Uri }

[<DataContract>]
type ServiceItemTosBack =
    { [<DataMember(Name = "sitename")>] mutable Sitename : string
      [<DataMember(Name = "privacy")>] mutable Privacy : ServiceItemTosBackPrivacy
      [<DataMember(Name = "terms")>] mutable Terms : ServiceItemTosBackTerms }

[<DataContract>]
type ServiceItemTosDr = { [<DataMember(Name = "rated")>] mutable Rated : string }

[<DataContract>]
type ServiceItem =
    { [<DataMember(Name = "id")>] mutable Id : string
      [<DataMember(Name = "type")>] mutable Type : string
      [<DataMember(Name = "name")>] mutable Name : string
      [<DataMember(Name = "url")>] mutable Url : Uri
      [<DataMember(Name = "tosback2")>] mutable TosBack : ServiceItemTosBack
      [<DataMember(Name = "tosdr")>] mutable TosDr : ServiceItemTosDr
      [<DataMember(Name = "related")>] mutable Related : string[]
      [<DataMember(Name = "keywords")>] mutable Keywords : string[]
      // late init
      mutable UrlRegex : Regex
      mutable Points : DataPointItem[]
      mutable Links : Links }
