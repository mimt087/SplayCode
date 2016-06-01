//------------------------------------------------------------------------------
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
        public const int CommandId = 4130;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("bd3a38f2-41d4-46ef-9317-ac600087ef56");

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
            ToolWindowPane window = this.package.FindToolWindow(typeof(ToolWindow1), 0, true);
            
            IEnumerable<Image> images = ((SplayCodeToolWindowControl)window.Content).FetchAllImages();

            List<Picture> pictures = new List<Picture>();
            foreach (Image img in images)
            {
                Picture pic = new Picture(img.PointToScreen(new Point(0, 0)).X, img.PointToScreen(new Point(0, 0)).Y,
                    img.Source.ToString(), img.ActualHeight, img.ActualWidth);
                pictures.Add(pic);
            }

            XmlSerializer x = new XmlSerializer(typeof(List<Picture>));
            //Stream stream = File.Create(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".xml");
            var xmlwriter = System.Xml.XmlWriter.Create(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".xml");

            x.Serialize(xmlwriter, pictures);
            
            //foreach (Picture p in pictures)
            //{
            //    x.Serialize(xmlwriter, p);
            //}
            

        }
    }
}
