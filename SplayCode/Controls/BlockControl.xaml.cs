using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SplayCode
{
    
    public partial class BlockControl : UserControl
    {

        private VirtualSpaceControl virtualSpace;

        private Point firstPoint = new Point();
        private bool isDraggingThumb = false;
        private Image content;

        public BlockControl(string label, Image content)
        {
            InitializeComponent();
            virtualSpace = VirtualSpaceControl.Instance;
            this.content = content;
            baseCanvas.Children.Add(content);
            this.label.Content = label;
            InitMouseCapture();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            virtualSpace.RemoveBlock(this);
        }

        void onDragDelta(object sender, DragDeltaEventArgs e)
        {
            Thickness t = this.Margin;
            t.Left = t.Left + e.HorizontalChange;
            t.Top = t.Top + e.VerticalChange;
            this.Margin = t;
            
        }

        void onDragStart(object sender, DragStartedEventArgs e)
        {
            dragger.Background = Brushes.Orange;
            isDraggingThumb = true;
        }

        void onDragComplete(object sender, DragCompletedEventArgs e)
        {
            dragger.Background = Brushes.Blue;
            isDraggingThumb = false;
        }

        void onResizeDelta(object sender, DragDeltaEventArgs e)
        {
            //Move the Thumb to the mouse position during the drag operation
            double yadjust = border.Height + e.VerticalChange;
            double xadjust = border.Width + e.HorizontalChange;
            
            if ((xadjust >= 0) && (yadjust >= 0))
            {
                border.Width = xadjust;
                border.Height = yadjust;
               
                Thickness t = myThumb.Margin;
                t.Left = t.Left + e.HorizontalChange;
                t.Top = t.Top + e.VerticalChange;
                myThumb.Margin = t;
            }
        }

        void onResizeStart(object sender, DragStartedEventArgs e)
        {
            myThumb.Background = Brushes.Orange;
            
        }

        void onResizeComplete(object sender, DragCompletedEventArgs e)
        {
            myThumb.Background = Brushes.Blue;
        }

        public Image GetImage()
        {
            return content;
        }

        private void InitMouseCapture()
        {
            label.MouseLeftButtonDown += (ss, ee) =>
            {
                firstPoint = ee.GetPosition(label);
                label.CaptureMouse();
                Debug.WriteLine("Mouse clicked");
            };

            label.MouseMove += (ss, ee) =>
            {

                if (ee.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                {
                    Point temp = ee.GetPosition(label);
                    Point res = new Point(firstPoint.X - temp.X, firstPoint.Y - temp.Y);

                    if (temp.X > 0)
                    {
                        Thickness t = this.Margin;
                        t.Left = t.Left - res.X;
                        this.Margin = t;
                    }

                    if (temp.Y > 0)
                    {
                        Thickness t = this.Margin;
                        t.Top = t.Top - res.Y;
                        this.Margin = t;
                    }
                    firstPoint = temp;
                }
            };
            label.MouseUp += (ss, ee) => { label.ReleaseMouseCapture(); };
        }
    }
}
