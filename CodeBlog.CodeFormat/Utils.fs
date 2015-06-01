module Utils

open System.IO
open System.Reflection
open Microsoft.VisualStudio.TestTools.UnitTesting
open FSharpx.Option

///
/// Gets the content of an embedded resource.
///
let GetEmbedded resourceName =
    let assembly = Assembly.GetExecutingAssembly()
    maybe {
        use! stream = try
                        Some <| assembly.GetManifestResourceStream(resourceName)
                        with ex -> None
        use reader = new StreamReader(stream)
        return reader.ReadToEnd()
    }

