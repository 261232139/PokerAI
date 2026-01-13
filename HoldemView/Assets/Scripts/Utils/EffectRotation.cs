using UnityEngine;

namespace GameEffect
{
    /// <summary>
    /// 自旋转
    /// </summary>
    public class EffectRotation : MonoBehaviour
    {
        /// <summary>
        /// 旋转速度
        /// </summary>
        public float Speed = 10;

        public bool xRotate = false;
        public bool yRotate = false;
        public bool zRotate = false;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void LateUpdate()
        {
            if (xRotate || yRotate || zRotate)
            {
                float x = 0;
                float y = 0;
                float z = 0;

                if (xRotate)
                {
                    x = Time.deltaTime * Speed;
                }

                if (yRotate)
                {
                    y = Time.deltaTime * Speed;
                }

                if (zRotate)
                {
                    z = Time.deltaTime * Speed;
                }

                transform.localEulerAngles += new Vector3(x, y, z);
            }
        }
    }
}