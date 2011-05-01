using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        Element CapElement;  // Element for the caption

        Dialog NextDialog;
        Dialog PrevDialog;
    }
}