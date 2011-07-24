using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Xtro.MDX.Direct3DX10;
using D3DX10Functions = Xtro.MDX.Direct3DX10.Functions;

namespace Xtro.MDX.Utilities
{
    public class ArcBall
    {
        Matrix Rotation;         // Matrix for arc ball's orientation
        Matrix Translation;      // Matrix for arc ball's position
        Matrix TranslationDelta; // Matrix for arc ball's position

        Point Offset;   // window offset, or upper-left corner of window
        int Width;   // arc ball's window width
        int Height;  // arc ball's window height
        //Vector2 Center;  // center of arc ball 
        float Radius;  // arc ball's radius in screen coords
        float RadiusTranslation; // arc ball's radius for translating the target

        Quaternion Down;             // Quaternion before button down
        Quaternion Now;              // Composite quaternion for current drag
        bool Drag;             // Whether user is dragging arc ball

        Point LastMouse;      // position of last mouse point
        Vector3 DownPoint;           // starting point of rotation arc
        Vector3 CurrentPoint;        // current point of rotation arc

        Vector3 ScreenToVector(float ScreenPointX, float ScreenPointY)
        {
            // Scale to screen
            // ReSharper disable PossibleLossOfFraction
            var X = -(ScreenPointX - Offset.X - Width / 2) / (Radius * Width / 2);
            var Y = (ScreenPointY - Offset.Y - Height / 2) / (Radius * Height / 2);
            // ReSharper restore PossibleLossOfFraction

            var Z = 0.0f;
            var Mag = X * X + Y * Y;

            if (Mag > 1.0f)
            {
                var Scale = (float)(1.0f / Math.Sqrt(Mag));
                X *= Scale;
                Y *= Scale;
            }
            else Z = (float)Math.Sqrt(1.0f - Mag);

            // Return vector
            return new Vector3(X, Y, Z);
        }

        public static Form DefaultForm;

        public ArcBall()
        {
            Reset();

            var ClientRectangle = DefaultForm.ClientRectangle;
            SetWindow(ClientRectangle.Right, ClientRectangle.Bottom);
        }

        public void Reset()
        {
            D3DX10Functions.QuaternionIdentity(out Down);
            D3DX10Functions.QuaternionIdentity(out Now);
            D3DX10Functions.MatrixIdentity(out Rotation);
            D3DX10Functions.MatrixIdentity(out Translation);
            D3DX10Functions.MatrixIdentity(out TranslationDelta);
            Drag = false;
            RadiusTranslation = 1.0f;
            Radius = 1.0f;
        }

        public void SetTranslationRadius(float RadiusTranslation)
        {
            this.RadiusTranslation = RadiusTranslation;
        }

        public void SetWindow(int Width, int Height, float Radius = 0.9f)
        {
            this.Width = Width;
            this.Height = Height;
            this.Radius = Radius;
            //Center = new Vector2(Width / 2.0f, Height / 2.0f);
        }

        public void SetOffset(int X, int Y)
        {
            Offset.X = X;
            Offset.Y = Y;
        }

        public void OnBegin(int X, int Y)
        {
            // Only enter the drag state if the click falls
            // inside the click rectangle.
            if (X >= Offset.X &&
                X < Offset.X + Width &&
                Y >= Offset.Y &&
                Y < Offset.Y + Height)
            {
                Drag = true;
                Down = Now;
                DownPoint = ScreenToVector(X, Y);
            }
        }

        public void OnMove(int X, int Y)
        {
            if (Drag)
            {
                CurrentPoint = ScreenToVector(X, Y);
                Now = Down * QuaternionFromBallPoints(ref DownPoint, ref CurrentPoint);
            }
        }

        public void OnEnd()
        {
            Drag = false;
        }

        public virtual void HandleMouseDownAndDoubleClickEvent(MouseEventArgs E)
        {
            // Update member var state
            if (E.Button == MouseButtons.Left) OnBegin(E.X, E.Y);
            if (E.Button == MouseButtons.Middle || E.Button == MouseButtons.Right)
            {
                LastMouse.X = E.X;
                LastMouse.Y = E.Y;
            }
        }

        public virtual void HandleMouseUpEvent(MouseEventArgs E)
        {
            // Update member var state
            if (E.Button == MouseButtons.Left) OnEnd();
        }

        public virtual void HandleMouseMove(MouseEventArgs E)
        {
            if (E.Button == MouseButtons.Left) OnMove(E.X, E.Y);
            else if (E.Button == MouseButtons.Middle || E.Button == MouseButtons.Right)
            {
                // Normalize based on size of window and bounding sphere radius
                var DeltaX = (LastMouse.X - E.X) * RadiusTranslation / Width;
                var DeltaY = (LastMouse.Y - E.Y) * RadiusTranslation / Height;

                if (E.Button == MouseButtons.Right)
                {
                    D3DX10Functions.MatrixTranslation(out TranslationDelta, -2 * DeltaX, 2 * DeltaY, 0.0f);
                    D3DX10Functions.MatrixMultiply(out Translation, ref Translation, ref TranslationDelta);
                }
                else  // E.Button == MouseButtons.Middle
                {
                    D3DX10Functions.MatrixTranslation(out TranslationDelta, 0.0f, 0.0f, 5 * DeltaY);
                    D3DX10Functions.MatrixMultiply(out Translation, ref Translation, ref TranslationDelta);
                }

                // Store mouse coordinate
                LastMouse.X = E.X;
                LastMouse.Y = E.Y;
            }
        }

        public Matrix GetRotationMatrix()
        {
            D3DX10Functions.MatrixRotationQuaternion(out Rotation, ref Now);
            return Rotation;
        }

        public Matrix GetTranslationMatrix()
        {
            return Translation;
        }

        public Matrix GetTranslationDeltaMatrix()
        {
            return TranslationDelta;
        }

        public bool IsBeingDragged()
        {
            return Drag;
        }

        public Quaternion GetQuaternionNow()
        {
            return Now;
        }

        public void SetQuaternionNow(ref Quaternion Q)
        {
            Now = Q;
        }

        public static Quaternion QuaternionFromBallPoints(ref Vector3 From, ref Vector3 To)
        {
            Vector3 Part;
            var Dot = D3DX10Functions.Vector3Dot(ref From, ref To);
            D3DX10Functions.Vector3Cross(out Part, ref From, ref To);

            return new Quaternion(Part.X, Part.Y, Part.Z, Dot);
        }
    }
}