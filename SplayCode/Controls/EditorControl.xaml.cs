using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using System.ComponentModel.Composition.Hosting;
using Microsoft.VisualStudio.ComponentModelHost;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System.Runtime.InteropServices;

namespace SplayCode.Controls
{
    /// <summary>
    /// Custom control which hosts a WpfTextViewHost for displaying an editor.
    /// The source code for initializing the editor is largely taken from:
    /// https://joshvarty.wordpress.com/2014/08/01/ripping-the-visual-studio-editor-apart-with-projection-buffers/
    /// </summary>
    public partial class EditorControl : UserControl
    {

        public static Microsoft.VisualStudio.OLE.Interop.IServiceProvider OLEServiceProvider;

        private ITextEditorFactoryService textEditorFactory;
        private IComponentModel componentModel;
        private IVsTextView currentlyFocusedTextView;
        private IVsInvisibleEditorManager invisibleEditorManager;
        private IVsEditorAdaptersFactoryService editorAdapter;

        private string filePath;
        public string FilePath
        {
            get { return filePath; }
        }

        public EditorControl(string filePath)
        {
            InitializeComponent();

            this.filePath = filePath;

            componentModel = (IComponentModel)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SComponentModel));
            textEditorFactory = componentModel.GetService<ITextEditorFactoryService>();
            invisibleEditorManager = (IVsInvisibleEditorManager)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsInvisibleEditorManager));
            editorAdapter = componentModel.GetService<IVsEditorAdaptersFactoryService>();
            this.Content = CreateEditor(filePath);

        }

        private IVsInvisibleEditor GetInvisibleEditor(string filePath)
        {
            IVsInvisibleEditor invisibleEditor;
            ErrorHandler.ThrowOnFailure(this.invisibleEditorManager.RegisterInvisibleEditor(
                filePath
                , pProject: null
                , dwFlags: (uint)_EDITORREGFLAGS.RIEF_ENABLECACHING
                , pFactory: null
                , ppEditor: out invisibleEditor));
            RegisterDocument(filePath);

            return invisibleEditor;
        }

        public IWpfTextViewHost CreateEditor(string filePath)
        {
            SplayCodeToolWindow.ApplicationObject.ItemOperations.OpenFile(filePath);
            SplayCodeToolWindow.Instance.Activate();

            //IVsInvisibleEditors are in-memory represenations of typical Visual Studio editors.
            //Language services, highlighting and error squiggles are hooked up to these editors
            //for us once we convert them to WpfTextViews. 
            var invisibleEditor = GetInvisibleEditor(filePath);

            var docDataPointer = IntPtr.Zero;
            Guid guidIVsTextLines = typeof(IVsTextLines).GUID;

            ErrorHandler.ThrowOnFailure(invisibleEditor.GetDocData(
                fEnsureWritable: 1
                , riid: ref guidIVsTextLines
                , ppDocData: out docDataPointer));

            IVsTextLines docData = (IVsTextLines)Marshal.GetObjectForIUnknown(docDataPointer);

            //Create a code window adapter
            var codeWindow = editorAdapter.CreateVsCodeWindowAdapter(OLEServiceProvider);
            ErrorHandler.ThrowOnFailure(codeWindow.SetBuffer(docData));

            //Get a text view for our editor which we will then use to get the WPF control for that editor.
            IVsTextView textView;
            ErrorHandler.ThrowOnFailure(codeWindow.GetPrimaryView(out textView));

            currentlyFocusedTextView = textView;
            var textViewHost = editorAdapter.GetWpfTextViewHost(textView);
            return textViewHost;
        }

        /// <summary>
        /// This function is acquired from
        /// http://stackoverflow.com/questions/32937232/syntax-highlight-doesnt-work-for-ivsinvisibleeditor.
        /// At the moment it is unclear where to use this function in the editor initialization process.
        /// </summary>
        uint RegisterDocument(string targetFile)
        {
            //Then when creating the IVsInvisibleEditor, find and lock the document 
            uint itemID;
            IntPtr docData;
            uint docCookie;
            IVsHierarchy hierarchy;
            var runningDocTable = (IVsRunningDocumentTable)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsRunningDocumentTable));

            var result = runningDocTable.FindAndLockDocument(
                dwRDTLockType: (uint)_VSRDTFLAGS.RDT_EditLock,
                pszMkDocument: targetFile,
                ppHier: out hierarchy,
                pitemid: out itemID,
                ppunkDocData: out docData,
                pdwCookie: out docCookie);

            return docCookie;
        }

        public IVsTextView GetTextView()
        {
            return currentlyFocusedTextView;
        }
    }
}
