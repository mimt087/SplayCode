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

namespace SplayCode.Controls
{
    /// <summary>
    /// Control for the maximized editor view.
    /// </summary>
    public partial class EditorViewControl : UserControl
    {
        private static EditorViewControl instance;
        public static EditorViewControl Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EditorViewControl();
                }
                return instance;
            }
        }

        public EditorControl Editor
        {
            get { return (EditorControl)editor.Content; }
        }

        public EditorViewControl()
        {
            InitializeComponent();          
        }

        public void SetEditor(BlockControl block)
        {
            label.Content = block.Label;
            editor.Content = new EditorControl(block.Editor.FilePath);
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            VirtualSpaceControl.Instance.ExitEditorView();
        }
    }
}
