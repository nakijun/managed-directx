using System;
using System.Diagnostics;
using System.Drawing;

namespace Xtro.MDX.Utilities
{
    public class EditBox : Control
    {
        protected UniBuffer Buffer;     // Buffer to hold text
        protected int Border;      // Border of the window
        protected int Spacing;     // Spacing between the text and the edge of border
        protected Rectangle TextRectangle;       // Bounding rectangle for the text
        protected Rectangle[] RenderRectangle = new Rectangle[9];  // Convenient rectangles for rendering elements
        protected double Blink;      // Caret blink time in milliseconds
        protected double LastBlink;  // Last timestamp of caret blink
        protected bool CaretOn;     // Flag to indicate whether caret is currently visible
        protected int Caret;       // Caret position, in characters
        protected bool InsertMode;  // If true, control is in insert mode. Else, overwrite mode.
        protected int SelectionStart;    // Starting position of the selection. The caret marks the end.
        protected int FirstVisible;// First visible character in the edit control
        protected Color TextColor;    // Text color
        protected Color SelectedTextColor; // Selected text color
        protected Color SelectedBackColor;   // Selected background color
        protected Color CaretColor;   // Caret color

        // Mouse-specific
        protected bool MouseDrag;       // True to indicate drag in progress

        // Static
        protected static bool HideCaret;   // If true, we don't render the caret.

        // PlaceCaret: Set the caret to a character position, and adjust the scrolling if
        //             necessary.
        protected void PlaceCaret(int CharacterPosition)
        {
            Debug.Assert(CharacterPosition >= 0 && CharacterPosition <= Buffer.GetTextSize());
            Caret = CharacterPosition;

            // Obtain the X offset of the character.
            int X1St, X1, X2;
            Buffer.CharacterPositionToX(FirstVisible, false, out X1St);  // 1st visible char
            Buffer.CharacterPositionToX(CharacterPosition, false, out X1);  // LEAD
            // If nCP is the NULL terminator, get the leading edge instead of trailing.
            if (CharacterPosition == Buffer.GetTextSize()) X2 = X1;
            else Buffer.CharacterPositionToX(CharacterPosition, true, out X2);  // TRAIL

            // If the left edge of the char is smaller than the left edge of the 1st visible char,
            // we need to scroll left until this char is visible.
            if (X1 < X1St)
            {
                // Simply make the first visible character the char at the new caret position.
                FirstVisible = CharacterPosition;
            }
            // If the right of the character is bigger than the offset of the control's right edge, we need to scroll right to this character.
            else if (X2 > X1St + TextRectangle.Width)
            {
                // Compute the X of the new left-most pixel
                var XNewLeft = X2 - TextRectangle.Width;

                // Compute the char position of this character
                int CarretPositionNew1St, NewTrail;
                Buffer.XtoCharacterPosition(XNewLeft, out CarretPositionNew1St, out NewTrail);

                // If this coordinate is not on a character border,
                // start from the next character so that the caret
                // position does not fall outside the text rectangle.
                int XNew1St;
                Buffer.CharacterPositionToX(CarretPositionNew1St, false, out XNew1St);
                if (XNew1St < XNewLeft) CarretPositionNew1St++;

                FirstVisible = CarretPositionNew1St;
            }
        }

        public void DeleteSelectionText()
        {
            var First = Math.Min(Caret, SelectionStart);
            var Last = Math.Max(Caret, SelectionStart);
            // Update caret and selection
            PlaceCaret(First);
            SelectionStart = Caret;
            // Remove the characters
            for (var I = First; I < Last; I++) Buffer.RemoveChar(First);
        }
    }
}