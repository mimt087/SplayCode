using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using SplayCode.Controls;
using System.Windows.Media;
using SplayCode.Data;

namespace SplayCode
{

    public partial class BlockControl : UserControl
    {
        private EditorControl editor;
        private int blockId;
        public int BlockId
        {
            get { return blockId; }
            set { blockId = value; }
        }

        // Default and minimum sizes for a block
        public static readonly double MINIMUM_BLOCK_HEIGHT = 300;
        public static readonly double MINIMUM_BLOCK_WIDTH = 300;
        public static readonly double DEFAULT_BLOCK_HEIGHT = 600;
        public static readonly double DEFAULT_BLOCK_WIDTH = 600;

        // Default highlight and normal colours
        public static readonly Color HIGHLIGHT_COLOR = Color.FromArgb(0xFF, 0xFF, 0xE4, 0x33);
        public static readonly Color NON_HIGHLIGHT_COLOR = Color.FromArgb(0xFF, 0xFF, 0xF2, 0x9D);

        public BlockControl(string label, string documentPath)
        {
            InitializeComponent();
            editor = new EditorControl(documentPath);
            contentSpace.Children.Add(editor);
            this.label.Content = label;
            this.GotFocus += BlockControl_GotFocus;
            this.GotMouseCapture += BlockControl_GotFocus;
            this.GotTouchCapture += BlockControl_GotFocus;
            MinHeight = MINIMUM_BLOCK_HEIGHT;
            MinWidth = MINIMUM_BLOCK_WIDTH;
            Height = DEFAULT_BLOCK_HEIGHT;
            Width = DEFAULT_BLOCK_WIDTH;
        }

        private void BlockControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource != closeButton) && (e.OriginalSource != closeIcon))
            {
                BlockManager.Instance.SetActiveBlock(this);
            }
        }

        public void SetHighlight(bool highlightOn)
        {
            if (highlightOn)
            {
                chrome.BorderBrush = new SolidColorBrush(HIGHLIGHT_COLOR);
                label.Background = new SolidColorBrush(HIGHLIGHT_COLOR);
            }
            else
            {
                chrome.BorderBrush = new SolidColorBrush(NON_HIGHLIGHT_COLOR);
                label.Background = new SolidColorBrush(NON_HIGHLIGHT_COLOR);
            }            
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
            UndoManager.Instance.SaveState();
            BlockManager.Instance.RemoveBlock(this);
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

        private void onDragStarted(object sender, DragStartedEventArgs e)
        {
            //VirtualSpaceControl.Instance.LogEditorInteraction(this);
        }

        void onDoubleClick(object sender, RoutedEventArgs e)
        {
            VirtualSpaceControl.Instance.focusViewOn(this);
        }

        // Scales the label, checkbox and closebutton so that they can remain the same size
        // even when the whole view is zoomed out
        public void RetainLabelBarSize(double zoomLevel)
        {
            //label
        }
    }
}
