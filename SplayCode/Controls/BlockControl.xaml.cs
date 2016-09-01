using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using SplayCode.Controls;
using System.Windows.Media;
using SplayCode.Data;
using System.Windows.Input;

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

        private ChromeBarOverlay overlayBar;

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
            this.GotKeyboardFocus += BlockControl_GotKeyboardFocus;
            this.MouseEnter += ShowOverlayBar;
            this.MouseLeave += HideOverlayBar;

            MinHeight = MINIMUM_BLOCK_HEIGHT;
            MinWidth = MINIMUM_BLOCK_WIDTH;
            Height = DEFAULT_BLOCK_HEIGHT;
            Width = DEFAULT_BLOCK_WIDTH;

            overlayBar = new ChromeBarOverlay();
            overlayBar.Width = this.Width;
            overlayBar.Visibility = Visibility.Hidden;
            VirtualSpaceControl.Instance.PutOverlayBar(overlayBar);
        }

        private void BlockControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource != closeButton) && (e.OriginalSource != closeIcon))
            {
                BlockManager.Instance.SetActiveBlock(this);
            }
        }

        private void BlockControl_GotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            if (IsMouseOver)
            {
                BlockManager.Instance.SetActiveBlock(this);
            }
            if (!Equals(BlockManager.Instance.ActiveBlock))
            {
                Keyboard.ClearFocus();
            }
        }

        private void ShowOverlayBar(object sender, MouseEventArgs e)
        {
            //overlayBar.Visibility = Visibility.Visible;
            ScaleTransform scaleTransform1 = new ScaleTransform(1.0 / VirtualSpaceControl.Instance.ZoomLevel, 
                1.0 / VirtualSpaceControl.Instance.ZoomLevel);
            labelBar.RenderTransform = scaleTransform1;
            Thickness t = labelBar.Margin;
            double fullWidth = Width + chrome.BorderThickness.Left * 2;
            t.Left = t.Left + ((Width - fullWidth * VirtualSpaceControl.Instance.ZoomLevel) / 2);
            t.Right = t.Right + ((Width - fullWidth * VirtualSpaceControl.Instance.ZoomLevel) / 2);
            if ((Width - t.Left - t.Right) < 250)
            {
                t.Left = (Width - 250) / 2;
                t.Right = (Width - 250) / 2;
            }
            labelBar.Margin = t;
        }

        private void HideOverlayBar(object sender, MouseEventArgs e)
        {
            //overlayBar.Visibility = Visibility.Hidden;
            ScaleTransform scaleTransform1 = new ScaleTransform(1.0, 1.0);
            labelBar.RenderTransform = scaleTransform1;
            Thickness t = labelBar.Margin;
            t.Left = 0;
            t.Right = 0;            
            labelBar.Margin = t;
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

        public void Position(Thickness m)
        {
            this.Margin = m;
            Thickness t = new Thickness();
            t.Left = m.Left - VirtualSpaceControl.Instance.ScrollView.HorizontalOffset;
            t.Top = m.Top - VirtualSpaceControl.Instance.ScrollView.VerticalOffset;
            overlayBar.Margin = t;
            RefreshVirtualSpaceSize();
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
            VirtualSpaceControl.Instance.CenterViewOn(this);
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
