using System.Windows.Forms;
using Xtro.MDX.Direct3DX10;
using D3DX10Functions = Xtro.MDX.Direct3DX10.Functions;

namespace Xtro.MDX.Utilities
{
    public class ModelViewerCamera : BaseCamera
    {
        ArcBall WorldArcBall;
        ArcBall ViewArcBall;
        Vector3 ModelCenter;
        Matrix ModelLastRotation;        // Last arcball rotation matrix for model 
        Matrix ModelRotation;            // Rotation matrix of model
        Matrix World;               // World matrix of model

        MouseKeys RotateModelButtonMask;
        MouseKeys ZoomButtonMask;
        MouseKeys RotateCameraButtonMask;

        bool AttachCameraToModel;
        bool LimitPitch;
        float Radius;              // Distance from the camera to model 
        float DefaultRadius;       // Distance from the camera to model 
        float MinimumRadius;           // Min radius
        float MaximumRadius;           // Max radius
        bool DragSinceLastUpdate; // True if mouse drag has happened since last time FrameMove is called.

        Matrix CameraRotationLast;

        public ModelViewerCamera()
        {
            D3DX10Functions.MatrixIdentity(out World);
            D3DX10Functions.MatrixIdentity(out ModelRotation);
            D3DX10Functions.MatrixIdentity(out ModelLastRotation);
            D3DX10Functions.MatrixIdentity(out CameraRotationLast);
            Radius = 5.0f;
            DefaultRadius = 5.0f;
            MinimumRadius = 1.0f;
            MaximumRadius = float.MaxValue;

            RotateModelButtonMask = MouseKeys.Left;
            ZoomButtonMask = MouseKeys.Wheel;
            RotateCameraButtonMask = MouseKeys.Right;
            DragSinceLastUpdate = true;
        }

        public override void HandleMouseDownAndDoubleClickEvent(MouseEventArgs E)
        {
            base.HandleMouseDownAndDoubleClickEvent(E);

            if ((E.Button == MouseButtons.Left && (RotateModelButtonMask & MouseKeys.Left) > 0) ||
                (E.Button == MouseButtons.Middle && (RotateModelButtonMask & MouseKeys.Middle) > 0) ||
                (E.Button == MouseButtons.Right && (RotateModelButtonMask & MouseKeys.Right) > 0))
            {
                WorldArcBall.OnBegin(E.X, E.Y);
            }

            if ((E.Button == MouseButtons.Left && (RotateCameraButtonMask & MouseKeys.Left) > 0) ||
                (E.Button == MouseButtons.Middle && (RotateCameraButtonMask & MouseKeys.Middle) > 0) ||
                (E.Button == MouseButtons.Right && (RotateCameraButtonMask & MouseKeys.Right) > 0))
            {
                ViewArcBall.OnBegin(E.X, E.Y);
            }

            if (E.Button == MouseButtons.Left || E.Button == MouseButtons.Middle || E.Button == MouseButtons.Right) DragSinceLastUpdate = true;
        }

        public void HandleMouseMoveEvent(MouseEventArgs E)
        {
            WorldArcBall.OnMove(E.X, E.Y);
            ViewArcBall.OnMove(E.X, E.Y);

            DragSinceLastUpdate = true;
        }

        public override void HandleMouseWheelEvent(MouseEventArgs E)
        {
            base.HandleMouseWheelEvent(E);

            DragSinceLastUpdate = true;
        }

        public override void HandleMouseUpEvent(MouseEventArgs E)
        {
            base.HandleMouseUpEvent(E);

            if ((E.Button == MouseButtons.Left && (RotateModelButtonMask & MouseKeys.Left) > 0) ||
                (E.Button == MouseButtons.Middle && (RotateModelButtonMask & MouseKeys.Middle) > 0) ||
                (E.Button == MouseButtons.Right && (RotateModelButtonMask & MouseKeys.Right) > 0))
            {
                WorldArcBall.OnEnd();
            }

            if ((E.Button == MouseButtons.Left && (RotateCameraButtonMask & MouseKeys.Left) > 0) ||
                (E.Button == MouseButtons.Middle && (RotateCameraButtonMask & MouseKeys.Middle) > 0) ||
                (E.Button == MouseButtons.Right && (RotateCameraButtonMask & MouseKeys.Right) > 0))
            {
                ViewArcBall.OnEnd();
            }

            if (E.Button == MouseButtons.Left || E.Button == MouseButtons.Middle || E.Button == MouseButtons.Right) DragSinceLastUpdate = true;
        }
    }
}