using Point = System.Drawing.Point;
using Xtro.MDX.Direct3D10;
using Xtro.MDX.Direct3DX10;

namespace Xtro.MDX.Utilities
{
    public sealed class TextHelper
    {
        protected Font Font;
        protected Sprite Sprite;
        protected Color Color;
        protected Point Point;
        protected int LineHeight;
        protected BlendState FontBlendState;

        public TextHelper(Font Font = null, Sprite Sprite = null, int LineHeight = 15)
        {
            Init(Font, Sprite, LineHeight);
        }

        ~TextHelper()
        {
            Delete();
        }

        public void Delete()
        {
            if (FontBlendState != null)
            {
                FontBlendState.Release();
                FontBlendState = null;
            }
        }

        public void Init( Font Font=null, Sprite Sprite=null,int LineHeight =15)
        {
            this.Font = Font;
            this.Sprite = Sprite;
            Color = new Color( 1, 1, 1, 1 );
            Point = new Point(0, 0);
            this.LineHeight = LineHeight;
            this.FontBlendState= null;

            // Create a blend state if a sprite is passed in
            if( Sprite!=null )
            {
                Device Device;
                Sprite.GetDevice( out Device );
                if( Device!=null )
                {
                    var StateDescription=new BlendDescription
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

    }
}