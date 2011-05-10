using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Xtro.MDX.Direct3D10;
using Xtro.MDX.Direct3DX10;
using Color = Xtro.MDX.Direct3DX10.Color;
using D3DX10Functions = Xtro.MDX.Direct3DX10.Functions;

namespace Xtro.MDX.Utilities
{
    public class Dialog
    {
        struct ElementHolder
        {
            uint ControlType;
            uint ElementNo;

            Element Element;
        };

        internal struct ScreenVertex
        {
            public float X, Y, Z;
            public Color Color;
            // ReSharper disable InconsistentNaming
            public float TU, TV;
            // ReSharper restore InconsistentNaming
        };

        int DefaultControlID;

        static double TimeRefresh;
        double TimeLastRefresh;

        static Control ControlFocus;        // The control which has focus

        static Control ControlPressed;      // The control currently pressed

        Control ControlMouseOver;           // The control which is hovered over

        bool Visible;
        bool ShowCaption;
        bool Minimized;
        bool Drag;
        string Caption;

        int X;
        int Y;
        int Width;
        int Height;
        int CaptionHeight;

        uint ColorTopLeft;
        uint ColorTopRight;
        uint ColorBottomLeft;
        uint ColorBottomRight;

        DialogResourceManager Manager;
        Callbacks.GuiEvent CallbackEvent;
        object CallbackEventUserContext;

        List<int> Textures = new List<int>();   // Index into m_TextureCache;
        List<int> Fonts = new List<int>();      // Index into m_FontCache;

        List<Control> Controls;
        List<ElementHolder> DefaultElements;

        Element CaptionElement;  // Element for the caption

        Dialog NextDialog;
        Dialog PrevDialog;

        public bool NonUserEvents;
        public bool KeyboardInput;
        public bool MouseInput;

        int OnRender(float ElapsedTime)
        {
            // See if the dialog needs to be refreshed
            if (TimeLastRefresh < TimeRefresh)
            {
                TimeLastRefresh = Functions.GetTime();
                Refresh();
            }

            // For invisible dialog, out now.
            if (!Visible || (Minimized && string.IsNullOrEmpty(Caption))) return 0;

            var Device = Manager.GetDevice();

            // Set up a state block here and restore it when finished drawing all the controls
            Manager.StateBlock.Capture();

            var BackgroundIsVisible = ((ColorTopLeft | ColorTopRight | ColorBottomRight | ColorBottomLeft) & 0xff000000) > 0;
            if (!Minimized && BackgroundIsVisible)
            {
                // Convert the draw rectangle from screen coordinates to clip space coordinates.
                var Left = X * 2.0f / Manager.BackBufferWidth - 1.0f;
                var Right = (X + Width) * 2.0f / Manager.BackBufferWidth - 1.0f;
                var Top = 1.0f - Y * 2.0f / Manager.BackBufferHeight;
                var Bottom = 1.0f - (Y + Height) * 2.0f / Manager.BackBufferHeight;

                var Vertices = new[]
                    {
                        new ScreenVertex{X= Left,Y=  Top,    Z=0.5f,Color=new Color( ColorTopLeft) ,TU= 0.0f,TV= 0.0f},
                        new ScreenVertex{X= Right, Y=Top,   Z= 0.5f, Color=new Color( ColorTopRight ), TU=1.0f, TV=0.0f},
                        new ScreenVertex{X= Left,  Y=Bottom, Z=0.5f, Color=new Color( ColorBottomLeft ), TU=0.0f, TV=1.0f},
                        new ScreenVertex{X= Right, Y=Bottom, Z=0.5f, Color=new Color( ColorBottomRight ), TU=1.0f, TV=1.0f}
                    };

                UnmanagedMemory VertexBuffer;
                if ((Manager.VertexBufferScreenQuad.Map(Map.WriteDiscard, 0, out VertexBuffer)) >= 0)
                {
                    VertexBuffer.Write(0, Vertices);
                    Manager.VertexBufferScreenQuad.Unmap();
                }

                // Set the quad VB as current
                var Stride = (uint)Marshal.SizeOf(typeof(ScreenVertex));
                const uint Offset = 0;
                Device.IA_SetVertexBuffers(0, 1, new[] { Manager.VertexBufferScreenQuad }, new[] { Stride }, new[] { Offset });
                Device.IA_SetInputLayout(Manager.InputLayout);
                Device.IA_SetPrimitiveTopology(PrimitiveTopology.TriangleStrip);

                // Draw dialog background
                TechniqueDescription TechniqueDescription;
                Manager.TechRenderUI_Untex.GetDescription(out TechniqueDescription);
                for (uint P = 0; P < TechniqueDescription.Passes; P++)
                {
                    Manager.TechRenderUI_Untex.GetPassByIndex(P).Apply(0);
                    Device.Draw(4, 0);
                }
            }

            var TextureNode = GetTexture(0);
            Manager.FxTexture.SetResource(TextureNode[0].TextureResourceView);

            // Sort depth back to front
            Manager.Sprite.Begin(0);

            Manager.TechRenderUI.GetPassByIndex(0).Apply(0);

            // Render the caption if it's enabled.
            if (!string.IsNullOrEmpty(Caption))
            {
                // DrawSprite will offset the rect down by
                // m_nCaptionHeight, so adjust the rect higher
                // here to negate the effect.
                var Rectangle = new Rectangle(0, -CaptionHeight, Width, 0);
                DrawSprite(CaptionElement, ref Rectangle, 0.99f);
                Rectangle.X += 5; // Make a left margin
                var Output = Caption;
                if (Minimized) Output += " (Minimized)";
                DrawText(Output, CaptionElement, ref Rectangle, true);
            }

            // If the dialog is minimized, skip rendering
            // its controls.
            if (!Minimized)
            {
                foreach (var Control in Controls)
                {
                    // Focused control is drawn last
                    if (Control == ControlFocus) continue;

                    Control.Render(ElapsedTime);
                }

                if (ControlFocus != null && ControlFocus.Dialog == this) ControlFocus.Render(ElapsedTime);
            }

            Manager.Sprite.End();

            Manager.StateBlock.Apply();
            // Restore depth stencil state
            Manager.Effect.GetTechniqueByName("RestoreState").GetPassByIndex(0).Apply(0);

            return 0;
        }

        public DialogResourceManager.FontNode[] GetFont(uint Index)
        {
            return Manager == null ? null : Manager.GetFontNode(Fonts[(int)Index]);
        }

        public int DrawSprite(Element Element, ref Rectangle DestinationRectangle, float Depth)
        {
            // No need to draw fully transparent layers
            if (Element.TextureColor.Current.A == 0) return 0;

            var TextureRectangle = Element.TextureRectangle;

            var Screen = DestinationRectangle;
            Screen.Offset(X, Y);
            Screen.Offset(Screen.Width / 2, Screen.Height / 2);

            // If caption is enabled, offset the Y position by its height.
            if (!string.IsNullOrEmpty(Caption)) Screen.Offset(0, CaptionHeight);

            var TextureNode = GetTexture(Element.Texture);
            if (TextureNode == null) return (int)Error.Fail;

            var BackBufferWidth = (float)Manager.BackBufferWidth;
            var BackBufferHeight = (float)Manager.BackBufferHeight;
            var ScaleX = (float)Screen.Width;
            var ScaleY = (float)Screen.Height;

            var Position = new Vector3(Screen.Left, Screen.Top, Depth);

            Matrix MatrixScaling;
            Matrix MatrixTranslation;
            D3DX10Functions.MatrixScaling(out MatrixScaling, ScaleX, ScaleY, 1.0f);
            D3DX10Functions.MatrixTranslation(out MatrixTranslation, Position.X, BackBufferHeight - Position.Y, Position.Z);

            Matrix MatrixProjection;
            D3DX10Functions.MatrixOrthoOffCenterLH(out MatrixProjection, 0.0f, BackBufferWidth, 0.0f, BackBufferHeight, 0.1f, 10);
            Manager.Sprite.SetProjectionTransform(ref MatrixProjection);

            SpriteStruct Sprite;

            Sprite.MatrixWorld = MatrixScaling * MatrixTranslation;
            Sprite.Texture = TextureNode[0].TextureResourceView;
            var TextureWidth = (float)TextureNode[0].Width;
            var TextureHeight = (float)TextureNode[0].Height;
            Sprite.TextureCoordinates.X = TextureRectangle.Left / TextureWidth;
            Sprite.TextureCoordinates.Y = TextureRectangle.Top / TextureHeight;
            Sprite.TextureSize.X = (TextureRectangle.Right - TextureRectangle.Left) / TextureWidth;
            Sprite.TextureSize.Y = (TextureRectangle.Bottom - TextureRectangle.Top) / TextureHeight;
            Sprite.TextureIndex = 0;
            Sprite.ColorModulate = Element.TextureColor.Current;

            return Manager.Sprite.DrawSpritesBuffered(new[] { Sprite }, 1);
        }

        public int DrawText(string Text, Element Element, ref Rectangle Destination, bool Shadow = false, int Count = -1)
        {
            int Result;

            // No need to draw fully transparent layers
            if (Element.FontColor.Current.A == 0) return 0;

            var Screen = Destination;
            Screen.Offset(X, Y);

            // If caption is enabled, offset the Y position by its height.
            if (!string.IsNullOrEmpty(Caption)) Screen.Offset(0, CaptionHeight);

            var BackBufferWidth = (float)Manager.BackBufferWidth;
            var BackBufferHeight = (float)Manager.BackBufferHeight;

            Matrix MatrixProjection;
            D3DX10Functions.MatrixOrthoOffCenterLH(out MatrixProjection, 0.0f, BackBufferWidth, 0.0f, BackBufferHeight, 0.1f, 10);
            Manager.Sprite.SetProjectionTransform(ref MatrixProjection);

            DialogResourceManager.FontNode[] FontNode = GetFont(Element.Font);

            if (Shadow)
            {
                var ShadowRectangle = Screen;
                ShadowRectangle.Offset(1, 1);
                var Color = new Color(0, 0, 0, Element.FontColor.Current.A);
                Result = FontNode[0].Font.DrawTextW(Manager.Sprite, Text, Count, ref ShadowRectangle, Element.TextFormat, ref Color);
                if (Result < 0) return Result;
            }

            Result = FontNode[0].Font.DrawTextW(Manager.Sprite, Text, Count, ref Screen, Element.TextFormat, ref Element.FontColor.Current);
            return Result < 0 ? Result : 0;
        }

        public void Refresh()
        {
            if (ControlFocus != null) ControlFocus.OnFocusOut();

            if (ControlMouseOver != null) ControlMouseOver.OnMouseLeave();

            ControlFocus = null;
            ControlPressed = null;
            ControlMouseOver = null;

            foreach (var Control in Controls)
            {
                Control.Refresh();
            }

            if (KeyboardInput) FocusDefaultControl();
        }

        public static void ClearFocus()
        {
            if (ControlFocus != null)
            {
                ControlFocus.OnFocusOut();
                ControlFocus = null;
            }
        }

        public void FocusDefaultControl()
        {
            // Check for default control in this dialog
            foreach (var Control in Controls)
            {
                if (Control.IsDefault)
                {
                    // Remove focus from the current control
                    ClearFocus();

                    // Give focus to the default control
                    ControlFocus = Control;
                    ControlFocus.OnFocusIn();
                    return;
                }
            }
        }

        public DialogResourceManager.TextureNode[] GetTexture(uint Index)
        {
            return Manager == null ? null : Manager.GetTextureNode(Textures[(int)Index]);
        }
    }
}