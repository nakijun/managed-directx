using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Color = Xtro.MDX.Direct3DX10.Color;

namespace Xtro.MDX.Utilities
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

        protected readonly List<Item[]> Items = new List<Item[]>();

        public ComboBox(Dialog Dialog)
            : base(Dialog)
        {
            Type = ControlType.ComboBox;

            DropHeight = 100;

            ScrollBarWidth = 16;
            Selected = -1;
            Focused = -1;
        }

        public override void Delete()
        {
            RemoveAllItems();
        }

        ~ComboBox()
        {
            Delete();
        }

        public override void SetTextColor(uint Color)
        {
            var Element = Elements[0];

            if (Element != null) Element.FontColor.States[(int)ControlState.Normal] = Color;

            Element = Elements[2];

            if (Element != null) Element.FontColor.States[(int)ControlState.Normal] = Color;
        }

        public override int OnInit()
        {
            return Dialog.InitControl(ScrollBar);
        }

        public override bool HandleKeyDownEvent(KeyEventArgs E)
        {
            if (!Enabled || !Visible) return false;

            // Let the scroll bar have a chance to handle it first
            if (ScrollBar.HandleKeyDownEvent(E)) return true;

            switch (E.KeyCode)
            {
            case Keys.Return:
                if (Opened)
                {
                    if (Selected != Focused)
                    {
                        Selected = Focused;
                        Dialog.SendEvent(Event.ComboBoxSelectionChanged, true, this);
                    }
                    Opened = false;

                    if (!Dialog.KeyboardInput) Dialog.ClearFocus();

                    return true;
                }
                break;

            case Keys.F4:

                Opened = !Opened;

                if (!Opened)
                {
                    Dialog.SendEvent(Event.ComboBoxSelectionChanged, true, this);

                    if (!Dialog.KeyboardInput) Dialog.ClearFocus();
                }

                return true;

            case Keys.Left:
            case Keys.Up:
                if (Focused > 0)
                {
                    Focused--;
                    Selected = Focused;

                    if (!Opened) Dialog.SendEvent(Event.ComboBoxSelectionChanged, true, this);
                }

                return true;

            case Keys.Right:
            case Keys.Down:
                if (Focused + 1 < (int)GetNumberOfItems())
                {
                    Focused++;
                    Selected = Focused;

                    if (!Opened) Dialog.SendEvent(Event.ComboBoxSelectionChanged, true, this);
                }

                return true;
            }

            return false;
        }

        public override bool HandleMouseMoveEvent(MouseEventArgs E, Point Point)
        {
            if (!Enabled || !Visible) return false;

            // Let the scroll bar handle it first.
            if (ScrollBar.HandleMouseMoveEvent(E, Point)) return true;

            if (Opened && DropdownRectangle.Contains(Point))
            {
                // Determine which item has been selected
                for (var I = 0; I < Items.Count; I++)
                {
                    var Item = Items[I];
                    if (Item[0].Visible && Item[0].ActiveRectangle.Contains(Point)) Focused = I;
                }
                return true;
            }

            return false;
        }

        public override bool HandleMouseDownAndDoubleClickEvent(MouseEventArgs E, Point Point)
        {
            if (!Enabled || !Visible) return false;

            // Let the scroll bar handle it first.
            if (ScrollBar.HandleMouseDownAndDoubleClickEvent(E, Point)) return true;

            if (ContainsPoint(Point))
            {
                // Pressed while inside the control
                Pressed = true;

                if (!HasFocus) Dialog.RequestFocus(this);

                // Toggle dropdown
                if (HasFocus)
                {
                    Opened = !Opened;

                    if (!Opened)
                    {
                        if (!Dialog.KeyboardInput) Dialog.ClearFocus();
                    }
                }

                return true;
            }

            // Perhaps this click is within the dropdown
            if (Opened && DropdownRectangle.Contains(Point))
            {
                // Determine which item has been selected
                for (var I = ScrollBar.GetTrackPosition(); I < Items.Count; I++)
                {
                    var Item = Items[I];
                    if (Item[0].Visible && Item[0].ActiveRectangle.Contains(Point))
                    {
                        Focused = Selected = I;
                        Dialog.SendEvent(Event.ComboBoxSelectionChanged, true, this);
                        Opened = false;

                        if (!Dialog.KeyboardInput) Dialog.ClearFocus();

                        break;
                    }
                }

                return true;
            }

            // Mouse click not on main control or in dropdown, fire an event if needed
            if (Opened)
            {
                Focused = Selected;

                Dialog.SendEvent(Event.ComboBoxSelectionChanged, true, this);
                Opened = false;
            }

            // Make sure the control is no longer in a pressed state
            Pressed = false;

            // Release focus if appropriate
            if (!Dialog.KeyboardInput) Dialog.ClearFocus();

            return false;
        }

        public override bool HandleMouseUpEvent(MouseEventArgs E, Point Point)
        {
            if (!Enabled || !Visible) return false;

            // Let the scroll bar handle it first.
            if (ScrollBar.HandleMouseUpEvent(E, Point)) return true;

            if (Pressed && ContainsPoint(Point))
            {
                // Button click
                Pressed = false;
                return true;
            }

            return false;
        }

        public override bool HandleMouseWheelEvent(MouseEventArgs E, Point Point)
        {
            if (!Enabled || !Visible) return false;

            // Let the scroll bar handle it first.
            if (ScrollBar.HandleMouseWheelEvent(E, Point)) return true;

            if (Opened) ScrollBar.Scroll(-E.Delta * SystemInformation.MouseWheelScrollLines);
            else
            {
                if (E.Delta > 0)
                {
                    if (Focused > 0)
                    {
                        Focused--;
                        Selected = Focused;

                        if (!Opened) Dialog.SendEvent(Event.ComboBoxSelectionChanged, true, this);
                    }
                }
                else
                {
                    if (Focused + 1 < (int)GetNumberOfItems())
                    {
                        Focused++;
                        Selected = Focused;

                        if (!Opened) Dialog.SendEvent(Event.ComboBoxSelectionChanged, true, this);
                    }
                }
            }
            return true;
        }

        public override void OnHotkey()
        {
            if (Opened) return;

            if (Selected == -1) return;

            if (Dialog.IsKeyboardInputEnabled()) Dialog.RequestFocus(this);

            Selected++;

            if (Selected >= Items.Count) Selected = 0;

            Focused = Selected;
            Dialog.SendEvent(Event.ComboBoxSelectionChanged, true, this);
        }

        public override bool CanHaveFocus()
        {
            return Visible && Enabled;
        }

        public override void OnFocusOut()
        {
            base.OnFocusOut();

            Opened = false;
        }

        bool ScrollBarInit;

        public override void Render(float ElapsedTime)
        {
            var State = ControlState.Normal;

            if (!Opened) State = ControlState.Hidden;

            // Dropdown box
            var Element = Elements[2];

            // If we have not initialized the scroll bar page size,
            // do that now.

            if (!ScrollBarInit)
            {
                // Update the page size of the scroll bar
                if (Dialog.GetManager().GetFontNode((int)Element.Font)[0].Height != 0) ScrollBar.SetPageSize(DropdownTextRectangle.Height / Dialog.GetManager().GetFontNode((int)Element.Font)[0].Height);
                else ScrollBar.SetPageSize(DropdownTextRectangle.Height);
                ScrollBarInit = true;
            }

            // Scroll bar
            if (Opened) ScrollBar.Render(ElapsedTime);

            // Blend current color
            Element.TextureColor.Blend(State, ElapsedTime);
            Element.FontColor.Blend(State, ElapsedTime);

            Dialog.DrawSprite(Element, ref DropdownRectangle, NearButtonDepth);

            // Selection outline
            var SelectionElement = Elements[3];
            SelectionElement.TextureColor.Current = Element.TextureColor.Current;
            SelectionElement.FontColor.Current = new Color(SelectionElement.FontColor.States[(int)ControlState.Normal]);

            var Font = Dialog.GetFont(Element.Font);
            if (Font != null)
            {
                var CurrentY = DropdownTextRectangle.Top;
                var RemainingHeight = DropdownTextRectangle.Height;
                //WCHAR strDropdown[4096] = {0};

                for (var I = ScrollBar.GetTrackPosition(); I < Items.Count; I++)
                {
                    var Item = Items[I];

                    // Make sure there's room left in the dropdown
                    RemainingHeight -= Font[0].Height;
                    if (RemainingHeight < 0)
                    {
                        Item[0].Visible = false;
                        continue;
                    }

                    Item[0].ActiveRectangle = new Rectangle(DropdownTextRectangle.X, CurrentY, DropdownTextRectangle.Width, CurrentY + Font[0].Height);
                    CurrentY += Font[0].Height;

                    //debug
                    //int blue = 50 * i;
                    //m_pDialog->DrawRect( &pItem->rcActive, 0xFFFF0000 | blue );

                    Item[0].Visible = true;

                    if (Opened)
                    {
                        if (I == Focused)
                        {
                            var DrawRectangle = new Rectangle(DropdownRectangle.X, Item[0].ActiveRectangle.Y - 2, DropdownRectangle.Width, Item[0].ActiveRectangle.Bottom + 2);
                            Dialog.DrawSprite(SelectionElement, ref DrawRectangle, NearButtonDepth);
                            Dialog.DrawText(Item[0].Text, SelectionElement, ref Item[0].ActiveRectangle);
                        }
                        else Dialog.DrawText(Item[0].Text, Element, ref Item[0].ActiveRectangle);
                    }
                }
            }

            var OffsetX = 0;
            var OffsetY = 0;

            State = ControlState.Normal;

            if (!Visible) State = ControlState.Hidden;
            else if (!Enabled) State = ControlState.Disabled;
            else if (Pressed)
            {
                State = ControlState.Pressed;

                OffsetX = 1;
                OffsetY = 2;
            }
            else if (MouseOver)
            {
                State = ControlState.MouseOver;

                OffsetX = -1;
                OffsetY = -2;
            }
            else if (HasFocus) State = ControlState.Focus;

            var BlendRate = (State == ControlState.Pressed) ? 0.0f : 0.8f;

            // Button
            Element = Elements[1];

            // Blend current color
            Element.TextureColor.Blend(State, ElapsedTime, BlendRate);

            var WindowRectangle = ButtonRectangle;
            WindowRectangle.Offset(OffsetX, OffsetY);
            Dialog.DrawSprite(Element, ref WindowRectangle, FarButtonDepth);

            if (Opened) State = ControlState.Pressed;

            // Main text box
            //xTODO: remove magic numbers
            Element = Elements[0];

            // Blend current color
            Element.TextureColor.Blend(State, ElapsedTime, BlendRate);
            Element.FontColor.Blend(State, ElapsedTime, BlendRate);

            Dialog.DrawSprite(Element, ref TextRectangle, NearButtonDepth);

            if (Selected >= 0 && Selected < Items.Count)
            {
                var Item = Items[Selected];
                if (Item != null) Dialog.DrawText(Item[0].Text, Element, ref TextRectangle);
            }
        }

        public override void UpdateRectangles()
        {

            base.UpdateRectangles();

            ButtonRectangle = BoundingBox;
            ButtonRectangle.X = ButtonRectangle.Right - ButtonRectangle.Height;

            TextRectangle = BoundingBox;
            TextRectangle.Width = ButtonRectangle.Left - TextRectangle.Left;

            DropdownRectangle = TextRectangle;
            DropdownRectangle.Offset(0, (int)(0.90f * TextRectangle.Height));
            DropdownRectangle.Height += DropHeight;
            DropdownRectangle.Width -= ScrollBarWidth;

            DropdownTextRectangle = DropdownRectangle;
            DropdownTextRectangle.X += (int)(0.1f * DropdownRectangle.Width);
            DropdownTextRectangle.Width -= (int)(0.1f * DropdownRectangle.Width);
            DropdownTextRectangle.Y += (int)(0.1f * DropdownRectangle.Height);
            DropdownTextRectangle.Height -= (int)(0.1f * DropdownRectangle.Height);

            // Update the scrollbar's rects
            ScrollBar.SetLocation(DropdownRectangle.Right, DropdownRectangle.Top + 2);
            ScrollBar.SetSize(ScrollBarWidth, DropdownRectangle.Height - 2);
            var FontNode = Dialog.GetManager().GetFontNode((int)Elements[2].Font);
            if (FontNode != null && FontNode[0].Height != 0)
            {
                ScrollBar.SetPageSize(DropdownTextRectangle.Height / FontNode[0].Height);

                // The selected item may have been scrolled off the page.
                // Ensure that it is in page again.
                ScrollBar.ShowItem(Selected);
            }
        }

        public int AddItem(string Text, object Data)
        {
            // Validate parameters
            if (string.IsNullOrEmpty(Text)) return (int)Error.InvalidArgument;

            // Create a new item and set the data
            var Item = new Item
            {
                Text = Text,
                Data = Data
            };

            Items.Add(new[] { Item });

            // Update the scroll bar with new range
            ScrollBar.SetTrackRange(0, Items.Count);

            // If this is the only item in the list, it's selected
            if (GetNumberOfItems() == 1)
            {
                Selected = 0;
                Focused = 0;
                Dialog.SendEvent(Event.ComboBoxSelectionChanged, false, this);
            }

            return 0;
        }

        public void RemoveAllItems()
        {
            Items.Clear();
            ScrollBar.SetTrackRange(0, 1);
            Focused = Selected = -1;
        }

        public void RemoveItem(uint Index)
        {
            Items.RemoveAt((int)Index);
            ScrollBar.SetTrackRange(0, Items.Count);
            if (Selected >= Items.Count) Selected = Items.Count - 1;
        }

        public bool ContainsItem(string Text, uint Start = 0u)
        {
            return (-1 != FindItem(Text, Start));
        }

        public int FindItem(string Text, uint Start = 0u)
        {
            if (string.IsNullOrEmpty(Text)) return -1;

            for (var I = (int)Start; I < Items.Count; I++)
            {
                var Item = Items[I];

                if (Item[0].Text == Text) return I;
            }

            return -1;
        }

        public object GetItemData(string Text)
        {
            var I = FindItem(Text);
            if (I == -1) return null;

            var Item = Items[I];
            return Item == null ? null : Item[0].Data;
        }

        public object GetItemData(int Index)
        {
            if (Index < 0 || Index >= Items.Count) return null;

            return Items[Index][0].Data;
        }

        public void SetDropHeight(uint Height)
        {
            DropHeight = (int)Height;
            UpdateRectangles();
        }

        public int GetScrollBarWidth()
        {
            return ScrollBarWidth;
        }

        public void SetScrollBarWidth(int Width)
        {
            ScrollBarWidth = Width;
            UpdateRectangles();
        }

        public int GetSelectedIndex()
        {
            return Selected;
        }

        public object GetSelectedData()
        {
            if (Selected < 0) return null;

            var Item = Items[Selected];
            return Item[0].Data;
        }

        public Item[] GetSelectedItem()
        {
            return Selected < 0 ? null : Items[Selected];
        }

        public uint GetNumberOfItems()
        {
            return (uint)Items.Count;
        }

        public Item[] GetItem(uint Index)
        {
            return Items[(int)Index];
        }

        public int SetSelectedByIndex(uint Index)
        {
            if (Index >= GetNumberOfItems()) return (int)Error.InvalidArgument;

            Focused = Selected = (int)Index;
            Dialog.SendEvent(Event.ComboBoxSelectionChanged, false, this);

            return 0;
        }

        public int SetSelectedByText(string Text)
        {
            if (string.IsNullOrEmpty(Text)) return (int)Error.InvalidArgument;

            var I = FindItem(Text);
            if (I == -1) return (int)Error.Fail;

            Focused = Selected = I;
            Dialog.SendEvent(Event.ComboBoxSelectionChanged, false, this);

            return 0;
        }

        public int SetSelectedByData(object Data)
        {
            for (var I = 0; I < Items.Count; I++)
            {
                var Item = Items[I];

                if (Item[0].Data == Data)
                {
                    Focused = Selected = I;
                    Dialog.SendEvent(Event.ComboBoxSelectionChanged, false, this);
                    return 0;
                }
            }

            return (int)Error.Fail;
        }
    }
}