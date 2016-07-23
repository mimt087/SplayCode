using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using SplayCode.Controls;
using System.Windows.Media;

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
            this.GotFocus += BlockControl_GotFocus;
            this.GotMouseCapture += BlockControl_GotFocus;
            this.GotTouchCapture += BlockControl_GotFocus;
        }

        private void BlockControl_GotFocus(object sender, RoutedEventArgs e)
        {
            VirtualSpaceControl.Instance.BringToTop(this);
        }

        public void changeColour(Color color)
        {
            chrome.BorderBrush = new SolidColorBrush(color);
            label.Background = new SolidColorBrush(color);
            closeButton.BorderBrush = new SolidColorBrush(color);
        }

        public void Reposition(double xDelta, double yDelta)
        {
            Thickness t = this.Margin;
            t.Left = t.Left + xDelta;
            t.Top = t.Top + yDelta;
            this.Margin = t;
            RefreshVirtualSpaceSize();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            virtualSpace.RemoveBlock(this);
        }

        void onDragDelta(object sender, DragDeltaEventArgs e)
        {
            Reposition(e.HorizontalChange, e.VerticalChange);
        }

        void onLeftResizeDelta(object sender, DragDeltaEventArgs e)
        {
            if (Width - e.HorizontalChange >= this.MinWidth)
            {
                // Adjust block size
                Width = Width - e.HorizontalChange;
                Reposition(e.HorizontalChange, 0);
            }
        }

        void onRightResizeDelta(object sender, DragDeltaEventArgs e)
        {
            if (Width + e.HorizontalChange >= this.MinWidth)
            {
                // Adjust block size
                Width = Width + e.HorizontalChange;
            }
        }

        void onBottomResizeDelta(object sender, DragDeltaEventArgs e)
        {
            if (Height + e.VerticalChange >= this.MinHeight)
            {
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

        void RefreshVirtualSpaceSize()
        {
            VirtualSpaceControl.Instance.ExpandToSize(Margin.Left + Width, Margin.Top + Height);
        }

        public EditorControl GetEditor()
        {
            return editor;
        }

    }
}
