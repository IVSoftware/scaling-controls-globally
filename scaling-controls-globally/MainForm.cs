using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace scaling_controls_globally
{
    public partial class MainForm : Form
    {
        public MainForm() => InitializeComponent();
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if(!DesignMode)
            {
                comboBox1.SelectedIndex = 0;
                IterateControlTree(this, (control) =>
                {
                    if (control is TableLayoutPanel tableLayoutPanel)
                    {
                        tableLayoutPanel.SizeChanged += (sender, e) => _wdtSizeChanged.StartOrRestart();
                    }
                });

                _wdtSizeChanged.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName!.Equals(nameof(WDT.Busy)) && !_wdtSizeChanged.Busy)
                    {
                        IterateControlTree(this, (control) =>
                        {
                            if (control is TableLayoutPanel tableLayoutPanel)
                            {
                                IterateControlTree(tableLayoutPanel, (child) => onAnyCellPaint(tableLayoutPanel, child));
                            }
                        });
                    }
                };
            }
        }

        WDT _wdtSizeChanged = new WDT { Interval = TimeSpan.FromMilliseconds(100) };

        const string MEAS_STRING = "0123456789ABCDEF"; // Representative 16-char string.

        SemaphoreSlim _sslimResizing= new SemaphoreSlim(1);
        private void onAnyCellPaint(TableLayoutPanel tableLayoutPanel, Control control)
        {
            if (!DesignMode)
            {
                if (_sslimResizing.Wait(0))
                {
                    try
                    {
                        if (control is ComboBox comboBox)
                        {
                            comboBox.DrawMode = DrawMode.OwnerDrawFixed;
                            var totalVerticalSpace =
                                comboBox.Margin.Top + comboBox.Margin.Bottom +
                                // I'm surprised that the Margin property
                                // makes a difference here but it does!
                                tableLayoutPanel.Margin.Top + tableLayoutPanel.Margin.Bottom +
                                tableLayoutPanel.Padding.Top + tableLayoutPanel.Padding.Bottom;

                            var pos = tableLayoutPanel.GetPositionFromControl(control);
                            var height = tableLayoutPanel.GetRowHeights()[pos.Row] - totalVerticalSpace;
                            var optimal = comboBox.BinarySearchFontSize(height);
                            comboBox.Font = new Font(comboBox.Font.FontFamily, optimal);
                            comboBox.ItemHeight = height;
                            return;
#if false
                                var graphics = comboBox.CreateGraphics();
                                float min, max;
                                SizeF meas;
                                min = 0; max = comboBox.Font.Size * 2;
                                var font = comboBox.Font;
                                string text = string.IsNullOrWhiteSpace(comboBox.Text) ? MEAS_STRING : comboBox.Text;
                                for (int i = 0; i < 10; i++)
                                {
                                    meas = graphics.MeasureString(text, font);
                                    if (meas.Height < height - 25)
                                    {
                                        // Needs to be bigger
                                        min = font.Size;
                                    }
                                    else if(meas.Height > height - 10)
                                    {
                                        // Needs to be smaller
                                        max = font.Size;
                                    }
                                    else break;
                                    font = new Font(comboBox.Font.FontFamily, (min + max) / 2);
                                }
                                for (int i = 0; i < 10; i++)
                                {
                                    meas = graphics.MeasureString(text, font);
                                    if (meas.Width < width - 50)
                                    {
                                        // Needs to be bigger
                                        min = font.Size;
                                    }
                                    else if (meas.Width > width - 40)
                                    {
                                        // Needs to be smaller
                                        max = font.Size;
                                    }
                                    else break;
                                    font = new Font(comboBox.Font.FontFamily, (min + max) / 2);
                                }
                                comboBox.Font = font;
                                comboBox.ItemHeight = height;
#endif
                        }
                    }
                    finally
                    {
                        _sslimResizing.Release();
                    }
                }
            }
        }

        internal void IterateControlTree(Control control, Action<Control> fx)
        {
            if (control == null)
            {
                control = this;
            }
            fx(control);
            foreach (Control child in control.Controls)
            {
                IterateControlTree(child, fx);
            }
        }
    }
    static class Extensions
    {

        const string MEAS_STRING = "0123456789ABCDEF"; // Representative 16-char string.
        public static float BinarySearchFontSize(this Control control, float containerHeight)
        {
            float
                vertical = BinarySearchVerticalFontSize(control, containerHeight),
                horizontal = BinarySearchHorizontalFontSize(control);
            return Math.Min(vertical, horizontal);
        }
        /// <summary>
        /// Get a font size that produces control size between 90-100% of available height.
        /// </summary>
        private static float BinarySearchVerticalFontSize(Control control, float containerHeight)
        {
            var name = control.Name;
            switch (name)
            {
                case "comboBox1":
                    Debug.WriteLine($"{control.Name}: CONTAINER HEIGHT {containerHeight}");
                    break;
            }
            var proto = new TextBox
            {
                Text = "|", // Vertical height independent of text length.
                Font = control.Font
            };
            using (var graphics = proto.CreateGraphics())
            {
                float
                    targetMin = 0.9F * containerHeight,
                    targetMax = containerHeight,
                    min, max;
                min = 0; max = proto.Font.Size * 2;
                for (int i = 0; i < 10; i++)
                {
                    if(proto.Height < targetMin)
                    {
                        // Needs to be bigger
                        min = proto.Font.Size;
                    }
                    else if (proto.Height > targetMax)
                    {
                        // Needs to be smaller
                        max = proto.Font.Size;
                    }
                    else
                    {
                        break;
                    }
                    var newSizeF = (min + max) / 2;
                    proto.Font = new Font(control.Font.FontFamily, newSizeF);
                    proto.Invalidate();
                }
                return proto.Font.Size;
            }
        }
        private static float BinarySearchHorizontalFontSize(Control control)
        {
            var name = control.Name;
            var text = string.IsNullOrWhiteSpace(control.Text) ? MEAS_STRING : control.Text;
            var protoFont = control.Font;
            using(var g = control.CreateGraphics())
            {
                using (var graphics = control.CreateGraphics())
                {
                    int width =
                        (control is ComboBox) ?
                            control.Width - SystemInformation.VerticalScrollBarWidth :
                            control.Width;
                    float
                        targetMin = 0.9F * width,
                        targetMax = width,
                        min, max;
                    min = 0; max = protoFont.Size * 2;
                    for (int i = 0; i < 10; i++)
                    {
                        var sizeF = g.MeasureString(text, protoFont);
                        if (sizeF.Width < targetMin)
                        {
                            // Needs to be bigger
                            min = protoFont.Size;
                        }
                        else if (sizeF.Width > targetMax)
                        {
                            // Needs to be smaller
                            max = protoFont.Size;
                        }
                        else
                        {
                            break;
                        }
                        var newSizeF = (min + max) / 2;
                        protoFont = new Font(control.Font.FontFamily, newSizeF);
                    }
                }
            }
            return protoFont.Size;
        }
    }
    class WDT : INotifyPropertyChanged
    {
        private int _count = 0;
        public TimeSpan Interval { get; set; } = TimeSpan.FromMilliseconds(1000);

        bool _busy = false;
        public bool Busy
        {
            get => _busy;
            set
            {
                if (!Equals(_busy, value))
                {
                    _busy = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Busy)));
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public void StartOrRestart() 
        {
            Busy = true;
            int countCapture = ++_count;
            Task
                .Delay(Interval)
                .GetAwaiter()
                .OnCompleted(() =>
                {
                    if(countCapture.Equals(_count))
                    {
                        Busy= false;
                    }
                });
        }
    }
}