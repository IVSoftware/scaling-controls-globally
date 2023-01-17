One way to **scale winforms controls with window size** is to use nested TableLayoutPanel controls and setting the rows and columns to use percent rather than absolute sizing. 

Then, place your controls in the cells and anchor them on all four sides. For buttons that have a backgound image set to stretch this is all you need to do. However, controls that use text may require a custom means to scale the font. This is especially true for the `ComboBox` controls you're using because the size is a function of the font not the other way around. 


The above screenshots utilize an extension to do a binary search that changes the font size until a target control height is reached. Something _like_ this would work but you'll probably want to do more testing than I did.

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





