using System.Drawing;
using System.Windows.Forms;

namespace Xtro.MDX.Utilities
{
    public class CheckBox : Button
    {
        protected bool FChecked;
        protected Rectangle ButtonRectangle;
        protected Rectangle TextRectangle;

        public bool Checked { get { return FChecked; } set { SetCheckedInternal(value, false); } }

        protected void SetCheckedInternal(bool Checked, bool FromInput)
        {
            FChecked = Checked;

            Dialog.SendEvent(Event.CheckBoxChanged, FromInput, this);
        }

        public CheckBox(Dialog Dialog)
            : base(Dialog)
        {
            Type = ControlType.CheckBox;
        }

        public override bool HandleKeyDownEvent(KeyEventArgs E)
        {
            if (!Enabled || !Visible) return false;

            if (E.KeyCode == Keys.Space)
            {
                Pressed = true;
                return true;
            }
            return false;
        }

        public override bool HandleKeyUpEvent(KeyEventArgs E)
        {
            if (!Enabled || !Visible) return false;

            if (E.KeyCode == Keys.Space)
            {
                if (Pressed)
                {
                    Pressed = false;
                    SetCheckedInternal(!FChecked, true);
                }
                return true;
            }

            return false;
        }

        public override bool HandleMouseDownAndDoubleClickEvent(MouseEventArgs E, Point Point)
        {
            if (!Enabled || !Visible) return false;

            if (E.Button == MouseButtons.Left)
            {
                if (ContainsPoint(Point))
                {
                    // Pressed while inside the control
                    Pressed = true;

                    if (!HasFocus) Dialog.RequestFocus(this);

                    return true;
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

                    // Button click
                    if (ContainsPoint(Point)) SetCheckedInternal(!FChecked, true);

                    return true;
                }
            }

            return false;
        }

        public override void OnHotkey()
        {
            if (Dialog.IsKeyboardInputEnabled()) Dialog.RequestFocus(this);
            SetCheckedInternal(FChecked, true);
        }

        public override bool ContainsPoint(Point Point)
        {
            return BoundingBox.Contains(Point) || ButtonRectangle.Contains(Point);
        }

        public override void UpdateRectangles()
        {
            base.UpdateRectangles();

            ButtonRectangle = BoundingBox;
            ButtonRectangle.Width = ButtonRectangle.Height; // not a bug. height to width

            TextRectangle = BoundingBox;
            TextRectangle.X += (int)(1.25f * ButtonRectangle.Width);
        }

        public override void Render(float ElapsedTime)
        {
            var State = ControlState.Normal;

            if (!Visible) State = ControlState.Hidden;
            else if (!Enabled) State = ControlState.Disabled;
            else if (Pressed) State = ControlState.Pressed;
            else if (MouseOver) State = ControlState.MouseOver;
            else if (HasFocus) State = ControlState.Focus;

            var Element = Elements[0];

            var BlendRate = (State == ControlState.Pressed) ? 0.0f : 0.8f;

            Element.TextureColor.Blend(State, ElapsedTime, BlendRate);
            Element.FontColor.Blend(State, ElapsedTime, BlendRate);

            Dialog.DrawSprite(Element, ref ButtonRectangle, NearButtonDepth);
            Dialog.DrawText(Text, Element, ref TextRectangle, true);

            if (!FChecked) State = ControlState.Hidden;

            Element = Elements[1];

            Element.TextureColor.Blend(State, ElapsedTime, BlendRate);
            Dialog.DrawSprite(Element, ref ButtonRectangle, FarButtonDepth);
        }
    }
}