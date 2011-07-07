using System.Collections.Generic;
using System.Drawing;

namespace Xtro.MDX.Utilities.Classes
{
    public class ComboBox : Button
    {
        public struct Item
        {
            public string Text;
            public object Data;

            public Rectangle ActiveRectangle;
            public bool Visible;
        };

        protected int Selected;
        protected int Focused;
        protected int DropHeight;
        protected ScrollBar ScrollBar;
        protected int ScrollBarWidth;
         
        protected bool Opened;

        protected Rectangle TextRectangle;
        protected Rectangle ButtonRectangle;
        protected Rectangle DropdownRectangle;
        protected Rectangle DropdownTextRectangle;
     
        protected List<Item> Items;

        public ComboBox(Dialog Dialog)
            : base(Dialog)
        {
            Type = ControlType.ComboBox;

            DropHeight = 100;

            ScrollBarWidth = 16;
            Selected = -1;
            Focused = -1;
        }

    }
}