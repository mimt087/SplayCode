using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using SplayCode.Controls;

namespace SplayCode
{

    public partial class BlockControl : UserControl
    {
        private VirtualSpaceControl virtualSpace;
        private EditorControl editor;

        public BlockControl(string label, string documentPath)
        {
            InitializeComponent();
            virtualSpace = VirtualSpaceControl.Instance;
            editor = new EditorControl(documentPath);
            contentSpace.Children.Add(editor);
            this.label.Content = label;
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

        void onLeftResizeDelta(object sender, DragDeltaEventArgs e)
        {
            if (Width - e.HorizontalChange >= 150)
            {
                // Adjust block position
                Thickness t = Margin;
                t.Left = t.Left + e.HorizontalChange;
                Margin = t;

                // Adjust block size
                Width = Width - e.HorizontalChange;
            }
            
        }

        void onRightResizeDelta(object sender, DragDeltaEventArgs e)
        {
            if (Width + e.HorizontalChange >= 150)
            {
                // Adjust block position
                Thickness t = Margin;
                t.Left = t.Left + e.HorizontalChange;
                Margin = t;

                // Adjust block size
                Width = Width + e.HorizontalChange;
            }
        }

        void onBottomResizeDelta(object sender, DragDeltaEventArgs e)
        {
            if (Height + e.VerticalChange >= 150)
            {
                // Adjust block position
                Thickness t = Margin;
                t.Top = t.Top + e.VerticalChange;
                Margin = t;

                // Adjust block size
                Height = Height + e.VerticalChange;
            }

        }

        void onBottomRightResizeDelta(object sender, DragDeltaEventArgs e)
        {
            onRightResizeDelta(sender, e);
            onBottomResizeDelta(sender, e);
        }

        void onBottomLeftResizeDelta(object sender, DragDeltaEventArgs e)
        {
            onLeftResizeDelta(sender, e);
            onBottomResizeDelta(sender, e);
        }

    }
}
