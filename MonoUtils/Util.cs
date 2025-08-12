using System;
using Microsoft.Xna.Framework;

namespace MonoUtils
{
    public static class Util
    {
        public static Vector3 Rotate(Vector3 pointToRotate, Vector3 pivotPoint, float radians)
        {
            float cos = MathF.Cos(radians);
            float sin = MathF.Sin(radians);

            Vector3 translatedPoint = pointToRotate - pivotPoint;

            float newX = (translatedPoint.X * cos) - (translatedPoint.Y * sin);
            float newY = (translatedPoint.X * sin) + (translatedPoint.Y * cos);

            return new Vector3(newX, newY, 0f) + pivotPoint;
        }
    }
}
