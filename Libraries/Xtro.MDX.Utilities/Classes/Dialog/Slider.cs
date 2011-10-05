using System;
using System.Drawing;
using System.Windows.Forms;

namespace Xtro.MDX.Utilities
{
    public sealed class Slider : Control
    {
        int Value;

        int Minimum;
        int Maximum;

        int DragX;      // Mouse position at start of drag
        int DragOffset; // Drag offset from the center of the button
        int ButtonX;

        bool Pressed;
        Rectangle ButtonRectangle;

        void SetValueInternal(int Value, bool FromInput)
        {
            // Clamp to range
            Value = Math.Max(Minimum, Value);
            Value = Math.Min(Maximum, Value);

            if (this.Value == Value) return;

            this.Value = Value;
            UpdateRectangles();

            Dialog.SendEvent(Event.SliderValueChanged, FromInput, this);
        }

        int ValueFromPosition(int X)
        {
            var ValuePerPixel = (float)(Maximum - Minimum) / BoundingBox.Width;
            return (int)(0.5f + Minimum + ValuePerPixel * (X - BoundingBox.Left));
        }

        public Slider(Dialog Dialog)
        {
            Type = ControlType.Slider;
            this.Dialog = Dialog;

            Maximum = 100;
            Value = 50;
        }

        public override bool ContainsPoint(Point Point)
        {
            return (BoundingBox.Contains(Point) || ButtonRectangle.Contains(Point));
        }

        public override bool CanHaveFocus()
        {
            return (Visible && Enabled);
        }

        public override bool HandleKeyDownEvent(KeyEventArgs E)
        {
            if (!Enabled || !Visible) return false;

            switch (E.KeyCode)
            {
            case Keys.Home:
                SetValueInternal(Minimum, true);
                return true;

            case Keys.End:
                SetValueInternal(Maximum, true);
                return true;

            case Keys.Left:
            case Keys.Down:
                SetValueInternal(Value - 1, true);
                return true;

            case Keys.Right:
            case Keys.Up:
                SetValueInternal(Value + 1, true);
                return true;

            case Keys.Next:
                SetValueInternal(Value - (10 > (Maximum - Minimum) / 10 ? 10 : (Maximum - Minimum) / 10),
                                  true);
                return true;

            case Keys.Prior:
                SetValueInternal(Value + (10 > (Maximum - Minimum) / 10 ? 10 : (Maximum - Minimum) / 10),
                                  true);
                return true;
            }

            return false;
        }

        public override bool HandleMouseDownAndDoubleClickEvent(MouseEventArgs E, Point Point)
        {
            if (!Enabled || !Visible) return false;

            if (E.Button == MouseButtons.Left)
            {
                if (ButtonRectangle.Contains(Point))
                {
                    // Pressed while inside the control
                    Pressed = true;

                    DragX = Point.X;
                    //m_nDragY = pt.y;
                    DragOffset = ButtonX - DragX;

                    //m_nDragValue = m_nValue;

                    if (!HasFocus) Dialog.RequestFocus(this);

                    return true;
                }

                if (BoundingBox.Contains(Point))
                {
                    DragX = Point.X;
                    DragOffset = 0;
                    Pressed = true;

                    if (!HasFocus)
                        Dialog.RequestFocus(this);

                    if (Point.X > ButtonX + X)
                    {
                        SetValueInternal(Value + 1, true);
                        return true;
                    }

                    if (Point.X < ButtonX + X)
                    {
                        SetValueInternal(Value - 1, true);
                        return true;
                    }
                }
            }

            return false;
        }

        public override bool HandleMouseUpEvent(MouseEventArgs E, Point Point)
        {
            if (!Enabled || !Visible) return false;

            if (E.Button == MouseButtons.Left)
            {
                if (Pressed)
                {
                    Pressed = false;
                    Dialog.SendEvent(Event.SliderValueChanged, true, this);

                    return true;
                }
            }

            return false;
        }

        public override bool HandleMouseMoveEvent(MouseEventArgs E, Point Point)
        {
            if (!Enabled || !Visible) return false;

            if (Pressed)
            {
                SetValueInternal(ValueFromPosition(X + Point.X + DragOffset), true);
                return true;
            }

            return false;
        }

        public override bool HandleMouseWheelEvent(MouseEventArgs E, Point Point)
        {
            if (!Enabled || !Visible) return false;

            SetValueInternal(Value - E.Delta, true);
            return true;
        }

        public override void UpdateRectangles()
        {
            base.UpdateRectangles();

            ButtonRectangle = BoundingBox;
            ButtonRectangle.Width = ButtonRectangle.Height;
            ButtonRectangle.Offset(-ButtonRectangle.Width / 2, 0);

            ButtonX = (int)((Value - Minimum) * (float)BoundingBox.Width / (Maximum - Minimum));
            ButtonRectangle.Offset(ButtonX, 0);
        }

        public override void Render(float ElapsedTime)
        {
            var State = ControlState.Normal;

            if (!Visible) State = ControlState.Hidden;
            else if (!Enabled) State = ControlState.Disabled;
            else if (Pressed) State = ControlState.Pressed;
            else if (MouseOver) State = ControlState.MouseOver;
            else if (HasFocus) State = ControlState.Focus;

            var BlendRate = (State == ControlState.Pressed) ? 0.0f : 0.8f;

            var Element = Elements[0];

            // Blend current color
            Element.TextureColor.Blend(State, ElapsedTime, BlendRate);
            Dialog.DrawSprite(Element, ref BoundingBox, FarButtonDepth);

            //xTODO: remove magic numbers
            Element = Elements[1];

            // Blend current color
            Element.TextureColor.Blend(State, ElapsedTime, BlendRate);
            Dialog.DrawSprite(Element, ref ButtonRectangle, NearButtonDepth);
        }

        public void SetValue(int Value)
        {
            SetValueInternal(Value, false);
        }

        public int GetValue()
        {
            return Value;
        }

        public void GetRange(out int Minimum, out int Maximum)
        {
            Minimum = this.Minimum; Maximum = this.Maximum;
        }

        public void SetRange(int Minimum, int Maximum)
        {
            this.Minimum = Minimum;
            this.Maximum = Maximum;

            SetValueInternal(Value, false);
        }
    }
}