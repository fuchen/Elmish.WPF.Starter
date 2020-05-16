module WindowLoader

open System
open System.Windows
open System.Windows.Markup
open System.Xaml
open System.IO
open System.Collections.Generic
open System.Reflection


let private watchWindows = new Dictionary<string, Window list>()
let mutable private watcher = None

let private createWindowFromFile (xamlFile: string) =
    use xamlReader = File.Open(xamlFile, FileMode.Open)
    XamlReader.Load(xamlReader):?> Window

let private createWindowFromResource (xamlFile: string) =
    let assembly = Assembly.GetExecutingAssembly()
    use stream = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + xamlFile)
    XamlReader.Load(stream):?> Window

let private rememberLoadedWindow(xamlFile: string, window: Window) =
    let filename = Path.GetFileName(xamlFile)
    match watchWindows.TryGetValue filename with
    | true, windows ->
        watchWindows.[filename] <- window :: windows
    | false, _ ->
        watchWindows.[filename] <- [window]

let startWatch watchDir =
    let handler(e: FileSystemEventArgs) =
        Action(fun () ->
            let xamlFile = e.FullPath
            let filename = Path.GetFileName(xamlFile)
            match watchWindows.TryGetValue filename with
            | true, windows ->
                windows
                |> List.iter (fun window ->
                    let newWindow = createWindowFromFile xamlFile
                    window.Content <- newWindow.Content
                    window.Title <- newWindow.Title
                    window.InvalidateVisual()
                    newWindow.Close() )
            | _ ->
                ())
        |> Application.Current.Dispatcher.InvokeAsync
        |> ignore

    let w = new FileSystemWatcher(watchDir, "*.xaml")
    w.Changed.Add(handler)
    w.Renamed.Add(handler)
    w.Created.Add(handler)
    w.EnableRaisingEvents <- true
    watcher <- Some w

let stopWatch() =
    match watcher with
    | Some w -> w.Dispose()
    | None -> ()
    watcher <- None

let loadWindow (xamlFile: string) =
    let window = createWindowFromResource xamlFile
    if watcher.IsSome then
        rememberLoadedWindow(xamlFile, window)
    window
