using System.ComponentModel;
using System.Diagnostics;
using static System.Windows.Forms.ComboBox;

namespace scaling_controls_globally
{
    internal class ComboBoxEx : ComboBox 
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);

            var text = e.Index == -1 ? Text : Items[e.Index].ToString();

            // Draw the background of the item.
            e.DrawBackground();

            using(var brush = new SolidBrush(ForeColor))
            {
                e.Graphics.DrawString(text, Font, brush, e.Bounds);
            }

            // Draw the focus rectangle if the mouse hovers over an item.
            e.DrawFocusRectangle();
        }
    }
}