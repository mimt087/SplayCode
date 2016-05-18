//------------------------------------------------------------------------------
// <copyright file="ToolWindow1Control.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SplayCode
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ToolWindow1Control.
    /// </summary>
    public partial class ToolWindow1Control : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindow1Control"/> class.
        /// </summary>
        public ToolWindow1Control()
        {
            this.InitializeComponent();
            INIT();
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "ToolWindow1");
        }

        private Point firstPoint = new Point();

        public void INIT()
        {
            imgSource.MouseLeftButtonDown += (ss, ee) =>
            {
                firstPoint = ee.GetPosition(this);
                imgSource.CaptureMouse();
            };

            imgSource.MouseMove += (ss, ee) =>
            {
                if (ee.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                {
                    Point temp = ee.GetPosition(this);
                    Point res = new Point(firstPoint.X - temp.X, firstPoint.Y - temp.Y);

                    if (temp.X > 0)
                    {
                        Canvas.SetLeft(imgSource, Canvas.GetLeft(imgSource) - res.X);
                    }

                    if (temp.Y > 0)
                    {
                        Canvas.SetTop(imgSource, Canvas.GetTop(imgSource) - res.Y);
                    }
                    firstPoint = temp;
                }
            };
            imgSource.MouseUp += (ss, ee) => { imgSource.ReleaseMouseCapture(); };
        }

    }
}