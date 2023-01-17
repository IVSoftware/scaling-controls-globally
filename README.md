One way to **scale winforms controls with window size** is to use nested `TableLayoutPanel` controls and set the rows and columns to use percent rather than absolute sizing.

[![nested table layout panels][1]][1]

***

Then, place your controls in the cells and anchor them on all four sides. For buttons that have a backgound image set to stretch this is all you need to do. However, controls that use text may require a custom means to scale the font. For example, you're using ComboBox controls where size is a function of the font not the other way around.

[![resizing results][2]][2]

***
The above screenshots utilize an extension to do a binary search that changes the font size until a target control height is reached. Something _like_ this would work but you'll probably want to do more testing than I did. The basic idea is to respond to `TableLayoutPanel` size changes by setting a watchdog timer, and when the timer expires iterate the control tree to apply the `BinarySearchFontSize` extension. You may want to [clone](https://github.com/IVSoftware/scaling-controls-globally.git) the code I used to test this answer and experiment for yourself to see how the pieces fit together.

    static class Extensions
    {
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
        /// <summary>
        /// Get a font size that fits text into available width.
        /// </summary>
        private static float BinarySearchHorizontalFontSize(Control control)
        {
            var name = control.Name;

            // Fine-tuning
            string text;
            if (control is ButtonBase)
            {
                text = "SETTINGS"; // representative max staing
            }
            else
            {
                text = string.IsNullOrWhiteSpace(control.Text) ? "LOCAL FOLDERS" : control.Text;
            }
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


  [1]: https://i.stack.imgur.com/Rnktd.png
  [2]: https://i.stack.imgur.com/USkqU.jpg