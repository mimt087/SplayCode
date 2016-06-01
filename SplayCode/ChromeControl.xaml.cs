using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            scrollView.Content = img;
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

        private Image GetImage()
        {
            return (Image)scrollView.Content;
        }
    }
}
