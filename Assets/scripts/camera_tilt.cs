using UnityEngine;

namespace MyGame.CameraControl
{
    public class Camera_tilt : MonoBehaviour
    {
        public float Max_Camera_Angle = 15;
        public float Camera_Roll_Speed = 10;

        void Update()
        {
    
            float Target_Angle = -Max_Camera_Angle * Input.GetAxis("Horizontal");
            float New_Camera_Z = Mathf.LerpAngle(transform.localEulerAngles.z, Target_Angle, Time.deltaTime * Camera_Roll_Speed);
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, New_Camera_Z);

        }

    }

}