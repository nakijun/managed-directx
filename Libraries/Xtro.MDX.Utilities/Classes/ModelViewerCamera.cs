using System;
using System.Drawing;
using System.Windows.Forms;
using Xtro.MDX.Direct3DX10;
using D3DX10Functions = Xtro.MDX.Direct3DX10.Functions;

namespace Xtro.MDX.Utilities
{
    public sealed class ModelViewerCamera : BaseCamera
    {
        readonly ArcBall WorldArcBall = new ArcBall();
        readonly ArcBall ViewArcBall = new ArcBall();
        Vector3 ModelCenter;
        Matrix ModelLastRotation;        // Last arcball rotation matrix for model 
        Matrix ModelRotation;            // Rotation matrix of model
        Matrix World;               // World matrix of model

        MouseKeys RotateModelButtonMask;
        MouseKeys ZoomButtonMask;
        MouseKeys RotateCameraButtonMask;

        bool AttachCameraToModel;
        //bool LimitPitch;
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

        //--------------------------------------------------------------------------------------
        // Update the view matrix & the model's world matrix based 
        //       on user input & elapsed time
        //--------------------------------------------------------------------------------------
        public override void FrameMove(float ElapsedTime)
        {
            if (IsKeyDown(Keys[(int)CameraKeys.Reset])) Reset();

            // If no dragged has happend since last time FrameMove is called,
            // and no camera key is held down, then no need to handle again.
            if (!DragSinceLastUpdate && KeysDown == 0) return;
            DragSinceLastUpdate = false;

            //// If no mouse button is held down, 
            //// Get the mouse movement (if any) if the mouse button are down
            //if( m_nCurrentButtonMask != 0 ) 
            //    UpdateMouseDelta( ElapsedTime );

            GetInput(EnablePositionMovement, CurrentButtonMask != 0, true, false);

            // Get amount of velocity based on the keyboard input and drag (if any)
            UpdateVelocity(ElapsedTime);

            // Simple euler method to calculate position delta
            var PositionDelta = Velocity * ElapsedTime;

            // Change the radius from the camera to the model based on wheel scrolling
            if (MouseWheelDelta != 0 && ZoomButtonMask == MouseKeys.Wheel) Radius -= MouseWheelDelta * Radius * 0.1f / 120.0f;
            Radius = Math.Min(MaximumRadius, Radius);
            Radius = Math.Max(MinimumRadius, Radius);
            MouseWheelDelta = 0;

            // Get the inverse of the arcball's rotation matrix
            Matrix CameraRotation;
            var RotationMatrix = ViewArcBall.GetRotationMatrix();
            D3DX10Functions.MatrixInverse(out CameraRotation, ref RotationMatrix);

            // Transform vectors based on camera's rotation matrix
            Vector3 WorldUp, WorldAhead;
            var LocalUp = new Vector3(0, 1, 0);
            var LocalAhead = new Vector3(0, 0, 1);
            D3DX10Functions.Vector3TransformCoordinates(out WorldUp, ref LocalUp, ref CameraRotation);
            D3DX10Functions.Vector3TransformCoordinates(out WorldAhead, ref LocalAhead, ref CameraRotation);

            // Transform the position delta by the camera's rotation 
            Vector3 PositionDeltaWorld;
            D3DX10Functions.Vector3TransformCoordinates(out PositionDeltaWorld, ref PositionDelta, ref CameraRotation);

            // Move the lookAt position 
            LookAt += PositionDeltaWorld;
            if (ClipToBoundary) ConstrainToBoundary(ref LookAt);

            // Update the eye point based on a radius away from the lookAt position
            Eye = LookAt - WorldAhead * Radius;

            // Update the view matrix
            D3DX10Functions.MatrixLookAtLH(out View, ref Eye, ref LookAt, ref WorldUp);

            Matrix InverseView;
            D3DX10Functions.MatrixInverse(out InverseView, ref View);
            InverseView.Value41 = InverseView.Value42 = InverseView.Value43 = 0;

            Matrix ModelLastRotationInverse;
            D3DX10Functions.MatrixInverse(out ModelLastRotationInverse, ref ModelLastRotation);

            // Accumulate the delta of the arcball's rotation in view space.
            // xNote that per-frame delta rotations could be problematic over long periods of time.
            var CurrentModelRotation = WorldArcBall.GetRotationMatrix();
            ModelRotation *= View * ModelLastRotationInverse * CurrentModelRotation * InverseView;

            if (ViewArcBall.IsBeingDragged() && AttachCameraToModel && !IsKeyDown(Keys[(int)CameraKeys.ControlDown]))
            {
                // Attach camera to model by inverse of the model rotation
                Matrix CameraLastRotationInverse;
                D3DX10Functions.MatrixInverse(out CameraLastRotationInverse, ref CameraRotationLast);
                var CameraRotationDelta = CameraLastRotationInverse * CameraRotation; // local to world matrix
                ModelRotation *= CameraRotationDelta;
            }
            CameraRotationLast = CameraRotation;

            ModelLastRotation = CurrentModelRotation;

            // Since we're accumulating delta rotations, we need to orthonormalize 
            // the matrix to prevent eventual matrix skew
            var BasisX = new Vector3(ModelRotation.Value11, ModelRotation.Value12, ModelRotation.Value13);
            Vector3 BasisY;// = new Vector3(ModelRotation.Value21, ModelRotation.Value22, ModelRotation.Value23);
            var BasisZ = new Vector3(ModelRotation.Value31, ModelRotation.Value32, ModelRotation.Value33);
            D3DX10Functions.Vector3Normalize(out BasisX, ref BasisX);
            D3DX10Functions.Vector3Cross(out BasisY, ref BasisZ, ref BasisX);
            D3DX10Functions.Vector3Normalize(out BasisY, ref BasisY);
            D3DX10Functions.Vector3Cross(out BasisZ, ref BasisX, ref BasisY);
            ModelRotation.Value11 = BasisX.X; ModelRotation.Value12 = BasisX.Y; ModelRotation.Value13 = BasisX.Z;
            ModelRotation.Value21 = BasisY.X; ModelRotation.Value22 = BasisY.Y; ModelRotation.Value23 = BasisY.Z;
            ModelRotation.Value31 = BasisZ.X; ModelRotation.Value32 = BasisZ.Y; ModelRotation.Value33 = BasisZ.Z;

            // Translate the rotation matrix to the same position as the lookAt position
            ModelRotation.Value41 = LookAt.X;
            ModelRotation.Value42 = LookAt.Y;
            ModelRotation.Value43 = LookAt.Z;

            // Translate world matrix so its at the center of the model
            Matrix Translation;
            D3DX10Functions.MatrixTranslation(out Translation, -ModelCenter.X, -ModelCenter.Y, -ModelCenter.Z);
            World = Translation * ModelRotation;
        }

        public override void SetDragRectangle(ref Rectangle Rectangle)
        {
            base.SetDragRectangle(ref Rectangle);

            WorldArcBall.SetOffset(Rectangle.Left, Rectangle.Top);
            ViewArcBall.SetOffset(Rectangle.Left, Rectangle.Top);
            SetWindow(Rectangle.Right - Rectangle.Left, Rectangle.Bottom - Rectangle.Top);
        }

        //--------------------------------------------------------------------------------------
        // Reset the camera's position back to the default
        //--------------------------------------------------------------------------------------
        public override void Reset()
        {
            base.Reset();

            D3DX10Functions.MatrixIdentity(out World);
            D3DX10Functions.MatrixIdentity(out ModelRotation);
            D3DX10Functions.MatrixIdentity(out ModelLastRotation);
            D3DX10Functions.MatrixIdentity(out CameraRotationLast);

            Radius = DefaultRadius;
            WorldArcBall.Reset();
            ViewArcBall.Reset();
        }

        //--------------------------------------------------------------------------------------
        // Override for setting the view parameters
        //--------------------------------------------------------------------------------------
        public override void SetViewParameters(ref Vector3 EyePoint, ref Vector3 LookatPoint)
        {
            base.SetViewParameters(ref EyePoint, ref LookatPoint);

            // Propogate changes to the member arcball
            Quaternion Quaternion;
            Matrix Rotation;
            var Up = new Vector3(0, 1, 0);
            D3DX10Functions.MatrixLookAtLH(out Rotation, ref EyePoint, ref LookatPoint, ref Up);
            D3DX10Functions.QuaternionRotationMatrix(out Quaternion, ref Rotation);
            ViewArcBall.SetQuaternionNow(ref Quaternion);

            // Set the radius according to the distance
            Vector3 EyeToPoint;
            D3DX10Functions.Vector3Subtract(out EyeToPoint, ref LookatPoint, ref EyePoint);
            SetRadius(D3DX10Functions.Vector3Length(ref EyeToPoint));

            // View information changed. FrameMove should be called.
            DragSinceLastUpdate = true;
        }

        public void SetButtonMasks(MouseKeys RotateModelButtonMask = MouseKeys.Left, MouseKeys ZoomButtonMask = MouseKeys.Wheel, MouseKeys RotateCameraButtonMask = MouseKeys.Right)
        {
            this.RotateModelButtonMask = RotateModelButtonMask;
            this.ZoomButtonMask = ZoomButtonMask;
            this.RotateCameraButtonMask = RotateCameraButtonMask;
        }

        public void SetAttachCameraToModel(bool Enable = false)
        {
            AttachCameraToModel = Enable;
        }

        public void SetWindow(int Width, int Height, float ArcballRadius = 0.9f)
        {
            WorldArcBall.SetWindow(Width, Height, ArcballRadius);
            ViewArcBall.SetWindow(Width, Height, ArcballRadius);
        }

        public void SetRadius(float DefaultRadius = 5.0f, float MinimumRadius = 1.0f, float MaximumRadius = float.MaxValue)
        {
            this.DefaultRadius = Radius = DefaultRadius;
            this.MinimumRadius = MinimumRadius;
            this.MaximumRadius = MaximumRadius;
            DragSinceLastUpdate = true;
        }

        public void SetModelCenter(Vector3 ModelCenter)
        {
            this.ModelCenter = ModelCenter;
        }

        //public void SetLimitPitch(bool LimitPitch)
        //{
        //    this.LimitPitch = LimitPitch;
        //}

        public void SetViewQuat(Quaternion Q)
        {
            ViewArcBall.SetQuaternionNow(ref Q);
            DragSinceLastUpdate = true;
        }

        public void SetWorldQuat(Quaternion Q)
        {
            WorldArcBall.SetQuaternionNow(ref Q);
            DragSinceLastUpdate = true;
        }

        // Functions to get state
        public Matrix GetWorldMatrix()
        {
            return World;
        }

        public void SetWorldMatrix(ref Matrix World)
        {
            this.World = World;
            DragSinceLastUpdate = true;
        }
    }
}