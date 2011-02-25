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
        protected Matrix View;              // View matrix 
        protected Matrix Projection;              // Projection matrix

        protected int KeysDown;            // Number of camera keys that are down.
        protected readonly CameraKeys[] Keys = new CameraKeys[(int)CameraKeys.MaximumKeys];  // State of input - KEY_WAS_DOWN_MASK|KEY_IS_DOWN_MASK
        protected Vector3 KeyboardDirection;   // Direction vector of keyboard input
        protected Point LastMousePosition;  // Last absolute position of mouse cursor
        protected bool MouseLeftButtonDown;    // True if left button is down 
        protected bool MouseMiddleButtonDown;    // True if middle button is down 
        protected bool MouseRightButtonDown;    // True if right button is down 
        protected int CurrentButtonMask;   // mask of which buttons are down
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
        protected Vector3 MinBoundary;         // Min point in clip boundary
        protected Vector3 MaxBoundary;         // Max point in clip boundary

        protected bool ResetCursorAfterMove;// If true, the class will reset the cursor position so that the cursor always has space to move 

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

        protected bool IsKeyDown(KeyMaskFlag Key)
        {
            return ((Key & KeyMaskFlag.IsDown) == KeyMaskFlag.IsDown);
        }

        bool WasKeyDown(KeyMaskFlag Key)
        {
            return ((Key & KeyMaskFlag.WasDown) == KeyMaskFlag.WasDown);
        }

        //--------------------------------------------------------------------------------------
        // Clamps Vector to lie inside m_vMinBoundary & m_vMaxBoundary
        //--------------------------------------------------------------------------------------
        protected void ConstrainToBoundary(Vector3 Vector)
        {
            // Constrain vector to a bounding box 
            Vector.X = Math.Max(Vector.X, MinBoundary.X);
            Vector.Y = Math.Max(Vector.Y, MinBoundary.Y);
            Vector.Z = Math.Max(Vector.Z, MinBoundary.Z);

            Vector.X = Math.Min(Vector.X, MaxBoundary.X);
            Vector.Y = Math.Min(Vector.Y, MaxBoundary.Y);
            Vector.Z = Math.Min(Vector.Z, MaxBoundary.Z);
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
            Matrix RotDelta;
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
                if (IsKeyDown((KeyMaskFlag)Keys[(int)CameraKeys.MoveForward])) KeyboardDirection.Z += 1.0f;
                if (IsKeyDown((KeyMaskFlag)Keys[(int)CameraKeys.MoveBackward])) KeyboardDirection.Z -= 1.0f;
                if (EnableYAxisMovement)
                {
                    if (IsKeyDown((KeyMaskFlag)Keys[(int)CameraKeys.MoveUp])) KeyboardDirection.Y += 1.0f;
                    if (IsKeyDown((KeyMaskFlag)Keys[(int)CameraKeys.MoveDown])) KeyboardDirection.Y -= 1.0f;
                }
                if (IsKeyDown((KeyMaskFlag)Keys[(int)CameraKeys.StrafeRight])) KeyboardDirection.X += 1.0f;
                if (IsKeyDown((KeyMaskFlag)Keys[(int)CameraKeys.StrafeLeft])) KeyboardDirection.X -= 1.0f;
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
    }
}