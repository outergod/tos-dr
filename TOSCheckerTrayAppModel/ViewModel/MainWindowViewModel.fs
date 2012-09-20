namespace TOSCheckerTrayApp.ViewModel
open System
open System.ComponentModel
open System.Windows
open TOSCheckerService
open TOSCheckerTrayApp.Service

type MainWindowViewModel() as self =
    inherit ViewModelBase()
    
    let inDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject())
    let mutable server = null
    let mutable closedTimeStamp = 0L
    let mutable domain = null
    let mutable service = None
    let mutable visible = false
        
    let receiver (data : string) =
        self.Domain <- data

    do 
        if not inDesignMode then 
            server <- TOSCheckerService.startServer Application.Current receiver

    member x.ClosedTimestamp
        with get() = closedTimeStamp
        and set(value) = 
            closedTimeStamp <- value
            x.OnPropertyChanged("ClosedTimestamp")

    member x.Service
        with get() = service
        and set(value) = 
            service <- value
            x.OnPropertyChanged("Service")

    member x.HasService with get() = service <> None
    member x.HasNoService with get() = not x.HasService

    member x.Domain
        with get() = domain
        and set(value) =
            domain <- value
            x.OnPropertyChanged("Domain")
            TOSDR.loadService value (fun service -> x.Service <- Some(service))

    member x.Visible
        with get() =
            match inDesignMode with
            | true -> Visibility.Visible
            | false ->
                match visible with 
                | true -> Visibility.Visible 
                | false -> Visibility.Hidden
        and set(value) = 
            visible <- 
                match value with 
                | Visibility.Visible -> true 
                | _ -> false

    member x.HideWindow (window : Window) =
            x.ClosedTimestamp <- DateTime.Now.Ticks
            window.Hide()
