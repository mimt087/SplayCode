﻿//------------------------------------------------------------------------------
// <copyright file="SaveLayoutCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using Microsoft.VisualStudio.Text.Editor;
using SplayCode.Controls;
using System.Linq;
using SplayCode.Data;
using System.Windows.Forms;

namespace SplayCode
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class SaveLayoutCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("9da3a146-946a-4fc8-a5a4-029f780074b9");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveLayoutCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private SaveLayoutCommand(Package package)
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
        public static SaveLayoutCommand Instance
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
            Instance = new SaveLayoutCommand(package);
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
            if (VirtualSpaceControl.Instance.CurrentLayoutFile.Equals(""))
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "XML Files (*.xml)|*.xml";
                saveFileDialog1.Title = "Save a Layout File";
                saveFileDialog1.ShowDialog();

                if (saveFileDialog1.FileName != "")
                {
                    saveLayout(saveFileDialog1.FileName);
                }
            } else
            {
                saveLayout(VirtualSpaceControl.Instance.CurrentLayoutFile);
            }
        }

        public void saveLayout (string fileName)
        {
            List<BlockControl> chromes = BlockManager.Instance.BlockList;
            XmlFormat format = new XmlFormat();
            IEnumerable<EditorControl> editors;

            format.VirtualSpaceX = VirtualSpaceControl.Instance.baseGrid.Width;
            format.VirtualSpaceY = VirtualSpaceControl.Instance.baseGrid.Height;
            format.ScrollOffsetHorizontal = VirtualSpaceControl.Instance.ScrollView.HorizontalOffset;
            format.ScrollOffsetVertical = VirtualSpaceControl.Instance.ScrollView.VerticalOffset;
            format.ZoomLevel = VirtualSpaceControl.Instance.ZoomLevel;

            foreach (BlockControl cc in chromes)
            {
                editors = cc.contentSpace.Children.OfType<EditorControl>();
                EditorControl editorControl = editors.First();
                string filepath = editorControl.FilePath;

                Editor editor = new Editor(cc.Margin.Left, cc.Margin.Top, filepath, cc.ActualHeight, cc.ActualWidth, System.Windows.Controls.Panel.GetZIndex(cc), cc.BlockId);
                format.Editors.Add(editor);
            }

            XmlSerializer x = new XmlSerializer(typeof(XmlFormat));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            var xmlwriter = XmlWriter.Create(fileName, settings);

            x.Serialize(xmlwriter, format);

            xmlwriter.Close();
        }
    }
}
