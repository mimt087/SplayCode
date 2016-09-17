//------------------------------------------------------------------------------
// <copyright file="AddImageCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using System.Windows.Forms;
using System.Windows;
using SplayCode.Data;

namespace SplayCode
{
    /// <summary>
    /// This is a command class that triggers when 'Add Files' button on the toolbar is clicked
    /// Execution of this command will prompt an 'open file dialog', which allows the users choose which files to open.
    /// Selection of a file or files will open them in the virtual space.
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
            SplayCodeToolWindow.Instance.Activate();

            //enable multi-selection of files
            OpenFileDialog openFileDialog1 = new OpenFileDialog();            
            openFileDialog1.RestoreDirectory = false;
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] filepaths = openFileDialog1.FileNames;
                //if multiple files are being added, 
                if (filepaths.Length > 1)
                {
                    //iterate through individual files and open them in the virtual space.
                    //check if the file is already open in the virtual space
                    foreach (string path in filepaths)
                    {

                        Uri documentPath = new Uri(path);

                        if (ImportManager.Instance.HandleDuplicateFiles(path))
                        {
                            BlockManager.Instance.AddBlock(documentPath.Segments[documentPath.Segments.Length - 1], path);
                        }
                    }
                }
                //if only one file is being opened, check if it already exists in the virtual space, then open.
                else
                {
                    Uri documentPath = new Uri(openFileDialog1.FileName);

                    if (ImportManager.Instance.HandleDuplicateFiles(openFileDialog1.FileName))
                    {
                        BlockManager.Instance.AddBlock(documentPath.Segments[documentPath.Segments.Length - 1],
                                openFileDialog1.FileName);
                    }
                }
            }
        }
    }
}
