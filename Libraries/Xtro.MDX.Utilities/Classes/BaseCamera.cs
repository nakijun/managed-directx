using System.Windows.Forms;
using Xtro.MDX.Direct3DX10;

namespace Xtro.MDX.Utilities
{
    public class BaseCamera
    {/*
    protected Matrix View;              // View matrix 
    protected Matrix Projection;              // Projection matrix
    protected readonly CameraKeys[] Keys=new CameraKeys[(int)CameraKeys.MaximumKeys];  // State of input - KEY_WAS_DOWN_MASK|KEY_IS_DOWN_MASK
    protected Vector3 KeyboardDirection;   // Direction vector of keyboard input
    protected POINT m_ptLastMousePosition;  // Last absolute position of mouse cursor
    protected bool m_bMouseLButtonDown;    // True if left button is down 
    protected bool m_bMouseMButtonDown;    // True if middle button is down 
    protected bool m_bMouseRButtonDown;    // True if right button is down 
    protected int m_nCurrentButtonMask;   // mask of which buttons are down
    protected int m_nMouseWheelDelta;     // Amount of middle wheel scroll (+/-) 
    protected D3DXVECTOR2 m_vMouseDelta;          // Mouse relative delta smoothed over a few frames
    protected float m_fFramesToSmoothMouseData; // Number of frames to smooth mouse data over
    protected 
    protected D3DXVECTOR3 m_vDefaultEye;          // Default camera eye position
    protected D3DXVECTOR3 m_vDefaultLookAt;       // Default LookAt position
    protected D3DXVECTOR3 m_vEye;                 // Camera eye position
    protected D3DXVECTOR3 m_vLookAt;              // LookAt position
    protected float m_fCameraYawAngle;      // Yaw angle of camera
    protected float m_fCameraPitchAngle;    // Pitch angle of camera
    protected 
    protected RECT m_rcDrag;               // Rectangle within which a drag can be initiated.
    protected D3DXVECTOR3 m_vVelocity;            // Velocity of camera
    protected bool m_bMovementDrag;        // If true, then camera movement will slow to a stop otherwise movement is instant
    protected D3DXVECTOR3 m_vVelocityDrag;        // Velocity drag force
    protected FLOAT m_fDragTimer;           // Countdown timer to apply drag
    protected FLOAT m_fTotalDragTimeToZero; // Time it takes for velocity to go from full to 0
    protected D3DXVECTOR2 m_vRotVelocity;         // Velocity of camera
    protected 
    protected float m_fFOV;                 // Field of view
    protected float m_fAspect;              // Aspect ratio
    protected float m_fNearPlane;           // Near plane
    protected float m_fFarPlane;            // Far plane
    protected 
    protected float m_fRotationScaler;      // Scaler for rotation
    protected float m_fMoveScaler;          // Scaler for movement
    protected 
    protected bool m_bInvertPitch;         // Invert the pitch axis
    protected bool m_bEnablePositionMovement; // If true, then the user can translate the camera/model 
    protected bool m_bEnableYAxisMovement; // If true, then camera can move in the y-axis
    protected 
    protected bool m_bClipToBoundary;      // If true, then the camera will be clipped to the boundary
    protected D3DXVECTOR3 m_vMinBoundary;         // Min point in clip boundary
    protected D3DXVECTOR3 m_vMaxBoundary;         // Max point in clip boundary
    protected 
    protected bool m_bResetCursorAfterMove;// If true, the class will reset the cursor position so that the cursor always has space to move 

        //--------------------------------------------------------------------------------------
// Maps a windows virtual key to an enum
//--------------------------------------------------------------------------------------
        protected virtual CameraKeys MapKey( Keys Key )
        {
            // This could be upgraded to a method that's user-definable but for 
            // simplicity, we'll use a hardcoded mapping.
            switch (Key)
            {
            case Keys.Control: return CameraKeys.ControlDown;
            case Keys.Left:return CameraKeys.StrafeLeft;
            case Keys.Right:return CameraKeys.StrafeRight;
            case Keys.Up:return CameraKeys.MoveForward;
            case Keys.Down:return CameraKeys.MoveBackward;
            case Keys.Prior:return CameraKeys.MoveUp;// pgup
            case Keys.Next:return CameraKeys.MoveDown;// pgdn

            case Keys.A: return CameraKeys.StrafeLeft;
            case Keys.D: return CameraKeys.StrafeRight;
            case Keys.W: return CameraKeys.MoveForward;
            case Keys.S: return CameraKeys.MoveBackward;
            case Keys.E: return CameraKeys.MoveUp;
            case Keys.Q: return CameraKeys.MoveDown;

            case Keys.NumPad4: return CameraKeys.StrafeLeft;
            case Keys.NumPad6: return CameraKeys.StrafeRight;
            case Keys.NumPad8: return CameraKeys.MoveForward;
            case Keys.NumPad2: return CameraKeys.MoveBackward;
            case Keys.NumPad9: return CameraKeys.MoveUp;
            case Keys.NumPad3: return CameraKeys.MoveDown;

            case Keys.Home:return CameraKeys.Reset;
            }

            return CameraKeys.Unknown;
        }

        protected bool IsKeyDown( KeyMaskFlag Key )
        {
            return ((Key & KeyMaskFlag.IsDown) == KeyMaskFlag.IsDown); 
        }

        bool WasKeyDown(KeyMaskFlag Key)
        {
            return ((Key & KeyMaskFlag.WasDown) == KeyMaskFlag.WasDown);
        }
*/    }
}
