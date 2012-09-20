namespace TOSCheckerTrayApp.ViewModel
open System
open System.Windows
open TOSCheckerService
open TOSCheckerTrayApp.Service

type MainWindowViewModel() as self =
    inherit ViewModelBase()
    
    let mutable closedTimeStamp = 0L
    let mutable items = [| |]
    let mutable domain = null
    let mutable service = None

    let mutable server = null
    
    let receiver (data : string) =
        self.Domain <- data

    do server <- TOSCheckerService.startServer Application.Current receiver

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

    member x.HasItems with get() = items.Length > 0
    member x.HasNoItems with get() = not x.HasItems

    member x.Service
        with get() = service
        and set(value) = 
            service <- value
            x.OnPropertyChanged("Service")

    member x.HasService with get() = service = None
    member x.HasNoService with get() = not x.HasService

    member x.Domain
        with get() = domain
        and set(value) =
            domain <- value
            x.OnPropertyChanged("Domain")
            TOSDR.loadService value (fun service -> x.Service <- Some(service))

    member x.HideWindow (window : Window) =
            x.ClosedTimestamp <- DateTime.Now.Ticks
            window.Hide()
