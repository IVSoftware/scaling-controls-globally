One way to **scale winforms controls with window size** is to use nested TableLayoutPanel controls and setting the rows and columns to use percent rather than absolute sizing. Then, place your controls in the cells and anchor them on all four sides.

This only gets you part of the way there, however. For buttons that have a backgound image set to stretch this is all you need to do. Other controls that use text may require a means to scale the font. This is especially true for the `ComboBox` controls you're using because the size is a function of the font not the other way around.

