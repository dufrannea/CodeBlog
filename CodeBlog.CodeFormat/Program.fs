namespace CodeBlog.CodeFormat

///
/// Main module
///
module CsToHtml = 
    open Microsoft.CodeAnalysis
    open Walker
    open Model
    open HtmlGenerator
    open System.Text
    
    ///
    /// Outputs a syntaxNode to html, between specified lines.
    ///
    let ExportToHtml (startLine : int) (endLine : int) (root : SyntaxNode) (model : SemanticModel) = 
        let walker = new Walker(model, Some (startLine, endLine))
        walker.Visit(root)
        let st = new StringBuilder()
        let result = RenderHtml walker.GeneratedTokens
        result (fun s-> st.Append(s) |> ignore) |> ignore
        st.ToString()
//
//module  Main = 
//    open Microsoft.CodeAnalysis
//    open Microsoft.CodeAnalysis.CSharp
//    open Microsoft.CodeAnalysis.CSharp.Syntax
//    open Walker
//    open Model
//    open System.Net
//    open HtmlGenerator
//    open Utils
//    let code = (GetEmbedded "TestProgram.cs").Value
//
//    [<EntryPoint>]
//    let main argv = 
//        let tree = CSharpSyntaxTree.ParseText(code)
//        let root = tree.GetRoot()
//        let objType = typedefof<System.Object>
//        let compilation = CSharpCompilation
//                            .Create("HelloWorld")
//                            .AddReferences(MetadataReference.CreateFromAssembly(objType.Assembly))
//                            .AddSyntaxTrees(tree)
//        let model = compilation.GetSemanticModel(tree, false)
//
//        let walker = Walker(model,(Some (3,10)))
//        walker.Visit(root)
//        RenderPage "D:\\sandbox\\formatted.html" <| walker.GeneratedTokens
//        0 
