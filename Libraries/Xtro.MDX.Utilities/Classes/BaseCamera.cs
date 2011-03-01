using System;
using System.Drawing;
using System.Windows.Forms;
using FormsKeys = System.Windows.Forms.Keys;
using Xtro.MDX.Direct3DX10;
using D3DX10Functions = Xtro.MDX.Direct3DX10.Functions;

namespace Xtro.MDX.Utilities
{
    public class BaseCamera
    {
// ReSharper disable MemberCanBePrivate.Global
        protected Matrix View;              // View matrix 
        protected Matrix Projection;              // Projection matrix

        protected int KeysDown;            // Number of camera keys that are down.
        protected readonly KeyMaskFlag[] Keys = new KeyMaskFlag[(int)CameraKeys.MaximumKeys];  // State of input - KEY_WAS_DOWN_MASK|KEY_IS_DOWN_MASK
        protected Vector3 KeyboardDirection;   // Direction vector of keyboard input
        protected Point LastMousePosition;  // Last absolute position of mouse cursor
        protected bool MouseLeftButtonDown;    // True if left button is down 
        protected bool MouseMiddleButtonDown;    // True if middle button is down 
        protected bool MouseRightButtonDown;    // True if right button is down 
        protected MouseKeys CurrentButtonMask;   // mask of which buttons are down
        protected int MouseWheelDelta;     // Amount of middle wheel scroll (+/-) 
        protected Vector2 MouseDelta;          // Mouse relative delta smoothed over a few frames
        protected float FramesToSmoothMouseData; // Number of frames to smooth mouse data over

        protected Vector3 DefaultEye;          // Default camera eye position
        protected Vector3 DefaultLookAt;       // Default LookAt position
        protected Vector3 Eye;                 // Camera eye position
        protected Vector3 LookAt;              // LookAt position
        protected float CameraYawAngle;      // Yaw angle of camera
        protected float CameraPitchAngle;    // Pitch angle of camera

        protected Rectangle Drag;               // Rectangle within which a drag can be initiated.
        protected Vector3 Velocity;            // Velocity of camera
        protected bool MovementDrag;        // If true, then camera movement will slow to a stop otherwise movement is instant
        protected Vector3 VelocityDrag;        // Velocity drag force
        protected float DragTimer;           // Countdown timer to apply drag
        protected float TotalDragTimeToZero; // Time it takes for velocity to go from full to 0
        protected Vector2 RotVelocity;         // Velocity of camera

        protected float FOV;                 // Field of view
        protected float Aspect;              // Aspect ratio
        protected float NearPlane;           // Near plane
        protected float FarPlane;            // Far plane

        protected float RotationScaler;      // Scaler for rotation
        protected float MoveScaler;          // Scaler for movement

        protected bool InvertPitch;         // Invert the pitch axis
        protected bool EnablePositionMovement; // If true, then the user can translate the camera/model 
        protected bool EnableYAxisMovement; // If true, then camera can move in the y-axis

        protected bool ClipToBoundary;      // If true, then the camera will be clipped to the boundary
        protected Vector3 MinimumBoundary;         // Min point in clip boundary
        protected Vector3 MaximumBoundary;         // Max point in clip boundary

        protected bool ResetCursorAfterMove;// If true, the class will reset the cursor position so that the cursor always has space to move 
        // ReSharper restore MemberCanBePrivate.Global

        //--------------------------------------------------------------------------------------
        // Maps a windows virtual key to an enum
        //--------------------------------------------------------------------------------------
        protected virtual CameraKeys MapKey(Keys Key)
        {
            // This could be upgraded to a method that's user-definable but for 
            // simplicity, we'll use a hardcoded mapping.
            switch (Key)
            {
            case FormsKeys.Control: return CameraKeys.ControlDown;
            case FormsKeys.Left: return CameraKeys.StrafeLeft;
            case FormsKeys.Right: return CameraKeys.StrafeRight;
            case FormsKeys.Up: return CameraKeys.MoveForward;
            case FormsKeys.Down: return CameraKeys.MoveBackward;
            case FormsKeys.Prior: return CameraKeys.MoveUp;// pgup
            case FormsKeys.Next: return CameraKeys.MoveDown;// pgdn

            case FormsKeys.A: return CameraKeys.StrafeLeft;
            case FormsKeys.D: return CameraKeys.StrafeRight;
            case FormsKeys.W: return CameraKeys.MoveForward;
            case FormsKeys.S: return CameraKeys.MoveBackward;
            case FormsKeys.E: return CameraKeys.MoveUp;
            case FormsKeys.Q: return CameraKeys.MoveDown;

            case FormsKeys.NumPad4: return CameraKeys.StrafeLeft;
            case FormsKeys.NumPad6: return CameraKeys.StrafeRight;
            case FormsKeys.NumPad8: return CameraKeys.MoveForward;
            case FormsKeys.NumPad2: return CameraKeys.MoveBackward;
            case FormsKeys.NumPad9: return CameraKeys.MoveUp;
            case FormsKeys.NumPad3: return CameraKeys.MoveDown;

            case FormsKeys.Home: return CameraKeys.Reset;
            }

            return CameraKeys.Unknown;
        }

        protected static bool IsKeyDown(KeyMaskFlag Key)
        {
            return ((Key & KeyMaskFlag.IsDown) == KeyMaskFlag.IsDown);
        }

        protected bool WasKeyDown(KeyMaskFlag Key)
        {
            return ((Key & KeyMaskFlag.WasDown) == KeyMaskFlag.WasDown);
        }

        //--------------------------------------------------------------------------------------
        // Clamps Vector to lie inside m_vMinBoundary & m_vMaxBoundary
        //--------------------------------------------------------------------------------------
        protected void ConstrainToBoundary(ref Vector3 Vector)
        {
            // Constrain vector to a bounding box 
            Vector.X = Math.Max(Vector.X, MinimumBoundary.X);
            Vector.Y = Math.Max(Vector.Y, MinimumBoundary.Y);
            Vector.Z = Math.Max(Vector.Z, MinimumBoundary.Z);

            Vector.X = Math.Min(Vector.X, MaximumBoundary.X);
            Vector.Y = Math.Min(Vector.Y, MaximumBoundary.Y);
            Vector.Z = Math.Min(Vector.Z, MaximumBoundary.Z);
        }

        //--------------------------------------------------------------------------------------
        // Figure out the mouse delta based on mouse movement
        //--------------------------------------------------------------------------------------
        protected void UpdateMouseDelta()
        {
            // Get current position of mouse
            var CursorMousePosition = Cursor.Position;

            // Calc how far it's moved since last frame
            var CursorMouseDelta = CursorMousePosition - new Size(LastMousePosition);

            // Record current position for next time
            LastMousePosition = CursorMousePosition;

            if (ResetCursorAfterMove && Functions.IsActive())
            {
                // Set position of camera to center of desktop, 
                // so it always has room to move.  This is very useful
                // if the cursor is hidden.  If this isn't done and cursor is hidden, 
                // then invisible cursor will hit the edge of the screen 
                // and the user can't tell what happened

                // Get the center of the current monitor
                var Monitor = Screen.FromControl(Functions.GetForm());
                var Center = new Point
                {
                    X = (Monitor.Bounds.Left + Monitor.Bounds.Right) / 2,
                    Y = (Monitor.Bounds.Top + Monitor.Bounds.Bottom) / 2
                };

                Cursor.Position = Center;
                LastMousePosition = Center;
            }

            // Smooth the relative mouse data over a few frames so it isn't 
            // jerky when moving slowly at low frame rates.
            var PercentOfNew = 1.0f / FramesToSmoothMouseData;
            var PercentOfOld = 1.0f - PercentOfNew;
            MouseDelta.X = MouseDelta.X * PercentOfOld + CursorMouseDelta.X * PercentOfNew;
            MouseDelta.Y = MouseDelta.Y * PercentOfOld + CursorMouseDelta.Y * PercentOfNew;

            RotVelocity = MouseDelta * RotationScaler;
        }

        //--------------------------------------------------------------------------------------
        // Figure out the velocity based on keyboard input & drag if any
        //--------------------------------------------------------------------------------------
        protected void UpdateVelocity(float ElapsedTime)
        {
            RotVelocity = MouseDelta * RotationScaler /*+GamePadRightThumb * 0.02f*/;

            var Accel = KeyboardDirection/* + m_vGamePadLeftThumb*/;

            // Normalize vector so if moving 2 dirs (left & forward), 
            // the camera doesn't move faster than if moving in 1 dir
            D3DX10Functions.Vector3Normalize(out Accel, ref Accel);

            // Scale the acceleration vector
            Accel *= MoveScaler;

            if (MovementDrag)
            {
                // Is there any acceleration this frame?
                if (D3DX10Functions.Vector3LengthSquare(ref Accel) > 0)
                {
                    // If so, then this means the user has pressed a movement key\
                    // so change the velocity immediately to acceleration 
                    // upon keyboard input.  This isn't normal physics
                    // but it will give a quick response to keyboard input
                    Velocity = Accel;
                    DragTimer = TotalDragTimeToZero;
                    VelocityDrag = Accel / DragTimer;
                }
                else
                {
                    // If no key being pressed, then slowly decrease velocity to 0
                    if (DragTimer > 0)
                    {
                        // Drag until timer is <= 0
                        Velocity -= VelocityDrag * ElapsedTime;
                        DragTimer -= ElapsedTime;
                    }
                    else
                    {
                        // Zero velocity
                        Velocity = new Vector3(0, 0, 0);
                    }
                }
            }
            else
            {
                // No drag, so immediately change the velocity
                Velocity = Accel;
            }
        }

        //--------------------------------------------------------------------------------------
        // Figure out the velocity based on keyboard input & drag if any
        //--------------------------------------------------------------------------------------
        protected void GetInput(bool GetKeyboardInput, bool GetMouseInput, bool GetGamepadInput, bool ResetCursorAfterMove)
        {
            KeyboardDirection = new Vector3(0, 0, 0);
            if (GetKeyboardInput)
            {
                // Update acceleration vector based on keyboard state
                if (IsKeyDown(Keys[(int)CameraKeys.MoveForward])) KeyboardDirection.Z += 1.0f;
                if (IsKeyDown(Keys[(int)CameraKeys.MoveBackward])) KeyboardDirection.Z -= 1.0f;
                if (EnableYAxisMovement)
                {
                    if (IsKeyDown(Keys[(int)CameraKeys.MoveUp])) KeyboardDirection.Y += 1.0f;
                    if (IsKeyDown(Keys[(int)CameraKeys.MoveDown])) KeyboardDirection.Y -= 1.0f;
                }
                if (IsKeyDown(Keys[(int)CameraKeys.StrafeRight])) KeyboardDirection.X += 1.0f;
                if (IsKeyDown(Keys[(int)CameraKeys.StrafeLeft])) KeyboardDirection.X -= 1.0f;
            }

            if (GetMouseInput)
            {
                UpdateMouseDelta();
            }

            if (GetGamepadInput)
            {  /*
                m_vGamePadLeftThumb = D3DXVECTOR3( 0, 0, 0 );
                m_vGamePadRightThumb = D3DXVECTOR3( 0, 0, 0 );

                // Get controller state
                for( DWORD iUserIndex = 0; iUserIndex < DXUT_MAX_CONTROLLERS; iUserIndex++ )
                {
                    DXUTGetGamepadState( iUserIndex, &m_GamePad[iUserIndex], true, true );

                    // Mark time if the controller is in a non-zero state
                    if( m_GamePad[iUserIndex].wButtons ||
                        m_GamePad[iUserIndex].sThumbLX || m_GamePad[iUserIndex].sThumbLX ||
                        m_GamePad[iUserIndex].sThumbRX || m_GamePad[iUserIndex].sThumbRY ||
                        m_GamePad[iUserIndex].bLeftTrigger || m_GamePad[iUserIndex].bRightTrigger )
                    {
                        m_GamePadLastActive[iUserIndex] = DXUTGetTime();
                    }
                }

                // Find out which controller was non-zero last
                int iMostRecentlyActive = -1;
                double fMostRecentlyActiveTime = 0.0f;
                for( DWORD iUserIndex = 0; iUserIndex < DXUT_MAX_CONTROLLERS; iUserIndex++ )
                {
                    if( m_GamePadLastActive[iUserIndex] > fMostRecentlyActiveTime )
                    {
                        fMostRecentlyActiveTime = m_GamePadLastActive[iUserIndex];
                        iMostRecentlyActive = iUserIndex;
                    }
                }

                // Use the most recent non-zero controller if its connected
                if( iMostRecentlyActive >= 0 && m_GamePad[iMostRecentlyActive].bConnected )
                {
                    m_vGamePadLeftThumb.x = m_GamePad[iMostRecentlyActive].fThumbLX;
                    m_vGamePadLeftThumb.y = 0.0f;
                    m_vGamePadLeftThumb.z = m_GamePad[iMostRecentlyActive].fThumbLY;

                    m_vGamePadRightThumb.x = m_GamePad[iMostRecentlyActive].fThumbRX;
                    m_vGamePadRightThumb.y = 0.0f;
                    m_vGamePadRightThumb.z = m_GamePad[iMostRecentlyActive].fThumbRY;
                }  */
            }
        }

        public BaseCamera()
        {
            // Set attributes for the view matrix
            var EyePoint = new Vector3(0.0f, 0.0f, 0.0f);
            var LookatPoint = new Vector3(0.0f, 0.0f, 1.0f);

            // Setup the view matrix
            SetViewParameters(ref EyePoint, ref LookatPoint);

            // Setup the projection matrix
            SetProjectionParameters(Constants.PI / 4, 1.0f, 1.0f, 1000.0f);

            LastMousePosition = Cursor.Position;

            Drag = new Rectangle(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue);
            TotalDragTimeToZero = 0.25f;

            RotationScaler = 0.01f;
            MoveScaler = 5.0f;

            EnableYAxisMovement = true;
            EnablePositionMovement = true;

            FramesToSmoothMouseData = 2.0f;

            MinimumBoundary = new Vector3(-1, -1, -1);
            MaximumBoundary = new Vector3(1, 1, 1);
        }

        public virtual void HandleKeyUpEvent(KeyEventArgs E)
        {
            // Map this key to a D3DUtil_CameraKeys enum and update the
            // state of m_aKeys[] by removing the KEY_IS_DOWN_MASK mask.
            var MappedKey = MapKey(E.KeyCode);
            if (MappedKey != CameraKeys.Unknown && (uint)MappedKey < 8)
            {
                Keys[(int)MappedKey] &= ~KeyMaskFlag.IsDown;
                KeysDown--;
            }
        }

        public virtual void HandleMouseDownAndDoubleClickEvent(MouseEventArgs E)
        {
            // Compute the drag rectangle in screen coord.
            var DragContains = Drag.Contains(new Point(E.X, E.Y));

            // Update member var state
            if (E.Button == MouseButtons.Left && DragContains)
            {
                MouseLeftButtonDown = true;
                CurrentButtonMask |= MouseKeys.Left;
            }
            if (E.Button == MouseButtons.Middle && DragContains)
            {
                MouseMiddleButtonDown = true;
                CurrentButtonMask |= MouseKeys.Middle;
            }
            if (E.Button == MouseButtons.Right && DragContains)
            {
                MouseRightButtonDown = true;
                CurrentButtonMask |= MouseKeys.Right;
            }

            LastMousePosition = Cursor.Position;
        }

        public virtual void HandleMouseUpEvent(MouseEventArgs E)
        {
            // Update member var state
            if (E.Button == MouseButtons.Left)
            {
                MouseLeftButtonDown = false;
                CurrentButtonMask &= ~MouseKeys.Left;
            }
            if (E.Button == MouseButtons.Middle)
            {
                MouseMiddleButtonDown = false;
                CurrentButtonMask &= ~MouseKeys.Middle;
            }
            if (E.Button == MouseButtons.Right)
            {
                MouseRightButtonDown = false;
                CurrentButtonMask &= ~MouseKeys.Right;
            }
        }

        public virtual void HandleMouseWheelEvent(MouseEventArgs E)
        {
            // Update member var state
            MouseWheelDelta += E.Delta;
        }

        public virtual void FrameMove(float ElapsedTime)
        {
        }

        //--------------------------------------------------------------------------------------
        // Reset the camera's position back to the default
        //--------------------------------------------------------------------------------------
        public virtual void Reset()
        {
            SetViewParameters(ref DefaultEye, ref DefaultLookAt);
        }

        //--------------------------------------------------------------------------------------
        // Client can call this to change the position and direction of camera
        //--------------------------------------------------------------------------------------
        public virtual void SetViewParameters(ref Vector3 EyePoint, ref Vector3 LookatPoint)
        {
            DefaultEye = Eye = EyePoint;
            DefaultLookAt = LookAt = LookatPoint;

            // Calc the view matrix
            var Up = new Vector3(0, 1, 0);
            D3DX10Functions.MatrixLookAtLH(out View, ref EyePoint, ref LookatPoint, ref Up);

            Matrix InverseView;
            D3DX10Functions.MatrixInverse(out InverseView, ref View);

            // The axis basis vectors and camera position are stored inside the 
            // position matrix in the 4 rows of the camera's world matrix.
            // To figure out the yaw/pitch of the camera, we just need the Z basis vector
            var Basis = new Vector3(InverseView.Value31, InverseView.Value32, InverseView.Value33);

            CameraYawAngle = (float)Math.Atan2(Basis.X, Basis.Z);
            var Length = Math.Sqrt(Basis.Z * Basis.Z + Basis.X * Basis.X);
            CameraPitchAngle = -(float)Math.Atan2(Basis.Y, Length);
        }

        //--------------------------------------------------------------------------------------
        // Calculates the projection matrix based on input params
        //--------------------------------------------------------------------------------------
        public virtual void SetProjectionParameters(float FOV, float Aspect, float NearPlane, float FarPlane)
        {
            // Set attributes for the projection matrix
            this.FOV = FOV;
            this.Aspect = Aspect;
            this.NearPlane = NearPlane;
            this.FarPlane = FarPlane;

            D3DX10Functions.MatrixPerspectiveFovLH(out Projection, FOV, Aspect, NearPlane, FarPlane);
        }

        public virtual void HandleKeyDownEvent(KeyEventArgs E)
        {
            // Map this key to a D3DUtil_CameraKeys enum and update the
            // state of m_aKeys[] by adding the KEY_WAS_DOWN_MASK|KEY_IS_DOWN_MASK mask
            // only if the key is not down
            var MappedKey = MapKey(E.KeyCode);
            if (MappedKey != CameraKeys.Unknown)
            {
                if (!IsKeyDown(Keys[(int)MappedKey]))
                {
                    Keys[(int)MappedKey] = KeyMaskFlag.WasDown | KeyMaskFlag.IsDown;
                    KeysDown++;
                }
            }
        }

        public virtual void SetDragRectangle(ref Rectangle Rectangle)
        {
            Drag = Rectangle;
        }

        public void SetInvertPitch(bool InvertPitch)
        {
            this.InvertPitch = InvertPitch;
        }

        public void SetDrag(bool MovementDrag, float TotalDragTimeToZero = 0.25f)
        {
            this.MovementDrag = MovementDrag;
            this.TotalDragTimeToZero = TotalDragTimeToZero;
        }

        public void SetEnableYAxisMovement(bool EnableYAxisMovement)
        {
            this.EnableYAxisMovement = EnableYAxisMovement;
        }

        public void SetEnablePositionMovement(bool EnablePositionMovement)
        {
            this.EnablePositionMovement = EnablePositionMovement;
        }

        public void SetClipToBoundary(bool ClipToBoundary, Vector3[] MinimumBoundary, Vector3[] MaximumBoundary)
        {
            this.ClipToBoundary = ClipToBoundary;
            if (MinimumBoundary != null && MinimumBoundary.Length > 0) this.MinimumBoundary = MinimumBoundary[0];
            if (MaximumBoundary != null && MaximumBoundary.Length > 0) this.MaximumBoundary = MaximumBoundary[0];
        }

        public void SetScalers(float RotationScaler = 0.01f, float MoveScaler = 5.0f)
        {
            this.RotationScaler = RotationScaler;
            this.MoveScaler = MoveScaler;
        }

        public void SetNumberOfFramesToSmoothMouseData(int Frames)
        {
            if (Frames > 0) FramesToSmoothMouseData = Frames;
        }

        public void SetResetCursorAfterMove(bool ResetCursorAfterMove)
        {
            this.ResetCursorAfterMove = ResetCursorAfterMove;
        }

        // Functions to get state
        public Matrix GetViewMatrix()
        {
            return View;
        }

        public Matrix GetProjMatrix()
        {
            return Projection;
        }

        public Vector3 GetEyePt()
        {
            return Eye;
        }

        public Vector3 GetLookAtPt()
        {
            return LookAt;
        }

        public float GetNearClip()
        {
            return NearPlane;
        }

        public float GetFarClip()
        {
            return FarPlane;
        }

        public bool IsBeingDragged()
        {
            return (MouseLeftButtonDown || MouseMiddleButtonDown || MouseRightButtonDown);
        }

        public bool IsMouseLeftButtonDown()
        {
            return MouseLeftButtonDown;
        }

        public bool IsMouseMiddleButtonDown()
        {
            return MouseMiddleButtonDown;
        }

        public bool IsMouseRightButtonDown()
        {
            return MouseRightButtonDown;
        }
    }
}