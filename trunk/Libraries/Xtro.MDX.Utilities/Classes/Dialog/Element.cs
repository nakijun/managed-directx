using System;
using System.Drawing;
using Xtro.MDX.Direct3DX10;
using Color = Xtro.MDX.Direct3DX10.Color;
using D3DX10Functions = Xtro.MDX.Direct3DX10.Functions;

namespace Xtro.MDX.Utilities
{
    public sealed class Element
    {
        public Element()
        {
            TextureColor.States = new uint[Control.MaximumStates];
            FontColor.States = new uint[Control.MaximumStates];
        }

        public struct BlendColor
        {
            public uint[] States; // Modulate colors for all possible control states
            public Color Current;

            public void Init(uint DefaultColor, uint DisabledColor = (uint)13172864, uint HiddenColor = (uint)0)
            {
                for (var I = 0; I < Control.MaximumStates; I++)
                {
                    States[I] = DefaultColor;
                }

                States[(int)ControlState.Disabled] = DisabledColor;
                States[(int)ControlState.Hidden] = HiddenColor;
                Current = new Color(HiddenColor);
            }

            public void Blend(ControlState State, float ElapsedTime, float Rate = 0.7f)
            {
                var DestinationColor = new Color(States[(int)State]);
                D3DX10Functions.ColorLerp(out Current, ref Current, ref DestinationColor, (float)(1.0f - Math.Pow(Rate, 30 * ElapsedTime)));
            }

            public void Clone(ref BlendColor Target)
            {
                Target.States = (uint[])States.Clone();
            }
        };

        public uint Texture;          // Index of the texture for this Element 
        public uint Font;             // Index of the font for this Element
        public FontDrawFlag TextFormat;     // The format argument to DrawText 

        public Rectangle TextureRectangle;         // Bounding rect of this element on the composite texture

        public BlendColor TextureColor;
        public BlendColor FontColor;

        public void SetTexture(uint Texture, Rectangle[] TextureRectangle, uint DefaultTextureColor = uint.MaxValue)
        {
            this.Texture = Texture;

            if (TextureRectangle != null && TextureRectangle.Length > 0) this.TextureRectangle = TextureRectangle[0];
            else this.TextureRectangle = new Rectangle();

            TextureColor.Init(DefaultTextureColor);
        }

        public void SetFont(uint Font, uint DefaultFontColor = uint.MaxValue, FontDrawFlag TextFormat = FontDrawFlag.Center | FontDrawFlag.VerticalCenter)
        {
            this.Font = Font;
            this.TextFormat = TextFormat;

            FontColor.Init(DefaultFontColor);
        }

        public void Refresh()
        {
            TextureColor.Current = new Color(TextureColor.States[(int)ControlState.Hidden]);
            FontColor.Current = new Color(FontColor.States[(int)ControlState.Hidden]);
        }

        public Element Clone()
        {
            var Result = new Element
            {         
                Font = Font,
                TextFormat = TextFormat,
                Texture = Texture,
                TextureRectangle = TextureRectangle
            };

            FontColor.Clone(ref Result.FontColor);
            TextureColor.Clone(ref Result.TextureColor);

            return Result;
        }
    }
}