module MainApp

open System
open System.Drawing
open System.Windows.Forms
open System.Windows
open System.Windows.Controls
open System.Windows.Media.Imaging
open FSharpx
open TOSCheckerTrayApp.ViewModel

open TOSCheckerService
open System.ServiceModel

type MainWindow = XAML<"MainWindow.xaml">
type ContextMenu = System.Windows.Forms.ContextMenu
type MenuItem = System.Windows.Forms.MenuItem

let app = new Application()

let toggleWindowVisibility (window : Window) (e : MouseEventArgs) =
    match window.Visibility with
    | Visibility.Visible -> window.Hide()
    | _ when DateTime.Now.Ticks - (window.DataContext :?> MainWindowViewModel).ClosedTimestamp > 2000000L -> 
        window.Show()
        window.Activate() |> ignore
    | _ -> ()


let createTrayIconContextMenu() =
    let menu = new ContextMenu()
    let exit = new MenuItem()
    menu.MenuItems.AddRange [| exit |]
    exit.Index <- 0
    exit.Text <- "C&lose"
    exit.Click.Add(fun _ -> app.Shutdown())
    menu

let createTrayIcon (window : Window) =
    let trayIcon = new NotifyIcon()
    let resource = new Uri("pack://application:,,,/tos-dr.ico") |> Application.GetResourceStream 
    trayIcon.Visible <- true
    trayIcon.Text <- "Terms of Service; Didn't Read"
    trayIcon.Icon <- new Icon(resource.Stream, 32, 32)
    trayIcon.MouseClick.Add(fun e -> toggleWindowVisibility window e)
    trayIcon.ContextMenu <- createTrayIconContextMenu()
    app.Exit.Add(fun _ -> trayIcon.Dispose())

let testReceive data =
    System.Diagnostics.Debug.Print("Received " + data)

let startServer() =
    let server = new ServiceHost(new TOSCheckerService(testReceive), new Uri("http://localhost:8123"))
    server.Open()
    app.Exit.Add(fun _ -> server.Close())
    server

let loadWindow() =
    let window = MainWindow().Root
    let server = startServer()
    window.Deactivated.Add(fun _ -> 
        (window.DataContext :?> MainWindowViewModel).ClosedTimestamp <- DateTime.Now.Ticks
        window.Hide())
    createTrayIcon window
    window

[<STAThread>]
app.Run(loadWindow()) |> ignore
