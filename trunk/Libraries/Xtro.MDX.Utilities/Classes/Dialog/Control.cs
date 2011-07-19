using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Xtro.MDX.Utilities
{
    public class Control
    {
        protected const float NearButtonDepth = 0.6f;
        protected const float FarButtonDepth = 0.8f;

        protected int ID;                 // ID number
        protected ControlType Type;  // Control type, set once in constructor  
        protected Keys Hotkey;            // Virtual key code for this control's hotkey
        protected object UserData;         // Data associated with this control that is set by user.

        protected bool Enabled;           // Enabled/disabled flag

        protected Rectangle BoundingBox;      // Rectangle defining the active region of the control

        public const int MaximumStates = 6;

        public bool Visible;                // Shown/hidden flag
        public bool MouseOver;              // Mouse pointer is above control
        public bool HasFocus;               // Control has input focus
        public bool IsDefault;              // Is the default control

        // Size, scale, and positioning members
        public int X, Y;
        public int Width, Height;

        // These members are set by the container
        public Dialog Dialog;    // Parent container
        public uint Index;              // Index within the control list

        public readonly List<Element> Elements = new List<Element>();  // All display elements

        public Control(Dialog Dialog = null)
        {
            Type = ControlType.Button;
            this.Dialog = Dialog;
            Enabled = true;
            Visible = true;

            this.Dialog = null;
        }

        public virtual void Delete()
        {
            Elements.Clear();
        }

        ~Control()
        {
            Delete();
        }

        public virtual void UpdateRectangles()
        {
            BoundingBox = new Rectangle(X, Y, Width, Height);
        }

        public virtual int OnInit()
        {
            return 0;
        }

        public virtual void Refresh()
        {
            MouseOver = false;
            HasFocus = false;

            foreach (var Element in Elements)
            {
                Element.Refresh();
            }
        }

        public virtual void Render(float ElapsedTime)
        {
        }

        public virtual bool CanHaveFocus()
        {
            return false;
        }

        public virtual void OnFocusIn()
        {
            HasFocus = true;
        }

        public virtual void OnFocusOut()
        {
            HasFocus = false;
        }

        public virtual void OnMouseEnter()
        {
            MouseOver = true;
        }

        public virtual void OnMouseLeave()
        {
            MouseOver = false;
        }

        public virtual void OnHotkey()
        {
        }

        public virtual bool ContainsPoint(Point Point)
        {
            return BoundingBox.Contains(Point);
        }

        public virtual void SetEnabled(bool Enabled)
        {
            this.Enabled = Enabled;
        }

        public virtual bool GetEnabled()
        {
            return Enabled;
        }

        public virtual void SetVisible(bool Visible)
        {
            this.Visible = Visible;
        }

        public virtual bool GetVisible()
        {
            return Visible;
        }

        public new ControlType GetType()
        {
            return Type;
        }

        public int GetID()
        {
            return ID;
        }

        public void SetID(int ID)
        {
            this.ID = ID;
        }

        public void SetLocation(int X, int Y)
        {
            this.X = X; this.Y = Y;
            UpdateRectangles();
        }

        public void SetSize(int Width, int Height)
        {
            this.Width = Width; this.Height = Height;
            UpdateRectangles();
        }

        public void SetHotkey(Keys Hotkey)
        {
            this.Hotkey = Hotkey;
        }

        public Keys GetHotkey()
        {
            return Hotkey;
        }

        public void SetUserData(object UserData)
        {
            this.UserData = UserData;
        }

        public object GetUserData()
        {
            return UserData;
        }

        public virtual void SetTextColor(uint Color)
        {
            var Element = Elements[0];

            if (Element != null) Element.FontColor.States[(int)ControlState.Normal] = Color;
        }

        public Element GetElement(uint Element)
        {
            return Elements[(int)Element];
        }

        public int SetElement(uint ElementNo, Element Element)
        {
            if (Element == null) return (int)Error.InvalidArgument;

            // Make certain the array is this large
            for (var I = Elements.Count; I <= ElementNo; I++)
            {
                Element NewElement;
                try { NewElement = new Element(); }
                catch { return (int)Error.OutOfMemory; }

                Elements.Add(NewElement);
            }

            // Update the data
            var CurrentElement = Elements[(int)ElementNo];
            CurrentElement.Font = Element.Font;
            CurrentElement.FontColor = Element.FontColor;
            CurrentElement.TextFormat = Element.TextFormat;
            CurrentElement.Texture = Element.Texture;
            CurrentElement.TextureColor = Element.TextureColor;
            CurrentElement.TextureRectangle = Element.TextureRectangle;

            return 0;
        }

        public virtual bool HandleMouseDownAndDoubleClickEvent(MouseEventArgs E, Point Point)
        {
            return false;
        }

        public virtual bool HandleMouseDownEvent(MouseEventArgs E, Point Point)
        {
            return false;
        }

        public virtual bool HandleMouseUpEvent(MouseEventArgs E, Point Point)
        {
            return false;
        }

        public virtual bool HandleMouseMoveEvent(MouseEventArgs E, Point Point)
        {
            return false;
        }

        public virtual bool HandleMouseWheelEvent(MouseEventArgs E, Point Point)
        {
            return false;
        }

        public virtual bool HandleMouseCaptureChangedEvent(EventArgs E)
        {
            return false;
        }

        public virtual bool HandleKeyDownEvent(KeyEventArgs E)
        {
            return false;
        }

        public virtual bool HandleKeyUpEvent(KeyEventArgs E)
        {
            return false;
        }

        public virtual bool HandleResizeEvent()
        {
            return false;
        }

        public virtual bool HandleMoveEvent()
        {
            return false;
        }

        public virtual bool HandleActivatedEvent()
        {
            return false;
        }

        public virtual bool HandleDeactivateEvent()
        {
            return false;
        }
    }
}