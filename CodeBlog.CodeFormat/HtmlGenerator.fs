namespace CodeBlog.CodeFormat

///
/// Turns generated tokens to Html
///
module internal HtmlGenerator = 
    open Utils
    open FSharpx.State
    open Walker
    open Model
    open System.Net
    open System.IO

    let classNameForType kind = 
        match kind with     
            | Type t-> "type " + t
            | Keyword -> "keyword"
            | Method  -> "method"
            | Comment  -> "comment"
            | Literal -> "stringliteral"
            | _ -> ""

    let write s = state {
        let! logger = getState
        return logger s
    }

    let isTooltip (t:string) () = 
        if ( not <| t.Equals("") ) then
            "tooltip"
        else ""
            
    let writeToken (token:Token) = state {
        match token.Type with 
            | Nothing -> do! write <| WebUtility.HtmlEncode token.Text
            | _ -> let txt = sprintf "<span class='%s %t'><span>%s</span>%s</span>" 
                                <| (classNameForType token.Type) 
                                <| (isTooltip token.ToolTip) 
                                <| (token.ToolTip)  
                                <| WebUtility.HtmlEncode token.Text
                   do! write txt
    }

    let writeTokens tokens = state {
        for token in tokens do
            do! writeToken token
    }

    let writeHtmlPage content = state {
        do! write "<!DOCTYPE html>"
        do! write "<html>"
        do! write "<head>"
        do! write "<style>"
        do! write <| (GetEmbedded "embed.css").Value
        do! write "</style>"
        do! write "<script>"
        do! write <| (GetEmbedded "embed.js").Value
        do! write "</script>"
        do! write "</head>"
        do! write "<body>"
        do! write "<pre class='linenumbers'>1</br>2</br></pre>"
        do! write "<pre class='code'>"
        do! content
        do! write "</pre>"
        do! write "</body>"
        do! write "</html>"
    }

    let RenderHtml tokens = 
        let writtenTokens = writeTokens <| Seq.ofList tokens
        writeHtmlPage writtenTokens

    let RenderPage path tokens= 
        use logger = new StreamWriter("D:\\sandbox\\formatted.html");
        let writtenTokens = writeTokens <| Seq.ofList tokens
        let allTogether = writeHtmlPage writtenTokens
        allTogether (fun s -> printf "%s" s)  |> fst
        allTogether (fun s -> logger.Write s) |> fst
