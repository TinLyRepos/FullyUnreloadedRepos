using UnityEngine;

namespace De2Utils
{
    public class De2Helper : MonoBehaviour
    {
        private static Camera mainCamera;

        public static void CacheMainCamera()
        {
            mainCamera = Camera.main;
        }

        public static Vector3 GetMouseToWorldPosition()
        {
            // Clamp mouse screen position to screen size
            Vector3 screenPos = Input.mousePosition;
            screenPos.x = Mathf.Clamp(screenPos.x, 0.0f, Screen.width);
            screenPos.y = Mathf.Clamp(screenPos.y, 0.0f, Screen.height);

            Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
            worldPos.z = 0.0f;

            return worldPos;
        }

        public static float GetAngleFromVector(Vector3 vector)
        {
            float radians = Mathf.Atan2(vector.y, vector.x);
            float degrees = radians * Mathf.Rad2Deg;
            return degrees;
        }

        public static Vector3 GetRamdomDirection()
        {
            return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }

        //===========================================================================
        public static AimDirection GetAimDirection(float angleDegrees)
        {
            AimDirection aimDirection;

            //Up Right
            if (angleDegrees >= 22f && angleDegrees <= 67f)
                aimDirection = AimDirection.UpRight;

            // Up
            else if (angleDegrees > 67f && angleDegrees <= 112f)
                aimDirection = AimDirection.Up;

            // Up Left
            else if (angleDegrees > 112f && angleDegrees <= 158f)
                aimDirection = AimDirection.UpLeft;

            // Left
            else if ((angleDegrees <= 180f && angleDegrees > 158f) || (angleDegrees > -180 && angleDegrees <= -135f))
                aimDirection = AimDirection.Left;

            // Down
            else if ((angleDegrees > -135f && angleDegrees <= -45f))
                aimDirection = AimDirection.Down;

            // Right
            else if ((angleDegrees > -45f && angleDegrees <= 0f) || (angleDegrees > 0 && angleDegrees < 22f))
                aimDirection = AimDirection.Right;

            else
                return AimDirection.Down;

            return aimDirection;
        }
    }
}