using System.Drawing;
using System.Windows.Forms;

namespace Xtro.MDX.Utilities
{
    public class Button : Static
    {
        protected bool Pressed;

        public Button(Dialog Dialog)
        {
            Type = ControlType.Button;
            this.Dialog = Dialog;
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
                    Dialog.SendEvent(Event.ButtonClicked, true, this);
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

                    if (!Dialog.KeyboardInput) Dialog.ClearFocus();

                    // Button click
                    if (ContainsPoint(Point)) Dialog.SendEvent(Event.ButtonClicked, true, this);

                    return true;
                }
            }

            return false;
        }

        public override bool ContainsPoint(Point Point)
        {
            return BoundingBox.Contains(Point);
        }

        public override bool CanHaveFocus()
        {
            return (Visible && Enabled);
        }

        public override void Render(float ElapsedTime)
        {
            var OffsetX = 0;
            var OffsetY = 0;

            var State = ControlState.Normal;

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

            // Background fill layer
            //xTODO: remove magic numbers
            var Element = Elements[0];

            var BlendRate = (State == ControlState.Pressed) ? 0.0f : 0.8f;

            var WindowRectangle = BoundingBox;
            WindowRectangle.Offset(OffsetX, OffsetY);


            // Blend current color
            Element.TextureColor.Blend(State, ElapsedTime, BlendRate);
            Element.FontColor.Blend(State, ElapsedTime, BlendRate);

            Dialog.DrawSprite(Element, ref WindowRectangle, FarButtonDepth);
            Dialog.DrawText(Text, Element, ref WindowRectangle);

            // Main button
            Element = Elements[1];

            // Blend current color
            Element.TextureColor.Blend(State, ElapsedTime, BlendRate);
            Element.FontColor.Blend(State, ElapsedTime, BlendRate);

            Dialog.DrawSprite(Element, ref WindowRectangle, NearButtonDepth);
            Dialog.DrawText(Text, Element, ref WindowRectangle);
        }

        public override void OnHotkey()
        {
            if (Dialog.IsKeyboardInputEnabled()) Dialog.RequestFocus(this);
            Dialog.SendEvent(Event.ButtonClicked, true, this);
        }
    }
}