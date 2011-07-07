using System;
using System.Drawing;
using System.Windows.Forms;

namespace Xtro.MDX.Utilities.Classes
{
    public class ScrollBar : Control
    {
        // Minimum scroll bar thumb size
        protected const int ScrollBarMinimumThumbSize = 8;

        // Delay and repeat period when clicking on the scroll bar arrows
        protected const float ScrollBarArrowClickDelay = 0.33f;
        protected const float ScrollBarArrowClickRepeat = 0.05f;

        // ARROWSTATE indicates the state of the arrow buttons.
        // CLEAR            No arrow is down.
        // CLICKED_UP       Up arrow is clicked.
        // CLICKED_DOWN     Down arrow is clicked.
        // HELD_UP          Up arrow is held down for sustained period.
        // HELD_DOWN        Down arrow is held down for sustained period.
        protected enum ArrowState
        {
            Clear,
            ClickedUp,
            ClickedDown,
            HeldUp,
            HeldDown
        };

        protected void UpdateThumbRectangle()
        {
            if (End - Start > PageSize)
            {
                var ThumbHeight = Math.Max(TrackRectangle.Height * PageSize / (End - Start), ScrollBarMinimumThumbSize);
                var MaxPosition = End - Start - PageSize;
                ThumbRectangle.Y = TrackRectangle.Y + (Position - Start) * (TrackRectangle.Height - ThumbHeight) / MaxPosition;
                ThumbRectangle.Height = ThumbHeight;
                ShowThumb = true;
            }
            else
            {
                // No content to scroll
                ThumbRectangle.Height = 0;
                ShowThumb = false;
            }
        }

        protected void Cap()  // Clips position at boundaries. Ensures it stays within legal range.
        {
            if (Position < Start || End - Start <= PageSize) Position = Start;
            else if (Position + PageSize > End) Position = End - PageSize;
        }

        protected bool ShowThumb;
        protected bool Drag;
        protected Rectangle UpButtonRectangle;
        protected Rectangle DownButtonRectangle;
        protected Rectangle TrackRectangle;
        protected Rectangle ThumbRectangle;
        protected int Position;  // Position of the first displayed item
        protected int PageSize;  // How many items are displayable in one page
        protected int Start;     // First item
        protected int End;       // The index after the last item
        protected Point LastMouse;// Last mouse position
        protected ArrowState Arrow; // State of the arrows
        protected double ArrowTimeStamp;  // Timestamp of last arrow event.

        public ScrollBar(Dialog Dialog)
            : base(Dialog)
        {
            Type = ControlType.ScrollBar;

            ShowThumb = true;
            PageSize = 1;
            End = 1;
            Arrow = ArrowState.Clear;
        }

        int ThumbOffsetY;

        public override bool HandleMouseDownAndDoubleClickEvent(MouseEventArgs E, Point Point)
        {
            LastMouse = Point;

            if (E.Button == MouseButtons.Left)
            {
                // Check for click on up button

                if (UpButtonRectangle.Contains(Point))
                {
                    if (Position > Start) Position--;
                    UpdateThumbRectangle();
                    Arrow = ArrowState.ClickedUp;
                    ArrowTimeStamp = Functions.GetTime();
                    return true;
                }

                // Check for click on down button

                if (DownButtonRectangle.Contains(Point))
                {
                    if (Position + PageSize < End) Position++;
                    UpdateThumbRectangle();
                    Arrow = ArrowState.ClickedDown;
                    ArrowTimeStamp = Functions.GetTime();
                    return true;
                }

                // Check for click on thumb

                if (ThumbRectangle.Contains(Point))
                {
                    Drag = true;
                    ThumbOffsetY = Point.Y - ThumbRectangle.Top;
                    return true;
                }

                // Check for click on track

                if (ThumbRectangle.Left <= Point.X && ThumbRectangle.Right > Point.X)
                {
                    if (ThumbRectangle.Top > Point.Y && TrackRectangle.Top <= Point.Y)
                    {
                        Scroll(-(PageSize - 1));
                        return true;
                    }

                    if (ThumbRectangle.Bottom <= Point.Y && TrackRectangle.Bottom > Point.Y)
                    {
                        Scroll(PageSize - 1);
                        return true;
                    }
                }
            }

            return false;
        }

        public override bool HandleMouseUpEvent(MouseEventArgs E, Point Point)
        {
            LastMouse = Point;

            if (E.Button == MouseButtons.Left)
            {
                Drag = false;
                UpdateThumbRectangle();
                Arrow = ArrowState.Clear;
            }

            return false;
        }

        public override bool HandleMouseMove(MouseEventArgs E)
        {
            var Point = E.Location;

            LastMouse = Point;

            if (Drag)
            {
                ThumbRectangle.Height += Point.Y - ThumbOffsetY - ThumbRectangle.Top;
                ThumbRectangle.Y = Point.Y - ThumbOffsetY;
                if (ThumbRectangle.Top < TrackRectangle.Top) ThumbRectangle.Offset(0, TrackRectangle.Top - ThumbRectangle.Top);
                else if (ThumbRectangle.Bottom > TrackRectangle.Bottom) ThumbRectangle.Offset(0, TrackRectangle.Bottom - ThumbRectangle.Bottom);

                // Compute first item index based on thumb position

                var MaxFirstItem = End - Start - PageSize;  // Largest possible index for first item
                var MaxThumb = TrackRectangle.Height - ThumbRectangle.Height;  // Largest possible thumb position from the top

                Position = Start + (ThumbRectangle.Top - TrackRectangle.Top + MaxThumb / (MaxFirstItem * 2)) * // Shift by half a row to avoid last row covered by only one pixel
                    MaxFirstItem / MaxThumb;

                return true;
            }

            return false;
        }

        public override void Render(float ElapsedTime)
        {
            // Check if the arrow button has been held for a while.
            // If so, update the thumb position to simulate repeated
            // scroll.
            if (Arrow != ArrowState.Clear)
            {
                var CurrentTime = Functions.GetTime();
                if (UpButtonRectangle.Contains(LastMouse))
                {
                    switch (Arrow)
                    {
                    case ArrowState.ClickedUp:
                        if (ScrollBarArrowClickDelay < CurrentTime - ArrowTimeStamp)
                        {
                            Scroll(-1);
                            Arrow = ArrowState.HeldUp;
                            ArrowTimeStamp = CurrentTime;
                        }
                        break;
                    case ArrowState.HeldUp:
                        if (ScrollBarArrowClickRepeat < CurrentTime - ArrowTimeStamp)
                        {
                            Scroll(-1);
                            ArrowTimeStamp = CurrentTime;
                        }
                        break;
                    }
                }
                else if (DownButtonRectangle.Contains(LastMouse))
                {
                    switch (Arrow)
                    {
                    case ArrowState.ClickedDown:
                        if (ScrollBarArrowClickDelay < CurrentTime - ArrowTimeStamp)
                        {
                            Scroll(1);
                            Arrow = ArrowState.HeldDown;
                            ArrowTimeStamp = CurrentTime;
                        }
                        break;
                    case ArrowState.HeldDown:
                        if (ScrollBarArrowClickRepeat < CurrentTime - ArrowTimeStamp)
                        {
                            Scroll(1);
                            ArrowTimeStamp = CurrentTime;
                        }
                        break;
                    }
                }
            }

            var State = ControlState.Normal;

            if (!Visible) State = ControlState.Hidden;
            else if (!Enabled || !ShowThumb) State = ControlState.Disabled;
            else if (MouseOver) State = ControlState.MouseOver;
            else if (HasFocus) State = ControlState.Focus;

            var BlendRate = (State == ControlState.Pressed) ? 0.0f : 0.8f;

            // Background track layer
            var Element = Elements[0];

            // Blend current color
            Element.TextureColor.Blend(State, ElapsedTime, BlendRate);
            Dialog.DrawSprite(Element, ref TrackRectangle, FarButtonDepth);

            // Up Arrow
            Element = Elements[1];

            // Blend current color
            Element.TextureColor.Blend(State, ElapsedTime, BlendRate);
            Dialog.DrawSprite(Element, ref UpButtonRectangle, NearButtonDepth);

            // Down Arrow
            Element = Elements[2];

            // Blend current color
            Element.TextureColor.Blend(State, ElapsedTime, BlendRate);
            Dialog.DrawSprite(Element, ref DownButtonRectangle, NearButtonDepth);

            // Thumb button
            Element = Elements[3];

            // Blend current color
            Element.TextureColor.Blend(State, ElapsedTime, BlendRate);
            Dialog.DrawSprite(Element, ref ThumbRectangle, NearButtonDepth);
        }

        public new void UpdateRectangles()
        {
            base.UpdateRectangles();

            // Make the buttons square

            UpButtonRectangle = new Rectangle(BoundingBox.X, BoundingBox.Y, BoundingBox.Width, BoundingBox.Width);
            DownButtonRectangle = new Rectangle(BoundingBox.X, BoundingBox.Bottom - BoundingBox.Width, BoundingBox.Width, BoundingBox.Width);
            TrackRectangle = new Rectangle(UpButtonRectangle.X, UpButtonRectangle.Bottom, UpButtonRectangle.Width, DownButtonRectangle.Top - UpButtonRectangle.Bottom);
            ThumbRectangle.X = UpButtonRectangle.X;
            ThumbRectangle.Width = UpButtonRectangle.Width;

            UpdateThumbRectangle();
        }

        public void SetTrackRange(int Start, int End)
        {
            this.Start = Start;
            this.End = End;
            Cap();
            UpdateThumbRectangle();
        }

        public int GetTrackPosition()
        {
            return Position;
        }

        public void SetTrackPosition(int Position)
        {
            this.Position = Position;
            Cap();
            UpdateThumbRectangle();
        }

        public int GetPageSize()
        {
            return PageSize;
        }

        public void SetPageSize(int PageSize)
        {
            this.PageSize = PageSize;
            Cap();
            UpdateThumbRectangle();
        }

        // Scroll() scrolls by Delta items.  A positive value scrolls down, while a negative
        // value scrolls up.
        public void Scroll(int Delta)
        {
            // Perform scroll
            Position += Delta;

            // Cap position
            Cap();

            // Update thumb position
            UpdateThumbRectangle();
        }

        public void ShowItem(int Index)
        {
            // Cap the index

            if (Index < 0) Index = 0;

            if (Index >= End) Index = End - 1;

            // Adjust position

            if (Position > Index) Position = Index;
            else if (Position + PageSize <= Index) Position = Index - PageSize + 1;

            UpdateThumbRectangle();
        }
    }
}