namespace TOSChecker
open System
open System.Runtime.InteropServices
open System.ServiceModel
open TOSCheckerService.Contracts

open SHDocVw
open mshtml
open System.IO
open Microsoft.Win32

[<ComVisible(true); InterfaceType(ComInterfaceType.InterfaceIsIUnknown); Guid("FC4801A3-2BA9-11CF-A229-00AA003D7352")>]
type IObjectWithSite = 
    [<PreserveSig>]
    abstract member SetSite : [<MarshalAs(UnmanagedType.IUnknown)>] site : Object -> int
    [<PreserveSig>]
    abstract member GetSite : Guid ref -> nativeint ref -> int

[<ComVisible(true); ProgId("Terms of Service; Didn't Read Helper"); ClassInterface(ClassInterfaceType.None); Guid("F6E787A0-BE33-406B-964E-9497E80589AA")>]
type BHO() =
    let mutable webBrowser : WebBrowser = null
    let mutable document : HTMLDocument = null
    let factory = new ChannelFactory<ITOSCheckerService>(new BasicHttpBinding(), "http://localhost:8123")

    let OnDocumentComplete _ url =
        let uri = new Uri(url.ToString())
        let channel = factory.CreateChannel()
        document <- webBrowser.Document :?> HTMLDocument
        sprintf "Attempting to load %s" uri.Host |> System.Diagnostics.Debug.Print 
        try 
            channel.LoadDomain uri.Host
        with
            | :? EndpointNotFoundException as e -> "Endpoint is not listening" |> System.Diagnostics.Debug.Print
            | e -> sprintf "Couldn't load: %s" e.Message |> System.Diagnostics.Debug.Print
            
    interface IDisposable with
        member x.Dispose() =
            factory.Close()

    interface IObjectWithSite with
        member this.SetSite site =
            let callback = new DWebBrowserEvents2_DocumentCompleteEventHandler(fun (pDisp : obj) (url : byref<obj>) -> OnDocumentComplete pDisp url)
            if not (site = null) then
                webBrowser <- site :?> WebBrowser
                webBrowser.add_DocumentComplete(callback)
            else
                webBrowser.remove_DocumentComplete(callback)
                webBrowser <- null
            0

        member this.GetSite guid ppvSite =
            let punk = Marshal.GetIUnknownForObject(webBrowser)
            let hr = Marshal.QueryInterface(punk, guid, ppvSite)
            Marshal.Release(punk) |> ignore
            hr

    static member bhoKeyName = """Software\Microsoft\Windows\CurrentVersion\Explorer\Browser Helper Objects"""

    [<ComRegisterFunction>]
    static member RegisterBHO (bhoType : Type) =
        let registryKey =
            match Registry.LocalMachine.OpenSubKey(BHO.bhoKeyName, true) with
            | null -> Registry.LocalMachine.CreateSubKey(BHO.bhoKeyName)
            | key -> key
        let guid = bhoType.GUID.ToString("B").ToUpper()
        let ourKey = 
            match registryKey.OpenSubKey(guid) with
            | null -> registryKey.CreateSubKey(guid)
            | key -> key
        ourKey.SetValue("NoExplorer", 1, RegistryValueKind.DWord)
        registryKey.Close()
        ourKey.Close()

    [<ComUnregisterFunction>]
    static member UnregisterBHO (bhoType : Type) =
        let registryKey = Registry.LocalMachine.OpenSubKey(BHO.bhoKeyName, true)
        if not (registryKey = null) then
            registryKey.DeleteSubKey(bhoType.GUID.ToString("B").ToUpper(), false)
