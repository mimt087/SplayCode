﻿//------------------------------------------------------------------------------
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
    using EnvDTE80;
    using EnvDTE;
    using Data;
    using Controls;    /// <summary>
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
    public class SplayCodeToolWindow : ToolWindowPane, IOleCommandTarget, IVsWindowFrameNotify2
    {
        private static SplayCodeToolWindow instance;
        public static SplayCodeToolWindow Instance
        {
            get { return instance; }
        }

        private static DTE2 m_applicationObject = null;
        DTEEvents m_packageDTEEvents = null;

        private static bool isInEditorViewMode = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SplayCodeToolWindow"/> class.
        /// </summary>
        public SplayCodeToolWindow() : base(null)
        {
            this.Caption = "SplayCode";
            instance = this;
            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = VirtualSpaceControl.Instance;
        }

        public override void OnToolWindowCreated()
        {
            DTE dte = (DTE)GetService(typeof(DTE));
            m_applicationObject = dte as DTE2;

            // --- Register key bindings to use in the editor
            var windowFrame = (IVsWindowFrame)Frame;
            var cmdUi = Microsoft.VisualStudio.VSConstants.GUID_TextEditorFactory;
            windowFrame.SetGuidProperty((int)__VSFPROPID.VSFPROPID_InheritKeyBindings,
              ref cmdUi);
            m_packageDTEEvents = ApplicationObject.Events.DTEEvents;
            m_packageDTEEvents.OnBeginShutdown += new _dispDTEEvents_OnBeginShutdownEventHandler(HandleVisualStudioShutDown);

            base.OnToolWindowCreated();
        }

        public static DTE2 ApplicationObject
        {
            get
            {
                return m_applicationObject;
            }
        }

        /// <summary>
        /// This functions asks the users if they want to save the changes before closing Visual Studio
        /// </summary>
        public void HandleVisualStudioShutDown()
        {
            //if there is currently no layout file loaded, and the global undo stack is not empty,
            //ask the user if he/she wants to save the layout in a new file
            if (VirtualSpaceControl.Instance.CurrentLayoutFile.Equals(""))
            {
                if (UndoManager.Instance.StateStack.Count != 0)
                {
                    MessageBoxResult res = System.Windows.MessageBox.Show("Do you want to save the layout?",
                          "SplayCode: Save Layout", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (res == MessageBoxResult.Yes)
                    {
                        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                        saveFileDialog1.Filter = "XML Files (*.xml)|*.xml";
                        saveFileDialog1.Title = "Save a Layout File";
                        saveFileDialog1.ShowDialog();

                        if (saveFileDialog1.FileName != "")
                        {
                            SaveLayoutCommand.Instance.saveLayout(saveFileDialog1.FileName);
                        }
                    }
                }
            }
            //if currently on a layout file and changes are made, ask the user if he/she wants to save the changes
            else
            {
                if (UndoManager.Instance.StateStack.Count != 0)
                {
                    MessageBoxResult res = System.Windows.MessageBox.Show("Do you want to save the changes to the layout?",
                                  "SplayCode: Unsaved changes", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    // If the users wants to save
                    if (res == MessageBoxResult.Yes)
                    {
                        SaveLayoutCommand.Instance.saveLayout(VirtualSpaceControl.Instance.CurrentLayoutFile);
                    }
                }
            }
        }

        /// <summary>
        /// Function for redirecting commands to the editors. Acquired from
        /// https://joshvarty.wordpress.com/2014/08/01/ripping-the-visual-studio-editor-apart-with-projection-buffers/
        /// </summary>
        protected override bool PreProcessMessage(ref Message m)
        {
            if (isInEditorViewMode)
            {
                var pMsg = new MSG[1];
                pMsg[0].hwnd = m.HWnd;
                pMsg[0].message = (uint)m.Msg;
                pMsg[0].wParam = m.WParam;
                pMsg[0].lParam = m.LParam;

                var vsWindowPane = (IVsWindowPane)(EditorViewControl.Instance.Editor.GetTextView());
                return vsWindowPane.TranslateAccelerator(pMsg) == 0;
            }
            else {
                BlockControl block = BlockManager.Instance.ActiveBlock;
                if (block != null)
                {
                    // copy the Message into a MSG[] array, so we can pass
                    // it along to the active core editor's IVsWindowPane.TranslateAccelerator
                    var pMsg = new MSG[1];
                    pMsg[0].hwnd = m.HWnd;
                    pMsg[0].message = (uint)m.Msg;
                    pMsg[0].wParam = m.WParam;
                    pMsg[0].lParam = m.LParam;

                    var vsWindowPane = (IVsWindowPane)(block.Editor.GetTextView());
                    return vsWindowPane.TranslateAccelerator(pMsg) == 0;
                }
                return base.PreProcessMessage(ref m);
            }
        }

        /// <summary>
        /// Function for redirecting commands to the editors. Acquired from
        /// https://joshvarty.wordpress.com/2014/08/01/ripping-the-visual-studio-editor-apart-with-projection-buffers/
        /// </summary>
        int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt,
      IntPtr pvaIn, IntPtr pvaOut)
        {

            var hr =
              (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;

            if (isInEditorViewMode)
            {
                var cmdTarget = (IOleCommandTarget)(EditorViewControl.Instance.Editor.GetTextView());
                hr = cmdTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
                return hr;
            }
            else {
                BlockControl block = BlockManager.Instance.ActiveBlock;
                if (block != null)
                {
                    var cmdTarget = (IOleCommandTarget)(block.Editor.GetTextView());
                    hr = cmdTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
                }
                return hr;              
            }
        }

        /// <summary>
        /// Function for redirecting commands to the editors. Acquired from
        /// https://joshvarty.wordpress.com/2014/08/01/ripping-the-visual-studio-editor-apart-with-projection-buffers/
        /// </summary>
        int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[]
          prgCmds, IntPtr pCmdText)
        {
            var hr =
              (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;

            if (isInEditorViewMode)
            {
                var cmdTarget = (IOleCommandTarget)(EditorViewControl.Instance.Editor.GetTextView());
                hr = cmdTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
                return hr;
            }
            else {               
                BlockControl block = BlockManager.Instance.ActiveBlock;
                if (block != null)
                {
                    var cmdTarget = (IOleCommandTarget)(block.Editor.GetTextView());
                    hr = cmdTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
                }
                return hr;
            }
        }

        /// <summary>
        /// Call this function to toggle whether to redirect commands to the maximized editor view.
        /// </summary>
        public static void SetEditorViewMode(bool isInEditorView)
        {
            isInEditorViewMode = isInEditorView;
        }

        /// <summary>
        /// This function is triggered when there are changes made to the layout or a new layout is created,
        /// and SplayCode tool window is being closed. The function asks the user if he/she wants to save the layout or changes made
        /// </summary>
        /// <param name="pgrfSaveOptions"></param>
        /// <returns></returns>
        public int OnClose(ref uint pgrfSaveOptions)
        {
            //if there is currently no layout file loaded, and the global undo stack is not empty,
            //ask the user if he/she wants to save the layout in a new file
            if (VirtualSpaceControl.Instance.CurrentLayoutFile.Equals(""))
            {
                if (UndoManager.Instance.StateStack.Count != 0)
                {
                    MessageBoxResult res = System.Windows.MessageBox.Show("Do you want to save the layout?",
                          "SplayCode: Save Layout", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                    if (res == MessageBoxResult.Yes)
                    {
                        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                        saveFileDialog1.Filter = "XML Files (*.xml)|*.xml";
                        saveFileDialog1.Title = "Save a Layout File";
                        saveFileDialog1.ShowDialog();

                        if (saveFileDialog1.FileName != "")
                        {
                            SaveLayoutCommand.Instance.saveLayout(saveFileDialog1.FileName);
                        }
                    }
                    else if (res == MessageBoxResult.Cancel)
                    {
                        return VSConstants.E_ABORT;
                    }
                }
            }
            //if currently on a layout file and changes are made, ask the user if he/she wants to save the changes
            else
            {
                if (UndoManager.Instance.StateStack.Count != 0)
                {
                    MessageBoxResult res = System.Windows.MessageBox.Show("Do you want to save the changes to the layout?",
                                  "SplayCode: Unsaved changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                    // If the users wants to save
                    if (res == MessageBoxResult.Yes)
                    {
                        SaveLayoutCommand.Instance.saveLayout(VirtualSpaceControl.Instance.CurrentLayoutFile);

                    }

                    if (res == MessageBoxResult.Cancel)
                    {
                        // If "cancel" is clicked, abort the close
                        return VSConstants.E_ABORT;
                    }
                }
            }
            VirtualSpaceControl.Instance.Reset();
            // Else, exit
            return VSConstants.S_OK;
        }
        
        /// <summary>
        /// Make this the active tool window within Visual Studio.
        /// </summary>
        public void Activate()
        {
            VirtualSpaceControl.Instance.SaveScrollOffsets();
            IVsWindowFrame windowFrame = (IVsWindowFrame)Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
            VirtualSpaceControl.Instance.LoadScrollOffsets();
        }
    }
}