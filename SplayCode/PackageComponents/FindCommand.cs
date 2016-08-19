//------------------------------------------------------------------------------
// <copyright file="FindCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SplayCode.Data;
using Microsoft.VisualBasic;
using System.Windows.Forms;
using System.Windows;

namespace SplayCode
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class FindCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 9;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("9da3a146-946a-4fc8-a5a4-029f780074b9");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private FindCommand(Package package)
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
        public static FindCommand Instance
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
            Instance = new FindCommand(package);
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
            String path = Interaction.InputBox("Which file do you want to find?", "Find", "Type Full Class Name");
            bool found = false;

            foreach (BlockControl bc in BlockManager.Instance.BlockList) {
                if (bc.Editor.FilePath.Contains(path))
                {
                    BlockManager.Instance.SetActiveBlock(bc);
                    VirtualSpaceControl.Instance.FocusViewOn(bc);
                    found = true;
                }
            }

            if (!found)
            {
                System.Windows.MessageBox.Show("The requested file is not found.", "Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
