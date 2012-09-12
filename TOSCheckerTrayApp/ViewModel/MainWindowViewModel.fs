namespace TOSCheckerTrayApp.ViewModel

type MainWindowViewModel() =
    inherit ViewModelBase()

    let mutable closedTimeStamp = 0L

    member x.ClosedTimestamp
        with get() = closedTimeStamp
        and set(value) = closedTimeStamp <- value
