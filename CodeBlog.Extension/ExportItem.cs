namespace CodeBlog.Extension
{
    using Microsoft.CodeAnalysis;

    internal class ExportItem
    {
        public ExportItem(int startLine, int endLine, SyntaxNode root, SemanticModel model)
        {
            this.startLine = startLine;
            this.endLine = endLine;
            this.root = root;
            this.model = model;
        }

        public int endLine { get; private set; }
        public SemanticModel model { get; private set; }
        public SyntaxNode root { get; private set; }
        public int startLine { get; private set; }
    }
}
