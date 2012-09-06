module TOSChecker
open System
open System.Runtime.InteropServices

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

[<Literal>]
let bhoKeyName = """Software\Microsoft\Windows\CurrentVersion\Explorer\Browser Helper Objects"""

[<ComVisible(true); ClassInterface(ClassInterfaceType.None); Guid("F6E787A0-BE33-406B-964E-9497E80589A7")>]
type BHO() =
    let mutable webBrowser : WebBrowser = null
    let mutable document : HTMLDocument = null

    let OnDocumentComplete _ url =
        document <- webBrowser.Document :?> HTMLDocument
        System.Windows.Forms.MessageBox.Show("URL loaded: " + url.ToString()) |> ignore

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

    [<ComRegisterFunction>]
    static member RegisterBHO (bhoType : Type) =
        let registryKey =
            match Registry.LocalMachine.OpenSubKey(bhoKeyName, true) with
            | null -> Registry.LocalMachine.CreateSubKey(bhoKeyName)
            | key -> key
        let guid = bhoType.GUID.ToString("B")
        let ourKey = 
            match registryKey.OpenSubKey(guid) with
            | null -> registryKey.CreateSubKey(guid)
            | key -> key
        ourKey.SetValue("Alright", 1)
        registryKey.Close()
        ourKey.Close()

    [<ComUnregisterFunction>]
    static member UnregisterBHO (bhoType : Type) =
        let registryKey = Registry.LocalMachine.OpenSubKey(bhoKeyName, true)
        if not (registryKey = null) then
            registryKey.DeleteSubKey(bhoType.GUID.ToString("B"), false)
