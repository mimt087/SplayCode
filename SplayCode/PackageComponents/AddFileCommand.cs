﻿//------------------------------------------------------------------------------
// <copyright file="AddImageCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using System.Windows.Forms;
using System.Windows;

namespace SplayCode
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class AddFileCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 3;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("9da3a146-946a-4fc8-a5a4-029f780074b9");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddFileCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private AddFileCommand(Package package)
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
        public static AddFileCommand Instance
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
            Instance = new AddFileCommand(package);
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
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            //MessageBoxResult res = new MessageBoxResult();
            //bool duplicate = false;
            
            openFileDialog1.RestoreDirectory = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Uri documentPath = new Uri(openFileDialog1.FileName);

                if (VirtualSpaceControl.Instance.HandleDuplicateFiles(openFileDialog1.FileName))
                {
                    VirtualSpaceControl.Instance.AddBlock(documentPath.Segments[documentPath.Segments.Length - 1],
                            openFileDialog1.FileName);
                }
                //foreach (BlockControl bc in VirtualSpaceControl.Instance.FetchAllBlocks())
                //{
                //    if (bc.GetEditor().getFilePath().Equals(openFileDialog1.FileName))
                //    {
                //        res = System.Windows.MessageBox.Show("The file is already added in the layout. Proceed with adding the file?",
                //          "Duplicate file", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                //        duplicate = true;
                //    }
                //}

                //if (duplicate)
                //{
                //    if (res == MessageBoxResult.Yes)
                //    {
                //        VirtualSpaceControl.Instance.AddBlock(documentPath.Segments[documentPath.Segments.Length - 1],
                //            openFileDialog1.FileName);
                //    }
                //}
                //else
                //{
                //    VirtualSpaceControl.Instance.AddBlock(documentPath.Segments[documentPath.Segments.Length - 1],
                //        openFileDialog1.FileName);
                //}
            }
        }
    }
}
