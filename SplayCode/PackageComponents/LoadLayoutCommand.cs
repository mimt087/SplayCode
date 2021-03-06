﻿//------------------------------------------------------------------------------
// <copyright file="LoadLayoutCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using SplayCode.Data;

namespace SplayCode
{
    /// <summary>
    /// This is a command class that triggers when 'Load Layout' button on the toolbar is clicked
    /// Execution of this command will prompt an 'open file dialog', in which an XML file can be selected.
    /// Selected XML file is read in using XMLSerialiser class and the input from the XML can be used to
    /// provide information required to reconstruct the spatial layout.
    /// </summary>
    internal sealed class LoadLayoutCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 5;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("9da3a146-946a-4fc8-a5a4-029f780074b9");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadLayoutCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private LoadLayoutCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static LoadLayoutCommand Instance
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
            Instance = new LoadLayoutCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            List<Editor> editorList = new List<Editor>();
            XmlFormat format = new XmlFormat();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            SplayCodeToolWindow.Instance.Activate();

            //openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog1.Filter = "XML Files (*.xml)|*.xml";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.Title = "Load a Layout file";
            openFileDialog1.RestoreDirectory = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // clear the layout if it is not empty
                if ((VirtualSpaceControl)SplayCodeToolWindow.Instance.Content != null) {
                    BlockManager.Instance.RemoveAllBlocks();
                    UndoManager.Instance.Reset();
                    VirtualSpaceControl.Instance.Reset();
                }

                string path = openFileDialog1.FileName;
                VirtualSpaceControl.Instance.CurrentLayoutFile = path;
                XmlSerializer x = new XmlSerializer(typeof(XmlFormat));
                StreamReader reader = new StreamReader(path);

                // read the layout file and produce XmlFormat instance with loaded information
                format = (XmlFormat)x.Deserialize(reader);
                reader.Close();

                // get the list of editors and restore settings from the last save
                editorList = format.Editors;
                VirtualSpaceControl.Instance.LoadLayoutSettings(format.VirtualSpaceX, format.VirtualSpaceY, format.ScrollOffsetHorizontal, format.ScrollOffsetVertical, format.ZoomLevel);
               
                // recreate the editor windows
                for (int i = 0; i < editorList.Count; i++)
                {
                    Editor editor = editorList[i];
                    Uri documentPath = new Uri(editor.source);                          
                    BlockManager.Instance.AddBlock(documentPath.Segments[documentPath.Segments.Length - 1],
                        editor.source, editor.X, editor.Y, editor.height, editor.width, editor.ZIndex, editor.BlockId, false);
                    if (i != 0)
                    {
                        UndoManager.Instance.StateStack.RemoveAt(UndoManager.Instance.StateStack.Count-1);
                    }
                }
            }
        }
    }
}
