using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoUtils.Graphics
{
    public sealed class Camera
    {
        public readonly static float minZ = 1f;
        public readonly static float maxZ = 2048f;

        private Vector2 position;
        private float z;
        private float zBase;
        private float zoomFactor;

        private float aspectRatio;
        private float fieldOfView;

        private Matrix view;
        private Matrix projection;

        public Vector2 Position { get { return position; } }
        public float Z { get { return z; } }
        public float ZBase { get { return zBase; } }
        public float ZoomFactor { get { return zoomFactor; } }
        public Matrix View { get { return view; } }
        public Matrix Projection { get { return projection; } }

        public Camera(Screen screen)
        {
            if (screen is null) { throw new ArgumentNullException("screen"); }

            aspectRatio = (float)screen.Width / (float)screen.Height;
            fieldOfView = MathHelper.PiOver2;

            this.position = new Vector2(0, 0);
            this.zBase = this.GetZFromHeight((float)screen.Height);
            this.z = this.zBase;

            this.updateMatrices();
        }

        public void updateMatrices()
        {
            this.zoomFactor = (this.z / this.zBase);

            this.view = Matrix.CreateLookAt(new Vector3(position, this.z), new Vector3(position, 0f), Vector3.Up);
            this.projection = Matrix.CreatePerspectiveFieldOfView(this.fieldOfView, this.aspectRatio, Camera.minZ, Camera.maxZ);
        }

        public float GetZFromHeight(float height)
        {
            return (0.5f * height) / MathF.Tan(0.5f * this.fieldOfView);
        }
        public void GetScreenBounds(out float width, out float height)
        {
            height = (2 * MathF.Tan(0.5f * this.fieldOfView) * this.z);
            width = this.aspectRatio * height;
        }
        public void GetScreenBounds(out float left, out float right, out float bottom, out float top)
        {
            this.GetScreenBounds(out float width, out float height);

            left = this.position.X - (width * 0.5f);
            right = left + width;
            bottom = this.position.Y - (height * 0.5f);
            top = bottom + height;
        }
        public void GetScreenBounds(out Vector2 min, out Vector2 max)
        {
            this.GetScreenBounds(out float left, out float right, out float bottom, out float top);

            min = new Vector2(left, bottom);
            max = new Vector2(right, top);
        }

        public void MoveCam(Vector2 amount)
        {
            this.position += amount;
        }

        public void MoveTo(Vector2 position)
        {
            this.position = position;
        }

        public void MoveZ(float amount)
        {
            this.z = Math.Clamp(this.z + amount, minZ, maxZ);
        }

        public void ResetZ()
        {
            this.z = this.zBase;
        }
    }
}
