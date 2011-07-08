using System;
using System.Drawing;

namespace Xtro.MDX.Utilities
{
    public class Slider : Control
    {
        protected int Value;

        protected int Minimum;
        protected int Maximum;

        protected int DragX;      // Mouse position at start of drag
        protected int DragOffset; // Drag offset from the center of the button
        protected int ButtonX;

        protected bool Pressed;
        protected Rectangle ButtonRectangle;

        protected void SetValueInternal(int Value, bool FromInput)
        {
            // Clamp to range
            Value = Math.Max(Minimum, Value);
            Value = Math.Min(Maximum, Value);

            if (this.Value == Value) return;

            this.Value = Value;
            UpdateRectangles();

            Dialog.SendEvent(Event.SliderValueChanged, FromInput, this);
        }

        protected int ValueFromPosition(int X)
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
    }
}