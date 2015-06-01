namespace CodeBlog.CodeFormat

module internal Model = 
    type TokenType = 
            | Type of string // typekind
            | Keyword 
            | Method 
            | Local
            | Field
            | Property 
            | Parameter
            | Comment 
            | Nothing 
            | Literal
    type Token = {
        Text : string;
        Type : TokenType
        ToolTip : string;
    }