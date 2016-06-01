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
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ChromeControl : UserControl
    {
        private SplayCodeToolWindowControl splayCodeToolWindow;

        public ChromeControl(Image img, String labelString)
        {
            InitializeComponent();
            baseCanvas.Children.Add(img);
            label.Content = labelString;
        }

        public void SetParent(SplayCodeToolWindowControl splayCodeToolWindow)
        {
            this.splayCodeToolWindow = splayCodeToolWindow;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            splayCodeToolWindow.RemoveItem(this);
        }

        void onDragDelta(object sender, DragDeltaEventArgs e)
        {
            /*//Move the Thumb to the mouse position during the drag operation
            double yadjust = baseCanvas.Height + e.VerticalChange;
            double xadjust = baseCanvas.Width + e.HorizontalChange;
            if ((xadjust >= 0) && (yadjust >= 0))
            {
                baseCanvas.Width = xadjust;
                baseCanvas.Height = yadjust;
                Thickness t = myThumb.Margin;
                t.Left = t.Left + e.HorizontalChange;
                t.Top = t.Top + e.VerticalChange;
                myThumb.Margin = t;
            }*/
        }

        void onDragStart(object sender, DragStartedEventArgs e)
        {
            myThumb.Background = Brushes.Orange;
            /*//Move the Thumb to the mouse position during the drag operation
            double yadjust = baseCanvas.Height + e.VerticalChange;
            double xadjust = baseCanvas.Width + e.HorizontalChange;
            if ((xadjust >= 0) && (yadjust >= 0))
            {
                baseCanvas.Width = xadjust;
                baseCanvas.Height = yadjust;
                Thickness t = myThumb.Margin;
                t.Left = t.Left + e.HorizontalChange;
                t.Top = t.Top + e.VerticalChange;
                myThumb.Margin = t;
            }*/
        }
    }
}
