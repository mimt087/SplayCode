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
        public EditorControl Editor
        {
            get { return editor; }
        }

        public string Label
        {
            get { return (string)label.Content; }
        }
        public string DocumentPath
        {
            get { return editor.FilePath; }
        }
        private int blockId;
        public int BlockId
        {
            get { return blockId; }
        }

        // Default and minimum sizes for a block
        public static readonly double MINIMUM_BLOCK_HEIGHT = 300;
        public static readonly double MINIMUM_BLOCK_WIDTH = 300;
        public static readonly double DEFAULT_BLOCK_HEIGHT = 600;
        public static readonly double DEFAULT_BLOCK_WIDTH = 600;

        // Default colours
        public static readonly Color HIGHLIGHT_COLOR = Color.FromArgb(0xFF, 0xFF, 0xEF, 0x9F);
        public static readonly Color NON_HIGHLIGHT_COLOR = Color.FromArgb(0xFF, 0xC2, 0xC3, 0xC9);
        public static readonly Color SELECTION_BORDER_COLOR = Color.FromArgb(0xFF, 0x74, 0x86, 0xA6);

        public BlockControl(string label, string documentPath, int id)
        {
            InitializeComponent();
            editor = new EditorControl(documentPath);
            contentSpace.Children.Add(editor);
            blockId = id;
            this.label.Content = label;
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

        private void maximizeButton_Click(object sender, RoutedEventArgs e)
        {
            VirtualSpaceControl.Instance.EnterEditorView(this);
        }

        void onDragStart(object sender, DragStartedEventArgs e)
        {
            UndoManager.Instance.SaveState();
        }

        void onDragDelta(object sender, DragDeltaEventArgs e)
        {
            BlockManager.Instance.ShiftBlock(this, e.HorizontalChange, e.VerticalChange); 
        }

        void onResizeStart(object sender, DragStartedEventArgs e)
        {
            UndoManager.Instance.SaveState();
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

        void onDoubleClick(object sender, RoutedEventArgs e)
        {
            VirtualSpaceControl.Instance.FocusViewOn(this);
        }

        // Scales the label, checkbox and closebutton so that they can remain the same size
        // even when the whole view is zoomed out
        public void RetainLabelBarSize(double zoomLevel)
        {
            //label
        }

        /// <summary>
        /// Turn the selection border on/off.
        /// </summary>
        public void ToggleSelectionBorder(bool borderOn)
        {
            if (borderOn)
            {
                BorderBrush = new SolidColorBrush(SELECTION_BORDER_COLOR);
            }
            else
            {
                BorderBrush = Brushes.Transparent;
            }
        }

        private void selectionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ToggleSelectionBorder(true);
            BlockManager.Instance.RegisterBlockSelection(this);
        }

        private void selectionCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleSelectionBorder(false);
            BlockManager.Instance.RemoveBlockSelection(this);
        }
    }
}
