namespace CodeBlog.Extension
{
    using System;
    using System.ComponentModel.Design;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.TextManager.Interop;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Editor;

    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CopyToHtmlCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid MenuGroup = new Guid("d9d8f688-6aa6-40ba-b357-812511fa0763");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        private IVsEditorAdaptersFactoryService adaptersFactory;
        private IVsTextManager txtMgr;


        /// <summary>
        /// Initializes a new instance of the <see cref="CopyToHtmlCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private CopyToHtmlCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }
            this.package = package;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            if (commandService != null)
            {
                CommandID menuCommandID = new CommandID(MenuGroup, CommandId);
                EventHandler eventHandler = this.ExportSelectionToHtml;
                OleMenuCommand menuItem = new OleMenuCommand(eventHandler, menuCommandID);
                menuItem.BeforeQueryStatus += CheckIfVisible;
                commandService.AddCommand(menuItem);
            }

            var componentModel = (IComponentModel)((IServiceProvider)package).GetService(typeof(SComponentModel));
            adaptersFactory = componentModel.GetService<IVsEditorAdaptersFactoryService>();
            txtMgr = (IVsTextManager)ServiceProvider.GetService(typeof(SVsTextManager));
        }

        private void ExportSelectionToHtml(object sender, EventArgs e)
        {
            var exportItem = GetExportItem();
            var result = CodeBlog.CodeFormat.CsToHtml.ExportToHtml(exportItem.startLine, exportItem.endLine, exportItem.root, exportItem.model);


            if (exportItem != null)
            {
                System.Windows.Forms.Clipboard.SetText(result);
            }
        }

        private ExportItem GetExportItem()
        {
            IVsTextView vTextView;
            txtMgr.GetActiveView(1, null, out vTextView);

            if (vTextView == null)
            {
                return null;
            }

            IWpfTextView wpfViewCurrent = this.adaptersFactory.GetWpfTextView(vTextView);
            ITextBuffer textCurrent = wpfViewCurrent.TextBuffer;

            var currentDocument = Microsoft.CodeAnalysis.Text.Extensions
                .GetRelatedDocuments(textCurrent)
                .First();

            var rootNode = currentDocument
                .GetSyntaxRootAsync()
                .Result;

            var lineStart = wpfViewCurrent
                .Selection
                .Start
                .Position
                .GetContainingLine()
                .LineNumber;

            var lineEnd = wpfViewCurrent
                .Selection
                .End
                .Position
                .GetContainingLine()
                .LineNumber;

            return new ExportItem(
                Math.Min(lineStart, lineEnd),
                Math.Max(lineStart, lineEnd),
                rootNode,
                currentDocument.GetSemanticModelAsync().Result);
        }

        private void CheckIfVisible(object sender, EventArgs e)
        {
            var menuCommand = sender as OleMenuCommand;
            if (menuCommand != null)
            {
                menuCommand.Visible = false;
                if (!string.IsNullOrEmpty(GetSelectedText()))
                {
                    menuCommand.Visible = true;
                }
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CopyToHtmlCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new CopyToHtmlCommand(package);
        }

        private string GetSelectedText()
        {
            IVsTextView vTextView;
            txtMgr.GetActiveView(1, null, out vTextView);
            string selectedText;
            vTextView.GetSelectedText(out selectedText);
            return selectedText;
        }
    }
}
