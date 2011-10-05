using System.Drawing;
using System.Windows.Forms;

namespace Xtro.MDX.Utilities
{
    public sealed class RadioButton : CheckBox
    {
        public uint ButtonGroup;

        void SetCheckedInternal(bool Checked, bool ClearGroup, bool FromInput)
        {
            if (Checked && ClearGroup) Dialog.ClearRadioButtonGroup(ButtonGroup);

            FChecked = Checked;
            Dialog.SendEvent(Event.RadioButtonChanged, FromInput, this);
        }

        public RadioButton(Dialog Dialog)
            : base(Dialog)
        {
            Type = ControlType.RadioButton;
        }

        public void SetChecked(bool Checked, bool ClearGroup = true)
        {
            SetCheckedInternal(Checked, ClearGroup, false);
        }

        public override void OnHotkey()
        {
            if (Dialog.IsKeyboardInputEnabled()) Dialog.RequestFocus(this);
            SetCheckedInternal(true, true, true);
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

                    Dialog.ClearRadioButtonGroup(ButtonGroup);
                    FChecked = !FChecked;

                    Dialog.SendEvent(Event.RadioButtonChanged, true, this);
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
                    if (ContainsPoint(Point))
                    {
                        Dialog.ClearRadioButtonGroup(ButtonGroup);
                        FChecked = !FChecked;

                        Dialog.SendEvent(Event.RadioButtonChanged, true, this);
                    }

                    return true;
                }
            }

            return false;
        }
    }
}