using System.Drawing;

namespace Xtro.MDX.Utilities
{
    public class Static : Control
    {
        public string Text;

        public Static(Dialog Dialog = null)
        {
            Type = ControlType.Static;
            this.Dialog = Dialog;

            Elements.Clear();
        }

        public override void Render(float ElapsedTime)
        {
            if (!Visible) return;

            var S = ControlState.Normal;

            if (!Enabled) S = ControlState.Disabled;

            var Element = Elements[0];

            Element.FontColor.Blend(S, ElapsedTime);

            Dialog.DrawText(Text, Element, ref BoundingBox, true);
        }

        public override bool ContainsPoint(Point Point)
        {
            return false;
        }
    }
}