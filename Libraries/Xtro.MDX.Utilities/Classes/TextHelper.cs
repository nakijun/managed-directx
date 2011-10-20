using System.Drawing;
using Point = System.Drawing.Point;
using Xtro.MDX.Direct3D10;
using Xtro.MDX.Direct3DX10;
using Color = Xtro.MDX.Direct3DX10.Color;
using Constants = Xtro.MDX.Direct3D10.Constants;
using D3DX10Functions = Xtro.MDX.Direct3DX10.Functions;
using Font = Xtro.MDX.Direct3DX10.Font;

namespace Xtro.MDX.Utilities
{
    public sealed class TextHelper
    {
        Font Font;
        Sprite Sprite;
        Color Color;
        Point Point;
        int LineHeight;
        BlendState FontBlendState;

        public TextHelper(Font Font = null, Sprite Sprite = null, int LineHeight = 15)
        {
            Init(Font, Sprite, LineHeight);
        }

        public void Delete()
        {
            if (FontBlendState != null)
            {
                FontBlendState.Release();
                FontBlendState = null;
            }
        }

        public void Init(Font Font = null, Sprite Sprite = null, int LineHeight = 15)
        {
            this.Font = Font;
            this.Sprite = Sprite;
            Color = new Color(1, 1, 1, 1);
            Point = new Point(0, 0);
            this.LineHeight = LineHeight;
            FontBlendState = null;

            // Create a blend state if a sprite is passed in
            if (Sprite != null)
            {
                Device Device;
                Sprite.GetDevice(out Device);
                if (Device != null)
                {
                    var StateDescription = new BlendDescription
                    {
                        AlphaToCoverageEnable = false,
                        SourceBlend = Blend.SourceAlpha,
                        DestinationBlend = Blend.InvertSourceAlpha,
                        BlendOperation = BlendOperation.Add,
                        SourceBlendAlpha = Blend.Zero,
                        DestinationBlendAlpha = Blend.Zero,
                        BlendOperationAlpha = BlendOperation.Add
                    };
                    StateDescription.RenderTargetWriteMask[0] = 0xf;
                    StateDescription.BlendEnable[0] = true;
                    Device.CreateBlendState(ref StateDescription, out FontBlendState);

                    Device.Release();
                }
            }
        }

        public void SetInsertionPosition(int X, int Y)
        {
            Point.X = X; Point.Y = Y;
        }

        public void SetForegroundColor(Color Color)
        {
            this.Color = Color;
        }

        public void Begin()
        {
            if (Sprite != null)
            {
                var Viewports = new Viewport[Constants.ViewportAndScissorRectangleObjectCountPerPipeline];
                uint ViewportCount = 1;
                Device Device;
                Sprite.GetDevice(out Device);
                if (Device != null)
                {
                    // Set projection
                    Device.RS_GetViewports(ref ViewportCount, Viewports);
                    Matrix ProjectionMatrix;
                    D3DX10Functions.MatrixOrthoOffCenterLH(out ProjectionMatrix, Viewports[0].TopLeftX, Viewports[0].TopLeftX + Viewports[0].Width, Viewports[0].TopLeftY, Viewports[0].TopLeftY + Viewports[0].Height, 0.1f, 10);
                    Sprite.SetProjectionTransform(ref ProjectionMatrix);

                    Sprite.Begin(SpriteFlag.SortTexture);
                    Device.Release();
                }
            }
        }

        public void End()
        {
            if (Sprite != null)
            {
                var OriginalBlendFactor = new Float4();
                uint OriginalSampleMask = 0;
                BlendState OriginalBlendState = null;
                Device Device;

                Sprite.GetDevice(out Device);
                if (Device != null)
                {
                    // Get the old blend state and set the new one
                    Device.OM_GetBlendState(out OriginalBlendState, out OriginalBlendFactor, out OriginalSampleMask);
                    if (FontBlendState != null)
                    {
                        var NewBlendFactor = new Float4();
                        Device.OM_SetBlendState(FontBlendState, ref NewBlendFactor, 0xffffffff);
                    }
                }

                Sprite.End();

                // Reset the original blend state
                if (Device != null && OriginalBlendState != null) Device.OM_SetBlendState(OriginalBlendState, ref OriginalBlendFactor, OriginalSampleMask);
                if (OriginalBlendState != null) OriginalBlendState.Release();
                if (Device != null) Device.Release();
            }
        }
        public int DrawFormattedTextLine(string Message, params object[] Arguments)
        {
            return DrawTextLine(string.Format(Message, Arguments));
        }

        public int DrawTextLine(string Message)
        {
            if (Font == null) return Functions.ErrorBox((int)Error.InvalidArgument, "DrawTextLine");

            var Rectangle = new Rectangle(Point.X, Point.Y, -Point.X, -Point.Y);

            var Result = Font.DrawText(Sprite, Message, -1, ref Rectangle, FontDrawFlag.NoClip, ref Color);
            if (Result < 0) return Functions.ErrorBox(Result, "DrawText");

            Point.Y += LineHeight;

            return 0;
        }

        public int DrawFormattedTextLine(ref Rectangle Rectangle, FontDrawFlag Flags, string Message, params object[] Arguments)
        {
            return DrawTextLine(ref Rectangle, Flags, string.Format(Message, Arguments));
        }

        public int DrawTextLine(ref Rectangle Rectangle, FontDrawFlag Flags, string Message)
        {
            if (Font == null) return Functions.ErrorBox((int)Error.InvalidArgument, "DrawTextLine");

            var Result = Font.DrawText(Sprite, Message, -1, ref Rectangle, Flags, ref Color);
            if (Result < 0) return Functions.ErrorBox(Result, "DrawText");

            Point.Y += LineHeight;

            return 0;
        }
    }
}