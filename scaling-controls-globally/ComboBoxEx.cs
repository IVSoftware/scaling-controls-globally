using System.ComponentModel;
using System.Diagnostics;
using static System.Windows.Forms.ComboBox;

namespace scaling_controls_globally
{
    internal class ComboBoxEx : ComboBox { }
    internal class ComboBoxExX : ComboBox
    {
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            DrawMode = DrawMode.OwnerDrawFixed;
            if(Parent is TableLayoutPanel tableLayoutPanel)
            {
                tableLayoutPanel.CellPaint += onCellPaint;
            }
        }
        private void onCellPaint(object? sender, TableLayoutCellPaintEventArgs e)
        {
            if(
                (sender is TableLayoutPanel tableLayoutPanel) && 
                ReferenceEquals(tableLayoutPanel.GetControlFromPosition(e.Column, e.Row), this)) 
            {
                var totalVerticalSpace =
                    Margin.Top + Margin.Bottom +
                    // I'm surprised that the Margin property
                    // makes a difference here but it does!
                    tableLayoutPanel.Margin.Top + tableLayoutPanel.Margin.Bottom +
                    tableLayoutPanel.Padding.Top + tableLayoutPanel.Padding.Bottom;
                AnchorHeight = e.CellBounds.Height - totalVerticalSpace;
            }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int AnchorHeight
        {
            get => _anchorHeight;
            set
            {
                if (!Equals(_anchorHeight, value))
                {
                    _anchorHeight = value;
                    ItemHeight = value;
                }
            }
        }
        int _anchorHeight = 0;
    }
    internal class ComboBoxExY : ComboBox , IMessageFilter
    {
        const int WM_SIZE = 0x0005;
        public ComboBoxExY() 
        {
            Application.AddMessageFilter(this);
            Disposed += (sender, Enabled) => Application.RemoveMessageFilter(this);
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg.Equals(WM_SIZE))
            {
                long
                    width = m.LParam & 0x0000FFFF,
                    height = (m.LParam & 0xFFFF0000) >> 16;

                Debug.WriteLine($"Width: {width} Height: {height}");
            }
            return false;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if(m.Msg.Equals(WM_SIZE))
            {
                long 
                    width = m.LParam & 0x0000FFFF,
                    height = (m.LParam & 0xFFFF0000) >> 16;

                Debug.WriteLine($"Width: {width} Height: {height}");
            }
        }
    }
    internal class ComboBoxExZ : Panel, IComboBox
    {
        readonly ComboBox _cb;
        public ComboBoxExZ()
        {
            _cb = new ComboBox
            {
                DrawMode =  DrawMode.OwnerDrawFixed,
                Size = this.Size,
            };
            Controls.Add(_cb);
            SizeChanged += (sender, e) => 
            { 
                if(sender is Control control)
                {
                    _cb.ItemHeight = control.Height;
                    _cb.Width = control.Width;
                }
            };
        }

        public AutoCompleteStringCollection AutoCompleteCustomSource { get => _cb.AutoCompleteCustomSource; set => _cb.AutoCompleteCustomSource = value; }
        public AutoCompleteMode AutoCompleteMode { get => _cb.AutoCompleteMode; set => _cb.AutoCompleteMode = value; }
        public AutoCompleteSource AutoCompleteSource { get => _cb.AutoCompleteSource; set => _cb.AutoCompleteSource = value; }
        public object DataSource { get => _cb.DataSource; set => _cb.DataSource = value; }
        public DrawMode DrawMode { get => _cb.DrawMode; set => _cb.DrawMode = value; }
        public int DropDownHeight { get => _cb.DropDownHeight; set => _cb.DropDownHeight = value; }
        public ComboBoxStyle DropDownStyle { get => _cb.DropDownStyle; set => _cb.DropDownStyle = value; }
        public int DropDownWidth { get => _cb.DropDownWidth; set => _cb.DropDownWidth = value; }
        public bool DroppedDown { get => _cb.DroppedDown; set => _cb.DroppedDown = value; }
        public FlatStyle FlatStyle { get => _cb.FlatStyle; set => _cb.FlatStyle = value; }
        public bool IntegralHeight { get => _cb.IntegralHeight; set => _cb.IntegralHeight = value; }
        public int ItemHeight { get => _cb.ItemHeight; set => _cb.ItemHeight = value; }

        public ObjectCollection Items => _cb.Items;

        public int MaxDropDownItems { get => _cb.MaxDropDownItems; set => _cb.MaxDropDownItems = value; }
        public int MaxLength { get => _cb.MaxLength; set => _cb.MaxLength = value; }

        public int PreferredHeight => _cb.PreferredHeight;

        public int SelectedIndex { get => _cb.SelectedIndex; set => _cb.SelectedIndex = value; }
        public object SelectedItem { get => _cb.SelectedItem; set => _cb.SelectedItem = value; }
        public string SelectedText { get => _cb.SelectedText; set => _cb.SelectedText = value; }
        public int SelectionLength { get => _cb.SelectionLength; set => _cb.SelectionLength = value; }
        public int SelectionStart { get => _cb.SelectionStart; set => _cb.SelectionStart = value; }
        public bool Sorted { get => _cb.Sorted; set => _cb.Sorted = value; }

        public event DrawItemEventHandler DrawItem
        {
            add
            {
                _cb.DrawItem += value;
            }

            remove
            {
                _cb.DrawItem -= value;
            }
        }

        public event EventHandler DropDown
        {
            add
            {
                _cb.DropDown += value;
            }

            remove
            {
                _cb.DropDown -= value;
            }
        }

        public event EventHandler DropDownClosed
        {
            add
            {
                _cb.DropDownClosed += value;
            }

            remove
            {
                _cb.DropDownClosed -= value;
            }
        }

        public event EventHandler DropDownStyleChanged
        {
            add
            {
                _cb.DropDownStyleChanged += value;
            }

            remove
            {
                _cb.DropDownStyleChanged -= value;
            }
        }

        public event MeasureItemEventHandler MeasureItem
        {
            add
            {
                _cb.MeasureItem += value;
            }

            remove
            {
                _cb.MeasureItem -= value;
            }
        }

        public event EventHandler SelectedIndexChanged
        {
            add
            {
                _cb.SelectedIndexChanged += value;
            }

            remove
            {
                _cb.SelectedIndexChanged -= value;
            }
        }

        public event EventHandler SelectionChangeCommitted
        {
            add
            {
                _cb.SelectionChangeCommitted += value;
            }

            remove
            {
                _cb.SelectionChangeCommitted -= value;
            }
        }

        public event EventHandler TextUpdate
        {
            add
            {
                _cb.TextUpdate += value;
            }

            remove
            {
                _cb.TextUpdate -= value;
            }
        }

        public void BeginUpdate()
        {
            _cb.BeginUpdate();
        }

        public void EndUpdate()
        {
            _cb.EndUpdate();
        }

        public int FindString(string s)
        {
            return _cb.FindString(s);
        }

        public int FindString(string s, int startIndex)
        {
            return _cb.FindString(s, startIndex);
        }

        public int FindStringExact(string s)
        {
            return _cb.FindStringExact(s);
        }

        public int FindStringExact(string s, int startIndex)
        {
            return _cb.FindStringExact(s, startIndex);
        }

        public int GetItemHeight(int index)
        {
            return _cb.GetItemHeight(index);
        }

        public void Select(int start, int length)
        {
            _cb.Select(start, length);
        }

        public void SelectAll()
        {
            _cb.SelectAll();
        }
    }

    public interface IComboBox
    {
        AutoCompleteStringCollection AutoCompleteCustomSource { get; set; }
        AutoCompleteMode AutoCompleteMode { get; set; }
        AutoCompleteSource AutoCompleteSource { get; set; }
        Color BackColor { get; set; }
        Image BackgroundImage { get; set; }
        ImageLayout BackgroundImageLayout { get; set; }
        object DataSource { get; set; }
        DrawMode DrawMode { get; set; }
        int DropDownHeight { get; set; }
        ComboBoxStyle DropDownStyle { get; set; }
        int DropDownWidth { get; set; }
        bool DroppedDown { get; set; }
        FlatStyle FlatStyle { get; set; }
        bool Focused { get; }
        Color ForeColor { get; set; }
        bool IntegralHeight { get; set; }
        int ItemHeight { get; set; }
        ObjectCollection Items { get; }
        int MaxDropDownItems { get; set; }
        Size MaximumSize { get; set; }
        int MaxLength { get; set; }
        Size MinimumSize { get; set; }
        Padding Padding { get; set; }
        int PreferredHeight { get; }
        int SelectedIndex { get; set; }
        object SelectedItem { get; set; }
        string SelectedText { get; set; }
        int SelectionLength { get; set; }
        int SelectionStart { get; set; }
        bool Sorted { get; set; }
        string Text { get; set; }

        event EventHandler BackgroundImageChanged;
        event EventHandler BackgroundImageLayoutChanged;
        event EventHandler DoubleClick;
        event DrawItemEventHandler DrawItem;
        event EventHandler DropDown;
        event EventHandler DropDownClosed;
        event EventHandler DropDownStyleChanged;
        event MeasureItemEventHandler MeasureItem;
        event EventHandler PaddingChanged;
        event PaintEventHandler Paint;
        event EventHandler SelectedIndexChanged;
        event EventHandler SelectionChangeCommitted;
        event EventHandler TextUpdate;

        void BeginUpdate();
        void EndUpdate();
        int FindString(string s);
        int FindString(string s, int startIndex);
        int FindStringExact(string s);
        int FindStringExact(string s, int startIndex);
        int GetItemHeight(int index);
        void ResetText();
        void Select(int start, int length);
        void SelectAll();
        string ToString();
    }
}