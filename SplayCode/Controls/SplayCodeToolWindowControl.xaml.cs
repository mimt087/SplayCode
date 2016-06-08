//------------------------------------------------------------------------------
// <copyright file="ToolWindow1Control.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SplayCode
{
    using Microsoft.VisualStudio.Shell;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ToolWindow1Control.
    /// </summary>
    public partial class SplayCodeToolWindowControl : UserControl
    {

        private List<ChromeControl> items = new List<ChromeControl>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindow1Control"/> class.
        /// </summary>
        public SplayCodeToolWindowControl()
        {
            this.InitializeComponent();
        }

        public void AddItem(ChromeControl item, bool load, double x, double y)
        {
            item.SetParent(this);
            items.Add(item);
            cavRoot.Children.Add(item);

            if (load)
            {
                Thickness t = new Thickness();
                t.Left = x;
                t.Top = y;
                item.Margin = t;
            } else
            {
                Thickness t = new Thickness();
                t.Left = 200 * items.Count;
                t.Top = 50;
                item.Margin = t;
            }          
        }

        public void RemoveItem(ChromeControl item)
        {
            items.Remove(item);
            cavRoot.Children.Remove(item);
        }

        public void RemoveAll()
        {
            cavRoot.Children.Clear();
        }

        public List<ChromeControl> FetchAllChromes()
        {
            //ToolWindowPane window = this.package.FindToolWindow(typeof(ToolWindow1), 0, true);
            List<Image> images = new List<Image>();
            List<ChromeControl> chromes = new List<ChromeControl>();
            foreach (UIElement element in cavRoot.Children)
            {
                if (element is ChromeControl)
                {
                    chromes.Add((ChromeControl)element);
                }
            }

            //foreach (ChromeControl cc in chromes)
            //{
            //    images.Add((Image)cc.scrollView.Content);
            //}

            return chromes;
            //foreach (ChromeControl cc in chromes)
            //{
            //    images.Add(ChromeControl.)
            //}
            //IEnumerable<ChromeControl> images = cavRoot.Children.OfType(ChromeControl);
            //return images;
        }

        

    }
}