//------------------------------------------------------------------------------
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

namespace SplayCode
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class LoadLayoutCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4131;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("bd3a38f2-41d4-46ef-9317-ac600087ef56");

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
            List<Picture> pictures = new List<Picture>();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Environment.CurrentDirectory + "\\bin\\Debug";
            openFileDialog1.Filter = "XML Files (*.xml)|*.xml";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ToolWindowPane window = this.package.FindToolWindow(typeof(ToolWindow1), 0, true);

                if ((SplayCodeToolWindowControl)window.Content != null) {
                    ((SplayCodeToolWindowControl)window.Content).RemoveAll();
                }
                string path = openFileDialog1.FileName;

                XmlSerializer x = new XmlSerializer(typeof(List<Picture>));
                StreamReader reader = new StreamReader(path);

                pictures = (List<Picture>)x.Deserialize(reader);
                reader.Close();

                foreach (Picture pic in pictures)
                {
                    System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                    Uri imgPath = new Uri(pic._source);
                    
                    img.Source = new BitmapImage(imgPath);
                    img.Height = pic._height;
                    img.Width = pic._width;

                    ChromeControl imgChrome = new ChromeControl(img, imgPath.Segments[imgPath.Segments.Length - 1]);
                    
                    ((SplayCodeToolWindowControl)window.Content).AddItem(imgChrome, true, pic._X, pic._Y);
                }
            }


        }
    }
}
