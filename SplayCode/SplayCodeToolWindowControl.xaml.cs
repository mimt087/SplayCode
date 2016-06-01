//------------------------------------------------------------------------------
// <copyright file="ToolWindow1Control.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SplayCode
{
    using Microsoft.VisualStudio.Shell;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ToolWindow1Control.
    /// </summary>
    public partial class SplayCodeToolWindowControl : UserControl
    {

        private Point firstPoint = new Point();
        private List<ChromeControl> items = new List<ChromeControl>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindow1Control"/> class.
        /// </summary>
        public SplayCodeToolWindowControl()
        {
            this.InitializeComponent();
        }

        public void AddItem(ChromeControl item)
        {
            item.SetParent(this);
            items.Add(item);
            cavRoot.Children.Add(item);
            Canvas.SetLeft(item, 200 * items.Count);
            Canvas.SetTop(item, 50);
            InitMouseCapture(item);
        }

        public void RemoveItem(ChromeControl item)
        {
            items.Remove(item);
            cavRoot.Children.Remove(item);
        }

        public List<Image> FetchAllImages()
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

            foreach (ChromeControl cc in chromes)
            {
                images.Add((Image)cc.scrollView.Content);
            }

            return images;
            //foreach (ChromeControl cc in chromes)
            //{
            //    images.Add(ChromeControl.)
            //}
            //IEnumerable<ChromeControl> images = cavRoot.Children.OfType(ChromeControl);
            //return images;
        }

        public void InitMouseCapture(ChromeControl element)
        {
            element.MouseLeftButtonDown += (ss, ee) =>
            {
                firstPoint = ee.GetPosition(this);
                element.CaptureMouse();
            };

            element.MouseMove += (ss, ee) =>
            {
                if (ee.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                {
                    Point temp = ee.GetPosition(this);
                    Point res = new Point(firstPoint.X - temp.X, firstPoint.Y - temp.Y);

                    if (temp.X > 0)
                    {
                        Canvas.SetLeft(element, Canvas.GetLeft(element) - res.X);
                    }

                    if (temp.Y > 0)
                    {
                        Canvas.SetTop(element, Canvas.GetTop(element) - res.Y);
                    }
                    firstPoint = temp;
                }
            };
            element.MouseUp += (ss, ee) => { element.ReleaseMouseCapture(); };
        }

    }
}