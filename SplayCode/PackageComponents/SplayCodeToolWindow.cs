//------------------------------------------------------------------------------
// <copyright file="ToolWindow1.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SplayCode
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using System.Windows.Forms;
    using Microsoft.VisualStudio.OLE.Interop;
    using System.Windows;
    using Microsoft.VisualStudio;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("8d4e6cbb-0bed-4758-976d-d850c6cbd4bd")]
    public class SplayCodeToolWindow : ToolWindowPane //, IVsWindowFrameNotify2
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SplayCodeToolWindow"/> class.
        /// </summary>
        public SplayCodeToolWindow() : base(null)
        {
            this.Caption = "SplayCode";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = VirtualSpaceControl.Instance;
        }

        public override void OnToolWindowCreated()
        {
            // --- Register key bindings to use in the editor
            var windowFrame = (IVsWindowFrame)Frame;
            var cmdUi = Microsoft.VisualStudio.VSConstants.GUID_TextEditorFactory;
            windowFrame.SetGuidProperty((int)__VSFPROPID.VSFPROPID_InheritKeyBindings,
              ref cmdUi);

            base.OnToolWindowCreated();
        }
       
        protected override bool PreProcessMessage(ref Message m)
        {
            BlockControl block = VirtualSpaceControl.Instance.GetActiveBlock();
            if (block != null)
            {
                // copy the Message into a MSG[] array, so we can pass
                // it along to the active core editor's IVsWindowPane.TranslateAccelerator
                var pMsg = new MSG[1];
                pMsg[0].hwnd = m.HWnd;
                pMsg[0].message = (uint)m.Msg;
                pMsg[0].wParam = m.WParam;
                pMsg[0].lParam = m.LParam;

                var vsWindowPane = (IVsWindowPane)(block.GetEditor().GetTextView());
                 return vsWindowPane.TranslateAccelerator(pMsg) == 0;
            }
            return base.PreProcessMessage(ref m);
        }

        //public int OnClose(ref uint pgrfSaveOptions)
        //{
        //    // Check if your content is dirty here, then
        //    SaveLayoutCommand.Instance.saveLayout(Environment.SpecialFolder.ApplicationData.ToString() + "\\temp.xml");
        //    //if ((Environment.SpecialFolder.ApplicationData.ToString() + "\\temp.xml").GetHashCode. ==
        //    //    VirtualSpaceControl.Instance.CurrentLayoutFileName.GetHashCode))
        //    // Prompt a dialog
        //    MessageBoxResult res = System.Windows.MessageBox.Show("Do you want to save the changes to the layout?",
        //                  "Unsaved changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
        //    // If the users wants to save
        //    if (res == MessageBoxResult.Yes)
        //    {
        //        SaveLayoutCommand.Instance.saveLayout(VirtualSpaceControl.Instance.CurrentLayoutFileName);
        //    }

        //    if (res == MessageBoxResult.Cancel)
        //    {
        //        // If "cancel" is clicked, abort the close
        //        return VSConstants.E_ABORT;
        //    }

        //    // Else, exit
        //    return VSConstants.S_OK;
        //}
    }
}