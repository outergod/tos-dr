namespace TOSCheckerTrayApp.ViewModel
open System.ServiceModel

type MainWindowViewModel() =
    inherit ViewModelBase()
    
    let mutable closedTimeStamp = 0L
    let mutable items = [| |]
    let mutable domain = null

    member x.ClosedTimestamp
        with get() = closedTimeStamp
        and set(value) = 
            closedTimeStamp <- value
            x.OnPropertyChanged("ClosedTimestamp")

    member x.Items
        with get() = items
        and set(value) = 
            items <- value
            x.OnPropertyChanged("Items")

    member x.HasNoItems with get() = items.Length = 0

    member x.Domain
        with get() = domain
        and set(value) =
            domain <- value
            x.OnPropertyChanged("Domain")
