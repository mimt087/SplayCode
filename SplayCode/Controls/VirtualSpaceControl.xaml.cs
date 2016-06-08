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

    public partial class VirtualSpaceControl : UserControl
    {

        private List<BlockControl> items = new List<BlockControl>();

        public VirtualSpaceControl()
        {
            this.InitializeComponent();
        }

        public void AddItem(BlockControl item, bool load, double x, double y)
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

        public void RemoveItem(BlockControl item)
        {
            items.Remove(item);
            cavRoot.Children.Remove(item);
        }

        public void RemoveAll()
        {
            cavRoot.Children.Clear();
        }

        public List<BlockControl> FetchAllChromes()
        {
            //ToolWindowPane window = this.package.FindToolWindow(typeof(ToolWindow1), 0, true);
            List<Image> images = new List<Image>();
            List<BlockControl> chromes = new List<BlockControl>();
            foreach (UIElement element in cavRoot.Children)
            {
                if (element is BlockControl)
                {
                    chromes.Add((BlockControl)element);
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