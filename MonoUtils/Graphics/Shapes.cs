using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoUtils.Graphics
{
    public sealed class Shapes : IDisposable
    {
        private Game game;
        private BasicEffect effect;
        private Camera camera;

        public enum FillMode
        {
            Filled,
            Border
        }

        private VertexPositionColor[] vertices;
        private int[] indices;

        private int shapeCount;
        private int vertexCount;
        private int indexCount;

        private bool isDisposed;
        private bool isBatchingStarted;

        public Shapes(Game game)
        {
            this.game = game ?? throw new ArgumentNullException("game");  // Error on null

            const int MaxVertexCount = 2048;
            const int MaxIndexCount = MaxVertexCount * 3;

            this.vertices = new VertexPositionColor[MaxVertexCount];
            this.indices = new int[MaxIndexCount];

            this.shapeCount = 0;
            this.vertexCount = 0;
            this.indexCount = 0;

            this.isDisposed = false;
            this.isBatchingStarted = false;

            this.effect = new BasicEffect(this.game.GraphicsDevice);
            this.effect.TextureEnabled = false;
            this.effect.FogEnabled = false;
            this.effect.LightingEnabled = false;
            this.effect.VertexColorEnabled = true;
            this.effect.World = Matrix.Identity;
            this.effect.View = Matrix.Identity;
            this.effect.Projection = Matrix.Identity;

        }

        public void Dispose()
        {
            if (isDisposed) { return; }

            this.effect?.Dispose();
            this.isDisposed = true;
        }

        public void Begin(Camera camera)
        {
            this.EnsureBatchingStarted(false);
            this.isBatchingStarted = true;

            if (camera is null)
            {
                Viewport vp = this.game.GraphicsDevice.Viewport;
                this.effect.View = Matrix.Identity;
                this.effect.Projection = Matrix.CreateOrthographicOffCenter(0, vp.Width, 0, vp.Height, 0f, 1f);
            }
            else
            {
                this.camera = camera;
                this.camera.updateMatrices();

                this.effect.View = this.camera.View;
                this.effect.Projection = this.camera.Projection;
            }
            this.isBatchingStarted = true;
        }

        public void End()
        {
            this.Flush();
            this.isBatchingStarted = false;
        }

        public void Flush()
        {
            if (shapeCount == 0) { return; }

            this.EnsureBatchingStarted(true);

            foreach(EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleList,
                    this.vertices,
                    0,
                    this.vertexCount,
                    indices,
                    0,
                    this.indexCount / 3);
            }

            this.shapeCount = 0;
            this.vertexCount = 0;
            this.indexCount = 0;
        }

        private void EnsureBatchingStarted(bool wantStarted)
        {
            if (wantStarted && !this.isBatchingStarted) { throw new Exception("Batching not started."); }
            else if (!wantStarted && this.isBatchingStarted) { throw new Exception("Batching already started."); }
        }

        private void EnsureSpace(int requiredVertices, int requiredIndices)
        {
            if (requiredVertices > vertices.Length) { throw new Exception($"Maximum vertex count is {vertices.Length}."); }
            if (requiredIndices > indices.Length) { throw new Exception($"Maximum index count is {indices.Length}."); }

            if ((vertices.Length - vertexCount) < requiredVertices || (indices.Length - indexCount) < requiredIndices)
            {
                Flush();
            }
        }

        /* ------------------------ RECTANGLE ------------------------ */
        /// <summary>
        /// Draw rectangle from 4 coordinates.
        /// </summary>
        public void DrawRectangle(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Color color, FillMode fill = FillMode.Filled, float thickness = 2f)
        {
            if (fill == FillMode.Filled)
            {
                EnsureBatchingStarted(true);
                EnsureSpace(requiredVertices: 4, requiredIndices: 6);

                indices[indexCount++] = vertexCount + 0;
                indices[indexCount++] = vertexCount + 1;
                indices[indexCount++] = vertexCount + 2;
                indices[indexCount++] = vertexCount + 0;
                indices[indexCount++] = vertexCount + 2;
                indices[indexCount++] = vertexCount + 3;

                vertices[vertexCount++] = new VertexPositionColor(a, color);
                vertices[vertexCount++] = new VertexPositionColor(b, color);
                vertices[vertexCount++] = new VertexPositionColor(c, color);
                vertices[vertexCount++] = new VertexPositionColor(d, color);

                this.shapeCount++;
            }
            else if (fill == FillMode.Border)
            {
                this.DrawLine(a, b, thickness, color);
                this.DrawLine(b, c, thickness, color);
                this.DrawLine(c, d, thickness, color);
                this.DrawLine(d, a, thickness, color);
            }
        }
        /// <summary>
        /// Draw rectangle from origin, width and height.
        /// </summary>
        public void DrawRectangle(float x, float y, float width, float height, Color color, FillMode fill = FillMode.Filled, float thickness = 2f)
        {
            float left = x;
            float right = x + width;
            float top = y + height;
            float bottom = y;

            if (fill == FillMode.Filled)
            {
                Vector3 a = new Vector3(left, top, 0f);
                Vector3 b = new Vector3(right, top, 0f);
                Vector3 c = new Vector3(right, bottom, 0f);
                Vector3 d = new Vector3(left, bottom, 0f);

                this.DrawRectangle(a, b, c, d, color);
            }
            else if (fill == FillMode.Border)
            {
                Vector2 a = new Vector2(left, top);
                Vector2 b = new Vector2(right, top);
                Vector2 c = new Vector2(right, bottom);
                Vector2 d = new Vector2(left, bottom);

                this.DrawLine(a, b, thickness, color);
                this.DrawLine(b, c, thickness, color);
                this.DrawLine(c, d, thickness, color);
                this.DrawLine(d, a, thickness, color);
            }
        }

        /* ------------------------ LINE ------------------------ */
        public void DrawLine(Vector2 a, Vector2 b, float thickness, Color color)
        {
            float halfThickness = (thickness / 2f);
            if (this.camera is not null)
            {
                halfThickness *= (this.camera.Z / this.camera.ZBase);
            }

            Vector2 e1 = b - a;
            e1.Normalize();
            e1 *= halfThickness;
            Vector2 e2 = -e1;
            Vector2 n1 = new Vector2(-e1.Y, e1.X);
            Vector2 n2 = -n1;

            Vector3 q1 = new Vector3(a + n1 + e2, 0f);
            Vector3 q2 = new Vector3(b + n1 + e1, 0f);
            Vector3 q3 = new Vector3(b + n2 + e1, 0f);
            Vector3 q4 = new Vector3(a + n2 + e2, 0f);

            this.DrawRectangle(q1, q2, q3, q4, color);
        }
        public void DrawLine(Vector3 a, Vector3 b, float thickness, Color color)
        {
            float halfThickness = (thickness / 2f);
            if (this.camera is not null)
            {
                halfThickness *= camera.ZoomFactor;
            }

            Vector3 e1 = b - a;
            e1.Normalize();
            e1 *= halfThickness;
            Vector3 e2 = -e1;
            Vector3 n1 = new Vector3(-e1.Y, e1.X, 0f);
            Vector3 n2 = -n1;

            Vector3 q1 = a + n1 + e2;
            Vector3 q2 = b + n1 + e1;
            Vector3 q3 = b + n2 + e1;
            Vector3 q4 = a + n2 + e2;

            this.DrawRectangle(q1, q2, q3, q4, color);
        }

        /* ------------------------ REGULAR POLYGON ------------------------ */
        public void DrawRegularPolygon(Vector2 origin, int sides, float size, Color color, FillMode fill = FillMode.Filled, float thickness = 2f)
        {
            if (sides < 3) { throw new Exception("A polygon needs a minimum of 3 sides."); }

            EnsureBatchingStarted(true);
            EnsureSpace(requiredVertices: sides + 1, requiredIndices: sides * 3);

            float angle = MathHelper.TwoPi / sides;

            Vector3 initialVectorPoint = new Vector3(origin.X, origin.Y + size, 0f);
            Vector3 origin3 = new Vector3(origin.X, origin.Y, 0f);

            int originIndex = vertexCount;

            if (fill == FillMode.Filled)
            {
                vertices[vertexCount++] = new VertexPositionColor(origin3, color);
                vertices[vertexCount++] = new VertexPositionColor(initialVectorPoint, color);

                for (int i = 1; i < sides; i++)
                {
                    indices[indexCount++] = originIndex;
                    indices[indexCount++] = originIndex + i;
                    indices[indexCount++] = originIndex + i + 1;

                    vertices[vertexCount++] = new VertexPositionColor(Util.Rotate(initialVectorPoint, origin3, -angle * i), color);
                }
                indices[indexCount++] = originIndex;
                indices[indexCount++] = originIndex + sides;
                indices[indexCount++] = originIndex + 1;

                this.shapeCount++;
            }
            else if (fill == FillMode.Border)
            {
                Vector3 previous = initialVectorPoint;
                Vector3 current = new Vector3(0, 0, 0);  // empty definition

                for (int i = 1; i < sides; i++)
                {
                    current = Util.Rotate(initialVectorPoint, origin3, -angle * i);
                    DrawLine(previous, current, thickness, color);
                    previous = current;
                }
                DrawLine(current, initialVectorPoint, thickness, color);
            }
        }

        /* ------------------------ CIRCLE ------------------------ */
        public void DrawCircle(Vector2 origin, float radius, Color color, FillMode fill = FillMode.Filled, float thickness = 2f)
        {
            int precision = (int)Math.Round(Math.Clamp(1f / camera.ZoomFactor * radius, 4f, 190f));

            if (fill == FillMode.Filled)
            {
                DrawRegularPolygon(origin, precision, radius, color);
            }
            else if (fill == FillMode.Border)
            {
                DrawRegularPolygon(origin, precision, radius, color, fill, thickness);
            }
        }

        /* ------------------------ POLYGON ------------------------ */
        public void DrawPolygon(Vector2[] points, Color color, FillMode fill = FillMode.Filled, float thickness = 2f)
        {
            if (points.Length < 3) { throw new Exception("A polygon needs a minimum of 3 points."); }

            if (fill == FillMode.Filled)
            {

            }
            else if (fill == FillMode.Border)
            {
                Vector2 previous = points[0];
                Vector2 current = new Vector2(0f, 0f);  // initializing empty vector
                for (int i = 1; i < points.Length; i++)
                {
                    current = points[i];
                    this.DrawLine(previous, current, thickness, color);
                    previous = current;
                }
                this.DrawLine(current, points[0], thickness, color);
            }
        }
    }
}
