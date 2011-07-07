using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Xtro.MDX.Direct3D10;
using Xtro.MDX.Direct3DX10;
using Color = Xtro.MDX.Direct3DX10.Color;
using SystemColor = System.Drawing.Color;
using D3DX10Functions = Xtro.MDX.Direct3DX10.Functions;

namespace Xtro.MDX.Utilities
{
    public class Dialog
    {
        public struct ElementHolder
        {
            public ControlType ControlType;
            public uint ElementNo;

            public Element Element;
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
        List<ElementHolder[]> DefaultElements;

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

        void InitDefaultElements()
        {
            SetFont(0, "Arial", 14, FontWeight.Normal);

            var Element = new Element();

            //-------------------------------------
            // Element for the caption
            //-------------------------------------
            CaptionElement.SetFont(0);
            var TextureRectangle = new Rectangle(17, 269, 241, 287);
            CaptionElement.SetTexture(0, new[] { TextureRectangle });
            CaptionElement.TextureColor.States[(int)ControlState.Normal] = (uint)SystemColor.FromArgb(255, 255, 255, 255).ToArgb();
            CaptionElement.FontColor.States[(int)ControlState.Normal] = (uint)SystemColor.FromArgb(255, 255, 255, 255).ToArgb();
            CaptionElement.SetFont(0, (uint)SystemColor.FromArgb(255, 255, 255, 255).ToArgb(), FontDrawFlag.Left | FontDrawFlag.VerticalCenter);
            // Pre-blend as we don't need to transition the state
            CaptionElement.TextureColor.Blend((int)ControlState.Normal, 10.0f);
            CaptionElement.FontColor.Blend((int)ControlState.Normal, 10.0f);

            //-------------------------------------
            // CDXUTStatic
            //-------------------------------------
            Element.SetFont(0);
            Element.FontColor.States[(int)ControlState.Disabled] = (uint)SystemColor.FromArgb(200, 200, 200, 200).ToArgb();

            // Assign the Element
            SetDefaultElement(ControlType.Static, 0, Element);


            //-------------------------------------
            // CDXUTButton - Button
            //-------------------------------------
            TextureRectangle = new Rectangle(0, 0, 136, 54);
            Element.SetTexture(0, new[] { TextureRectangle });
            Element.SetFont(0);
            Element.TextureColor.States[(int)ControlState.Normal] = (uint)SystemColor.FromArgb(150, 255, 255, 255).ToArgb();
            Element.TextureColor.States[(int)ControlState.Pressed] = (uint)SystemColor.FromArgb(200, 255, 255, 255).ToArgb();
            Element.FontColor.States[(int)ControlState.MouseOver] = (uint)SystemColor.FromArgb(255, 0, 0, 0).ToArgb();

            // Assign the Element
            SetDefaultElement(ControlType.Button, 0, Element);


            //-------------------------------------
            // CDXUTButton - Fill layer
            //-------------------------------------
            TextureRectangle = new Rectangle(136, 0, 252, 54);
            Element.SetTexture(0, new[] { TextureRectangle }, (uint)SystemColor.FromArgb(0, 255, 255, 255).ToArgb());
            Element.TextureColor.States[(int)ControlState.MouseOver] = (uint)SystemColor.FromArgb(160, 255, 255, 255).ToArgb();
            Element.TextureColor.States[(int)ControlState.Pressed] = (uint)SystemColor.FromArgb(60, 0, 0, 0).ToArgb();
            Element.TextureColor.States[(int)ControlState.Focus] = (uint)SystemColor.FromArgb(30, 255, 255, 255).ToArgb();


            // Assign the Element
            SetDefaultElement(ControlType.Button, 1, Element);


            //-------------------------------------
            // CDXUTCheckBox - Box
            //-------------------------------------
            TextureRectangle = new Rectangle(0, 54, 27, 81);
            Element.SetTexture(0, new[] { TextureRectangle });
            Element.SetFont(0, (uint)SystemColor.FromArgb(255, 255, 255, 255).ToArgb(), FontDrawFlag.Left | FontDrawFlag.VerticalCenter);
            Element.FontColor.States[(int)ControlState.Disabled] = (uint)SystemColor.FromArgb(200, 200, 200, 200).ToArgb();
            Element.TextureColor.States[(int)ControlState.Normal] = (uint)SystemColor.FromArgb(150, 255, 255, 255).ToArgb();
            Element.TextureColor.States[(int)ControlState.Focus] = (uint)SystemColor.FromArgb(200, 255, 255, 255).ToArgb();
            Element.TextureColor.States[(int)ControlState.Pressed] = (uint)SystemColor.FromArgb(255, 255, 255, 255).ToArgb();

            // Assign the Element
            SetDefaultElement(ControlType.CheckBox, 0, Element);


            //-------------------------------------
            // CDXUTCheckBox - Check
            //-------------------------------------
            TextureRectangle = new Rectangle(27, 54, 54, 81);
            Element.SetTexture(0, new[] { TextureRectangle });

            // Assign the Element
            SetDefaultElement(ControlType.CheckBox, 1, Element);


            //-------------------------------------
            // CDXUTRadioButton - Box
            //-------------------------------------
            TextureRectangle = new Rectangle(54, 54, 81, 81);
            Element.SetTexture(0, new[] { TextureRectangle });
            Element.SetFont(0, (uint)SystemColor.FromArgb(255, 255, 255, 255).ToArgb(), FontDrawFlag.Left | FontDrawFlag.VerticalCenter);
            Element.FontColor.States[(int)ControlState.Disabled] = (uint)SystemColor.FromArgb(200, 200, 200, 200).ToArgb();
            Element.TextureColor.States[(int)ControlState.Normal] = (uint)SystemColor.FromArgb(150, 255, 255, 255).ToArgb();
            Element.TextureColor.States[(int)ControlState.Focus] = (uint)SystemColor.FromArgb(200, 255, 255, 255).ToArgb();
            Element.TextureColor.States[(int)ControlState.Pressed] = (uint)SystemColor.FromArgb(255, 255, 255, 255).ToArgb();

            // Assign the Element
            SetDefaultElement(ControlType.RadioButton, 0, Element);


            //-------------------------------------
            // CDXUTRadioButton - Check
            //-------------------------------------
            TextureRectangle = new Rectangle(81, 54, 108, 81);
            Element.SetTexture(0, new[] { TextureRectangle });

            // Assign the Element
            SetDefaultElement(ControlType.RadioButton, 1, Element);


            //-------------------------------------
            // CDXUTComboBox - Main
            //-------------------------------------
            TextureRectangle = new Rectangle(7, 81, 247, 123);
            Element.SetTexture(0, new[] { TextureRectangle });
            Element.SetFont(0);
            Element.TextureColor.States[(int)ControlState.Normal] = (uint)SystemColor.FromArgb(150, 200, 200, 200).ToArgb();
            Element.TextureColor.States[(int)ControlState.Focus] = (uint)SystemColor.FromArgb(170, 230, 230, 230).ToArgb();
            Element.TextureColor.States[(int)ControlState.Disabled] = (uint)SystemColor.FromArgb(70, 200, 200, 200).ToArgb();
            Element.FontColor.States[(int)ControlState.MouseOver] = (uint)SystemColor.FromArgb(255, 0, 0, 0).ToArgb();
            Element.FontColor.States[(int)ControlState.Pressed] = (uint)SystemColor.FromArgb(255, 0, 0, 0).ToArgb();
            Element.FontColor.States[(int)ControlState.Disabled] = (uint)SystemColor.FromArgb(200, 200, 200, 200).ToArgb();


            // Assign the Element
            SetDefaultElement(ControlType.ComboBox, 0, Element);


            //-------------------------------------
            // CDXUTComboBox - Button
            //-------------------------------------
            TextureRectangle = new Rectangle(98, 189, 151, 238);
            Element.SetTexture(0, new[] { TextureRectangle });
            Element.TextureColor.States[(int)ControlState.Normal] = (uint)SystemColor.FromArgb(150, 255, 255, 255).ToArgb();
            Element.TextureColor.States[(int)ControlState.Pressed] = (uint)SystemColor.FromArgb(255, 150, 150, 150).ToArgb();
            Element.TextureColor.States[(int)ControlState.Focus] = (uint)SystemColor.FromArgb(200, 255, 255, 255).ToArgb();
            Element.TextureColor.States[(int)ControlState.Disabled] = (uint)SystemColor.FromArgb(70, 255, 255, 255).ToArgb();

            // Assign the Element
            SetDefaultElement(ControlType.ComboBox, 1, Element);


            //-------------------------------------
            // CDXUTComboBox - Dropdown
            //-------------------------------------
            TextureRectangle = new Rectangle(13, 123, 241, 160);
            Element.SetTexture(0, new[] { TextureRectangle });
            Element.SetFont(0, (uint)SystemColor.FromArgb(255, 0, 0, 0).ToArgb(), FontDrawFlag.Left | FontDrawFlag.Top);

            // Assign the Element
            SetDefaultElement(ControlType.ComboBox, 2, Element);


            //-------------------------------------
            // CDXUTComboBox - Selection
            //-------------------------------------
            TextureRectangle = new Rectangle(12, 163, 239, 183);
            Element.SetTexture(0, new[] { TextureRectangle });
            Element.SetFont(0, (uint)SystemColor.FromArgb(255, 255, 255, 255).ToArgb(), FontDrawFlag.Left | FontDrawFlag.Top);

            // Assign the Element
            SetDefaultElement(ControlType.ComboBox, 3, Element);


            //-------------------------------------
            // CDXUTSlider - Track
            //-------------------------------------
            TextureRectangle = new Rectangle(1, 187, 93, 228);
            Element.SetTexture(0, new[] { TextureRectangle });
            Element.TextureColor.States[(int)ControlState.Normal] = (uint)SystemColor.FromArgb(150, 255, 255, 255).ToArgb();
            Element.TextureColor.States[(int)ControlState.Focus] = (uint)SystemColor.FromArgb(200, 255, 255, 255).ToArgb();
            Element.TextureColor.States[(int)ControlState.Disabled] = (uint)SystemColor.FromArgb(70, 255, 255, 255).ToArgb();

            // Assign the Element
            SetDefaultElement(ControlType.Slider, 0, Element);

            //-------------------------------------
            // CDXUTSlider - Button
            //-------------------------------------
            TextureRectangle = new Rectangle(151, 193, 192, 234);
            Element.SetTexture(0, new[] { TextureRectangle });

            // Assign the Element
            SetDefaultElement(ControlType.Slider, 1, Element);

            //-------------------------------------
            // CDXUTScrollBar - Track
            //-------------------------------------
            const int ScrollBarStartX = 196;
            const int ScrollBarStartY = 191;
            TextureRectangle = new Rectangle(ScrollBarStartX + 0, ScrollBarStartY + 21, ScrollBarStartX + 22, ScrollBarStartY + 32);
            Element.SetTexture(0, new[] { TextureRectangle });
            Element.TextureColor.States[(int)ControlState.Disabled] = (uint)SystemColor.FromArgb(255, 200, 200, 200).ToArgb();

            // Assign the Element
            SetDefaultElement(ControlType.ScrollBar, 0, Element);

            //-------------------------------------
            // CDXUTScrollBar - Up Arrow
            //-------------------------------------
            TextureRectangle = new Rectangle(ScrollBarStartX + 0, ScrollBarStartY + 1, ScrollBarStartX + 22, ScrollBarStartY + 21);
            Element.SetTexture(0, new[] { TextureRectangle });
            Element.TextureColor.States[(int)ControlState.Disabled] = (uint)SystemColor.FromArgb(255, 200, 200, 200).ToArgb();


            // Assign the Element
            SetDefaultElement(ControlType.ScrollBar, 1, Element);

            //-------------------------------------
            // CDXUTScrollBar - Down Arrow
            //-------------------------------------
            TextureRectangle = new Rectangle(ScrollBarStartX + 0, ScrollBarStartY + 32, ScrollBarStartX + 22, ScrollBarStartY + 53);
            Element.SetTexture(0, new[] { TextureRectangle });
            Element.TextureColor.States[(int)ControlState.Disabled] = (uint)SystemColor.FromArgb(255, 200, 200, 200).ToArgb();


            // Assign the Element
            SetDefaultElement(ControlType.ScrollBar, 2, Element);

            //-------------------------------------
            // CDXUTScrollBar - Button
            //-------------------------------------
            TextureRectangle = new Rectangle(220, 192, 238, 234);
            Element.SetTexture(0, new[] { TextureRectangle });

            // Assign the Element
            SetDefaultElement(ControlType.ScrollBar, 3, Element);


            //-------------------------------------
            // CDXUTEditBox
            //-------------------------------------
            // Element assignment:
            //   0 - text area
            //   1 - top left border
            //   2 - top border
            //   3 - top right border
            //   4 - left border
            //   5 - right border
            //   6 - lower left border
            //   7 - lower border
            //   8 - lower right border

            Element.SetFont(0, (uint)SystemColor.FromArgb(255, 0, 0, 0).ToArgb(), FontDrawFlag.Left | FontDrawFlag.Top);

            // Assign the style
            TextureRectangle = new Rectangle(14, 90, 241, 113);
            Element.SetTexture(0, new[] { TextureRectangle });
            SetDefaultElement(ControlType.EditBox, 0, Element);
            TextureRectangle = new Rectangle(8, 82, 14, 90);
            Element.SetTexture(0, new[] { TextureRectangle });
            SetDefaultElement(ControlType.EditBox, 1, Element);
            TextureRectangle = new Rectangle(14, 82, 241, 90);
            Element.SetTexture(0, new[] { TextureRectangle });
            SetDefaultElement(ControlType.EditBox, 2, Element);
            TextureRectangle = new Rectangle(241, 82, 246, 90);
            Element.SetTexture(0, new[] { TextureRectangle });
            SetDefaultElement(ControlType.EditBox, 3, Element);
            TextureRectangle = new Rectangle(8, 90, 14, 113);
            Element.SetTexture(0, new[] { TextureRectangle });
            SetDefaultElement(ControlType.EditBox, 4, Element);
            TextureRectangle = new Rectangle(241, 90, 246, 113);
            Element.SetTexture(0, new[] { TextureRectangle });
            SetDefaultElement(ControlType.EditBox, 5, Element);
            TextureRectangle = new Rectangle(8, 113, 14, 121);
            Element.SetTexture(0, new[] { TextureRectangle });
            SetDefaultElement(ControlType.EditBox, 6, Element);
            TextureRectangle = new Rectangle(14, 113, 241, 121);
            Element.SetTexture(0, new[] { TextureRectangle });
            SetDefaultElement(ControlType.EditBox, 7, Element);
            TextureRectangle = new Rectangle(241, 113, 246, 121);
            Element.SetTexture(0, new[] { TextureRectangle });
            SetDefaultElement(ControlType.EditBox, 8, Element);

            //-------------------------------------
            // CDXUTListBox - Main
            //-------------------------------------
            TextureRectangle = new Rectangle(13, 123, 241, 160);
            Element.SetTexture(0, new[] { TextureRectangle });
            Element.SetFont(0, (uint)SystemColor.FromArgb(255, 0, 0, 0).ToArgb(), FontDrawFlag.Left | FontDrawFlag.Top);

            // Assign the Element
            SetDefaultElement(ControlType.ListBox, 0, Element);

            //-------------------------------------
            // CDXUTListBox - Selection
            //-------------------------------------

            TextureRectangle = new Rectangle(16, 166, 240, 183);
            Element.SetTexture(0, new[] { TextureRectangle });
            Element.SetFont(0, (uint)SystemColor.FromArgb(255, 255, 255, 255).ToArgb(), FontDrawFlag.Left | FontDrawFlag.Top);

            // Assign the Element
            SetDefaultElement(ControlType.ListBox, 1, Element);
        }

        void OnMouseMove(Point Point)
        {
            // Figure out which control the mouse is over now
            var Control = GetControlAtPoint(Point);

            // If the mouse is still over the same control, nothing needs to be done
            if (Control == ControlMouseOver) return;

            // Handle mouse leaving the old control
            if (ControlMouseOver != null) ControlMouseOver.OnMouseLeave();

            // Handle mouse entering the new control
            ControlMouseOver = Control;
            if (Control != null) ControlMouseOver.OnMouseEnter();
        }

        void OnMouseUp(Point Point)
        {
            ControlPressed = null;
            ControlMouseOver = null;
        }

        internal void SetNextDialog(Dialog NextDialog)
        {
            if (NextDialog == null) NextDialog = this;

            this.NextDialog = NextDialog;
            this.NextDialog.PrevDialog = this;
        }

        bool OnCycleFocus(bool Forward)
        {
            Control Control = null;
            Dialog Dialog = null; // pDialog and pLastDialog are used to track wrapping of
            Dialog LastDialog = null;    // focus from first control to last or vice versa.

            if (ControlFocus == null)
            {
                // If s_pControlFocus is NULL, we focus the first control of first dialog in
                // the case that Forward is true, and focus the last control of last dialog when
                // Forward is false.
                //
                if (Forward)
                {
                    // Search for the first control from the start of the dialog
                    // array.
                    for (var D = 0; D < Manager.Dialogs.Count; D++)
                    {
                        Dialog = LastDialog = Manager.Dialogs[D];
                        if (Dialog != null && Dialog.Controls.Count > 0)
                        {
                            Control = Dialog.Controls[0];
                            break;
                        }
                    }

                    if (Dialog == null || Control == null)
                    {
                        // No dialog has been registered yet or no controls have been
                        // added to the dialogs. Cannot proceed.
                        return true;
                    }
                }
                else
                {
                    // Search for the first control from the end of the dialog
                    // array.
                    for (var D = Manager.Dialogs.Count - 1; D >= 0; D--)
                    {
                        Dialog = LastDialog = Manager.Dialogs[D];
                        if (Dialog != null && Dialog.Controls.Count > 0)
                        {
                            Control = Dialog.Controls[Dialog.Controls.Count - 1];
                            break;
                        }
                    }

                    if (Dialog == null || Control == null)
                    {
                        // No dialog has been registered yet or no controls have been
                        // added to the dialogs. Cannot proceed.
                        return true;
                    }
                }
            }
            else if (ControlFocus.Dialog != this)
            {
                // If a control belonging to another dialog has focus, let that other
                // dialog handle this event by returning false.
                //
                return false;
            }
            else
            {
                // Focused control belongs to this dialog. Cycle to the
                // next/previous control.
                LastDialog = ControlFocus.Dialog;
                Control = (Forward) ? GetNextControl(ControlFocus) : GetPrevControl(ControlFocus);
                Dialog = Control.Dialog;
            }

            for (var I = 0; I < 0xffff; I++)
            {
                // If we just wrapped from last control to first or vice versa,
                // set the focused control to NULL. This state, where no control
                // has focus, allows the camera to work.
                var LastDialogIndex = Manager.Dialogs.IndexOf(LastDialog);
                var DialogIndex = Manager.Dialogs.IndexOf(Dialog);
                if ((!Forward && LastDialogIndex < DialogIndex) || (Forward && DialogIndex < LastDialogIndex))
                {
                    if (ControlFocus != null) ControlFocus.OnFocusOut();
                    ControlFocus = null;
                    return true;
                }

                // If we've gone in a full circle then focus doesn't change
                if (Control == ControlFocus) return true;

                // If the dialog accepts keybord input and the control can have focus then
                // move focus
                if (Control.Dialog.KeyboardInput && Control.CanHaveFocus())
                {
                    if (ControlFocus != null) ControlFocus.OnFocusOut();
                    ControlFocus = Control;
                    ControlFocus.OnFocusIn();
                    return true;
                }

                LastDialog = Dialog;
                Control = (Forward) ? GetNextControl(Control) : GetPrevControl(Control);
                Dialog = Control.Dialog;
            }

            // If we reached this point, the chain of dialogs didn't form a complete loop
            return false;
        }

        ~Dialog()
        {
            Delete();
        }

        public Dialog()
        {
            Visible = true;
            CaptionHeight = 18;

            NextDialog = this;
            PrevDialog = this;

            DefaultControlID = 0xffff;
            MouseInput = true;
        }

        public void Delete()
        {
            RemoveAllControls();

            Fonts.Clear();
            Textures.Clear();
            DefaultElements.Clear();
        }

        public void Init(DialogResourceManager Manager, bool RegisterDialog, string ControlTextureFilename)
        {
            this.Manager = Manager;
            if (RegisterDialog) Manager.RegisterDialog(this);
            SetTexture(0, ControlTextureFilename);
            InitDefaultElements();
        }

        public bool HandleMouseDownAndDoubleClickEvent(MouseEventArgs E)
        {
            // For invisible dialog, do not handle anything.
            if (!Visible) return false;

            // If automation command-line switch is on, enable this dialog's keyboard input
            // upon any key press or mouse click.
            if (Functions.GetAutomation() && E.Button == MouseButtons.Left) Manager.EnableKeyboardInputForAllDialogs();

            // If caption is enable, check for clicks in the caption area.
            if (!string.IsNullOrEmpty(Caption))
            {
                if (E.Button == MouseButtons.Left)
                {
                    if (E.X >= X && E.X < X + Width && E.Y >= Y && E.Y < Y + CaptionHeight)
                    {
                        Drag = true;
                        return true;
                    }
                }
            }

            // If the dialog is minimized, don't send any messages to controls.
            if (Minimized) return false;

            // If not accepting mouse input, return false to indicate the message should still 
            // be handled by the application (usually to move the camera).
            if (!MouseInput) return false;

            var MousePoint = E.Location;

            // If caption is enabled, offset the Y coordinate by the negative of its height.
            if (!string.IsNullOrEmpty(Caption)) MousePoint.Y -= CaptionHeight;

            // If a control is in focus, it belongs to this dialog, and it's enabled, then give
            // it the first chance at handling the message.
            if (ControlFocus != null && ControlFocus.Dialog == this && ControlFocus.GetEnabled())
            {
                if (ControlFocus.HandleMouseDownAndDoubleClickEvent(E, MousePoint)) return true;
            }

            // Not yet handled, see if the mouse is over any controls
            var Control = GetControlAtPoint(MousePoint);
            if (Control != null && Control.GetEnabled())
            {
                if (Control.HandleMouseDownAndDoubleClickEvent(E, MousePoint)) return true;
            }
            else
            {
                // Mouse not over any controls in this dialog, if there was a control
                // which had focus it just lost it
                if (E.Button == MouseButtons.Left && ControlFocus != null && ControlFocus.Dialog == this)
                {
                    ControlFocus.OnFocusOut();
                    ControlFocus = null;
                }
            }

            return false;
        }

        public bool HandleKeyDownEvent(KeyEventArgs E)
        {
            // For invisible dialog, do not handle anything.
            if (!Visible) return false;

            // If automation command-line switch is on, enable this dialog's keyboard input
            // upon any key press or mouse click.
            if (Functions.GetAutomation()) Manager.EnableKeyboardInputForAllDialogs();

            // If the dialog is minimized, don't send any messages to controls.
            if (Minimized) return false;

            // If a control is in focus, it belongs to this dialog, and it's enabled, then give
            // it the first chance at handling the message.
            if (ControlFocus != null && ControlFocus.Dialog == this && ControlFocus.GetEnabled())
            {
                if (ControlFocus.HandleKeyDownEvent(E)) return true;
            }

            // Not yet handled, see if this matches a control's hotkey
            // Activate the hotkey if the focus doesn't belong to an
            // edit box.
            if (ControlFocus == null || (ControlFocus.GetType() != ControlType.EditBox && ControlFocus.GetType() != ControlType.ImeEditBox))
            {
                foreach (var C in Controls)
                {
                    if (C.GetHotkey() == E.KeyCode)
                    {
                        C.OnHotkey();
                        return true;
                    }
                }
            }

            // Not yet handled, check for focus messages

            // If keyboard input is not enabled, this message should be ignored
            if (!KeyboardInput) return false;

            switch (E.KeyCode)
            {
            case Keys.Right:
            case Keys.Down:
                if (ControlFocus != null)
                {
                    return OnCycleFocus(true);
                }
                break;

            case Keys.Left:
            case Keys.Up:
                if (ControlFocus != null)
                {
                    return OnCycleFocus(false);
                }
                break;

            case Keys.Tab:
                {
                    return OnCycleFocus(!E.Shift);
                }
            }

            return false;
        }

        public bool HandleMouseUpEvent(MouseEventArgs E)
        {
            // For invisible dialog, do not handle anything.
            if (!Visible) return false;

            // If caption is enable, check for clicks in the caption area.
            if (!string.IsNullOrEmpty(Caption))
            {
                if (E.Button == MouseButtons.Left && Drag)
                {
                    if (E.X >= X && E.X < X + Width && E.Y >= Y && E.Y < Y + CaptionHeight)
                    {
                        Drag = false;
                        Minimized = !Minimized;
                        return true;
                    }
                }
            }

            // If the dialog is minimized, don't send any messages to controls.
            if (Minimized) return false;

            // If not accepting mouse input, return false to indicate the message should still 
            // be handled by the application (usually to move the camera).
            if (!MouseInput) return false;

            var MousePoint = E.Location;
            MousePoint.X -= X;
            MousePoint.Y -= Y;

            // If caption is enabled, offset the Y coordinate by the negative of its height.
            if (!string.IsNullOrEmpty(Caption)) MousePoint.Y -= CaptionHeight;

            // If a control is in focus, it belongs to this dialog, and it's enabled, then give
            // it the first chance at handling the message.
            if (ControlFocus != null && ControlFocus.Dialog == this && ControlFocus.GetEnabled())
            {
                if (ControlFocus.HandleMouseUpEvent(E, MousePoint)) return true;
            }

            // Not yet handled, see if the mouse is over any controls
            var Control = GetControlAtPoint(MousePoint);
            if (Control != null && Control.GetEnabled())
            {
                if (Control.HandleMouseUpEvent(E, MousePoint)) return true;
            }

            return false;
        }

        public bool HandleResizeEvent()
        {
            // For invisible dialog, do not handle anything.
            if (!Visible) return false;

            // If the dialog is minimized, don't send any messages to controls.
            if (Minimized) return false;

            // If a control is in focus, it belongs to this dialog, and it's enabled, then give
            // it the first chance at handling the message.
            if (ControlFocus != null && ControlFocus.Dialog == this && ControlFocus.GetEnabled())
            {
                if (ControlFocus.HandleResizeEvent()) return true;
            }

            // Handle sizing and moving messages so that in case the mouse cursor is moved out
            // of an UI control because of the window adjustment, we can properly
            // unhighlight the highlighted control.
            var Point = new Point(-1, -1);
            OnMouseMove(Point);

            return false;
        }

        public bool HandleMoveEvent()
        {
            // For invisible dialog, do not handle anything.
            if (!Visible) return false;

            // If the dialog is minimized, don't send any messages to controls.
            if (Minimized) return false;

            // If a control is in focus, it belongs to this dialog, and it's enabled, then give
            // it the first chance at handling the message.
            if (ControlFocus != null && ControlFocus.Dialog == this && ControlFocus.GetEnabled())
            {
                if (ControlFocus.HandleMoveEvent()) return true;
            }

            // Handle sizing and moving messages so that in case the mouse cursor is moved out
            // of an UI control because of the window adjustment, we can properly
            // unhighlight the highlighted control.
            var Point = new Point(-1, -1);
            OnMouseMove(Point);

            return false;
        }

        public bool HandleActivatedEvent()
        {
            // For invisible dialog, do not handle anything.
            if (!Visible) return false;

            // If the dialog is minimized, don't send any messages to controls.
            if (Minimized) return false;

            // If a control is in focus, it belongs to this dialog, and it's enabled, then give
            // it the first chance at handling the message.
            if (ControlFocus != null && ControlFocus.Dialog == this && ControlFocus.GetEnabled())
            {
                if (ControlFocus.HandleActivatedEvent()) return true;
            }

            // Call OnFocusIn()/OnFocusOut() of the control that currently has the focus
            // as the application is activated/deactivated.  This matches the Windows
            // behavior.
            if (ControlFocus != null && ControlFocus.Dialog == this && ControlFocus.GetEnabled()) ControlFocus.OnFocusIn();

            return false;
        }

        public bool HandleDeactivateEvent()
        {
            // For invisible dialog, do not handle anything.
            if (!Visible) return false;

            // If the dialog is minimized, don't send any messages to controls.
            if (Minimized) return false;

            // If a control is in focus, it belongs to this dialog, and it's enabled, then give
            // it the first chance at handling the message.
            if (ControlFocus != null && ControlFocus.Dialog == this && ControlFocus.GetEnabled())
            {
                if (ControlFocus.HandleDeactivateEvent()) return true;
            }

            // Call OnFocusIn()/OnFocusOut() of the control that currently has the focus
            // as the application is activated/deactivated.  This matches the Windows
            // behavior.
            if (ControlFocus != null && ControlFocus.Dialog == this && ControlFocus.GetEnabled()) ControlFocus.OnFocusOut();

            return false;
        }

        public bool HandleKeyUpEvent(KeyEventArgs E)
        {
            // For invisible dialog, do not handle anything.
            if (!Visible) return false;

            // If the dialog is minimized, don't send any messages to controls.
            if (Minimized) return false;

            // If a control is in focus, it belongs to this dialog, and it's enabled, then give
            // it the first chance at handling the message.
            if (ControlFocus != null && ControlFocus.Dialog == this && ControlFocus.GetEnabled())
            {
                if (ControlFocus.HandleKeyUpEvent(E)) return true;
            }

            return false;
        }

        public bool HandleMouseMoveEvent(MouseEventArgs E)
        {
            // For invisible dialog, do not handle anything.
            if (!Visible) return false;

            // If the dialog is minimized, don't send any messages to controls.
            if (Minimized) return false;

            // If not accepting mouse input, return false to indicate the message should still 
            // be handled by the application (usually to move the camera).
            if (!MouseInput) return false;

            var MousePoint = E.Location;
            MousePoint.X -= X;
            MousePoint.Y -= Y;

            // If caption is enabled, offset the Y coordinate by the negative of its height.
            if (!string.IsNullOrEmpty(Caption)) MousePoint.Y -= CaptionHeight;

            // If a control is in focus, it belongs to this dialog, and it's enabled, then give
            // it the first chance at handling the message.
            if (ControlFocus != null && ControlFocus.Dialog == this && ControlFocus.GetEnabled())
            {
                if (ControlFocus.HandleMouseMoveEvent(E, MousePoint)) return true;
            }

            // Not yet handled, see if the mouse is over any controls
            var Control = GetControlAtPoint(MousePoint);
            if (Control != null && Control.GetEnabled())
            {
                if (Control.HandleMouseMoveEvent(E, MousePoint)) return true;
            }

            // Still not handled, hand this off to the dialog. Return false to indicate the
            // message should still be handled by the application (usually to move the camera).
            OnMouseMove(MousePoint);

            return false;
        }

        public bool HandleMouseWheelEvent(MouseEventArgs E)
        {
            // For invisible dialog, do not handle anything.
            if (!Visible) return false;

            // If the dialog is minimized, don't send any messages to controls.
            if (Minimized) return false;

            // If not accepting mouse input, return false to indicate the message should still 
            // be handled by the application (usually to move the camera).
            if (!MouseInput) return false;

            var MousePoint = E.Location;
            MousePoint.X -= X;
            MousePoint.Y -= Y;

            // If caption is enabled, offset the Y coordinate by the negative of its height.
            if (!string.IsNullOrEmpty(Caption)) MousePoint.Y -= CaptionHeight;

            // If a control is in focus, it belongs to this dialog, and it's enabled, then give
            // it the first chance at handling the message.
            if (ControlFocus != null && ControlFocus.Dialog == this && ControlFocus.GetEnabled())
            {
                if (ControlFocus.HandleMouseWheelEvent(E, MousePoint)) return true;
            }

            // Not yet handled, see if the mouse is over any controls
            var Control = GetControlAtPoint(MousePoint);
            if (Control != null && Control.GetEnabled())
            {
                if (Control.HandleMouseWheelEvent(E, MousePoint)) return true;
            }

            return false;
        }

        public bool HandleMouseCaptureChangedEvent(EventArgs E)
        {
            // For invisible dialog, do not handle anything.
            if (!Visible) return false;

            // If the dialog is minimized, don't send any messages to controls.
            if (Minimized) return false;

            // If a control is in focus, it belongs to this dialog, and it's enabled, then give
            // it the first chance at handling the message.
            if (ControlFocus != null && ControlFocus.Dialog == this && ControlFocus.GetEnabled())
            {
                if (ControlFocus.HandleMouseCaptureChangedEvent(E)) return true;
            }

            // The application has lost mouse capture.
            // The dialog object may not have received
            // a WM_MOUSEUP when capture changed. Reset
            // m_bDrag so that the dialog does not mistakenly
            // think the mouse button is still held down.
            Drag = false;

            return false;
        }

        public int AddStatic(int ID, string Text, int X, int Y, int Width, int Height, bool IsDefault, out Static Created)
        {
            var Static = new Static(this);

            Created = Static;

            var Result = AddControl(Static);
            if (Result < 0) return Result;

            // Set the ID and list index
            Static.SetID(ID);
            Static.Text = Text;
            Static.SetLocation(X, Y);
            Static.SetSize(Width, Height);
            Static.IsDefault = IsDefault;

            return 0;
        }

        public int AddButton(int ID, string Text, int X, int Y, int Width, int Height, Keys Hotkey, bool IsDefault, out Button Created)
        {
            var Button = new Button(this);

            Created = Button;

            var Result = AddControl(Button);
            if (Result < 0) return Result;

            // Set the ID and list index
            Button.SetID(ID);
            Button.Text = Text;
            Button.SetLocation(X, Y);
            Button.SetSize(Width, Height);
            Button.SetHotkey(Hotkey);
            Button.IsDefault = IsDefault;

            return 0;
        }

        public int AddCheckBox(int ID, string Text, int X, int Y, int Width, int Height, bool Checked, Keys Hotkey, bool IsDefault, out CheckBox Created)
        {
            var CheckBox = new CheckBox(this);

            Created = CheckBox;

            var Result = AddControl(CheckBox);
            if (Result < 0) return Result;

            // Set the ID and list index
            CheckBox.SetID(ID);
            CheckBox.Text = Text;
            CheckBox.SetLocation(X, Y);
            CheckBox.SetSize(Width, Height);
            CheckBox.SetHotkey(Hotkey);
            CheckBox.IsDefault = IsDefault;
            CheckBox.Checked = Checked;

            return 0;
        }

        public int AddControl(Control Control)
        {
            var Result = InitControl(Control);
            if (Result < 0) return Result;

            // Add to the list
            Controls.Add(Control);

            return 0;
        }

        public int InitControl(Control Control)
        {
            if (Control == null) return (int)Error.InvalidArgument;

            Control.Index = (uint)Controls.Count;

            // Look for a default Element entries
            foreach (var ElementHolder in DefaultElements)
            {
                if (ElementHolder[0].ControlType == Control.GetType()) Control.SetElement(ElementHolder[0].ElementNo, ElementHolder[0].Element);
            }

            var Result = Control.OnInit();

            return Result < 0 ? Result : 0;
        }

        public Control GetControlAtPoint(Point Point)
        {
            // Search through all child controls for the first one which
            // contains the mouse point
            foreach (var Control in Controls)
            {
                if (Control == null) continue;

                // We only return the current control if it is visible
                // and enabled.  Because GetControlAtPoint() is used to do mouse
                // hittest, it makes sense to perform this filtering.
                if (Control.ContainsPoint(Point) && Control.GetEnabled() && Control.GetVisible()) return Control;
            }

            return null;
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

            var FontNode = GetFont(Element.Font);

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

        public int SetFont(uint Index, string FaceName, int Height, FontWeight Weight)
        {
            // Make sure the list is at least as large as the index being set
            for (var I = Fonts.Count; I <= Index; I++)
            {
                Fonts.Add(-1);
            }

            var Font = Manager.AddFont(FaceName, Height, Weight);
            Fonts[(int)Index] = Font;

            return 0;
        }

        public int SetDefaultElement(ControlType ControlType, uint ElementNo, Element Element)
        {
            // If this Element type already exist in the list, simply update the stored Element
            for (var I = 0; I < DefaultElements.Count; I++)
            {
                if (DefaultElements[I][0].ControlType == ControlType && DefaultElements[I][0].ElementNo == ElementNo)
                {
                    DefaultElements[I][0].Element = Element;

                    return 0;
                }
            }

            // Otherwise, add a new entry
            var NewHolder = new ElementHolder
            {
                ControlType = ControlType,
                ElementNo = ElementNo,
                Element = Element
            };

            DefaultElements.Add(new[] { NewHolder });

            return 0;
        }

        public static Control GetNextControl(Control Control)
        {
            var Index = Control.Index + 1;

            var Dialog = Control.Dialog;

            // Cycle through dialogs in the loop to find the next control. Note
            // that if only one control exists in all looped dialogs it will
            // be the returned 'next' control.
            while (Index >= Dialog.Controls.Count)
            {
                Dialog = Dialog.NextDialog;
                Index = 0;
            }

            return Dialog.Controls[(int)Index];
        }

        public static Control GetPrevControl(Control Control)
        {
            var Index = (int)(Control.Index - 1);

            var Dialog = Control.Dialog;

            // Cycle through dialogs in the loop to find the next control. Note
            // that if only one control exists in all looped dialogs it will
            // be the returned 'previous' control.
            while (Index < 0)
            {
                Dialog = Dialog.PrevDialog ?? Control.Dialog;

                Index = Dialog.Controls.Count - 1;
            }

            return Dialog.Controls[Index];
        }

        public void RemoveAllControls()
        {
            if (ControlFocus != null && ControlFocus.Dialog == this) ControlFocus = null;
            if (ControlPressed != null && ControlPressed.Dialog == this) ControlPressed = null;
            ControlMouseOver = null;

            foreach (var Control in Controls)
            {
                Control.Delete();
            }

            Controls.Clear();
        }

        public int SetTexture(uint Index, string Filename)
        {
            // Make sure the list is at least as large as the index being set
            for (var I = Textures.Count; I <= Index; I++)
            {
                Textures.Add(-1);
            }

            var Texture = Manager.AddTexture(Filename);

            Textures[(int)Index] = Texture;
            return 0;
        }

        public void EnableKeyboardInput(bool Enable)
        {
            KeyboardInput = Enable;
        }

        public void SendEvent(Event Event, bool TriggeredByUser, Control Control)
        {
            // If no callback has been registered there's nowhere to send the event to
            if (CallbackEvent == null) return;

            // Discard events triggered programatically if these types of events haven't been
            // enabled
            if (!TriggeredByUser && !NonUserEvents) return;

            CallbackEvent(Event, Control.GetID(), Control, CallbackEventUserContext);
        }

        public bool IsKeyboardInputEnabled()
        {
            return KeyboardInput;
        }

        public static void RequestFocus(Control Control)
        {
            if (ControlFocus == Control) return;

            if (!Control.CanHaveFocus()) return;

            if (ControlFocus != null) ControlFocus.OnFocusOut();

            Control.OnFocusIn();
            ControlFocus = Control;
        }

        public void ClearRadioButtonGroup(uint ButtonGroup)
        {
            // Find all radio buttons with the given group number
            foreach (var Control in Controls)
            {
                if (Control.GetType() == ControlType.RadioButton)
                {
                    var RadioButton = (RadioButton)Control;

                    if (RadioButton.ButtonGroup == ButtonGroup) RadioButton.SetChecked(false, false);
                }
            }
        }
    }
}