using UnityEngine;
using Unity.Mathematics;

namespace Common
{

    public static class GeometryUtils
    {

        private readonly static float PI_2 = math.PI * 0.5f;

        public static Vector2 GetNormal(Vector2 vector, bool clockwise)
        {
            return Rotate(vector, clockwise ? -PI_2 : PI_2, false);
        }

        public static Vector2 Rotate(Vector2 vector, float angle, bool degrees = true)
        {
            if (angle == 0f
                || vector == Vector2.zero)
                return vector;

            float rad = degrees ? math.radians(angle) : angle;
            float cos = math.cos(rad);
            float sin = math.sin(rad);
            float2x2 transform = new float2x2(cos, -sin, sin, cos);

            return math.mul(vector, transform);
        }

        public static bool IsPointInRange(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
        {
            return math.dot((lineEnd - lineStart), (point - lineStart)) >= 0f
                && math.dot((lineStart - lineEnd), (point - lineEnd)) >= 0f;
        }

        public static float GetDistanceFromPointToLine(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
        {
            if (lineStart.x == lineEnd.x)
            {
                Vector2 minY = lineStart.y < lineEnd.y
                    ? lineStart
                    : lineEnd;
                Vector2 maxY = lineStart.y < lineEnd.y
                    ? lineEnd : lineStart;

                Vector2 closestPoint = point;
                closestPoint.x = lineStart.x;
                if (point.y < minY.y)
                    closestPoint.y = minY.y;
                else if (point.y > maxY.y)
                    closestPoint.y = maxY.y;

                return math.distance(closestPoint, point);
            }

            // Get f(x) = m*x + n coefficients
            Vector2 line = lineEnd - lineStart;
            float m1 = line.y / line.x;
            float n1 = lineStart.y - lineStart.x * m1;

            // Get normal g(x) coefficients
            Vector2 normal = GetNormal(line, true);
            float m2 = normal.y / normal.x;
            float n2 = point.y - point.x * m2;

            // Get f(x) and g(x) crossing point
            float x0 = (n2 - n1) / (m1 - m2);
            float y0 = m1 * x0 + n1;
            Vector2 crossPoint = new Vector2(x0, y0);

            Vector2 minX = lineStart.x < lineEnd.x
                ? lineStart
                : lineEnd;
            Vector2 maxX = lineStart.x < lineEnd.x
                ? lineEnd : lineStart;
            if (crossPoint.x < minX.x)
                crossPoint = minX;
            else if (crossPoint.x > maxX.x)
                crossPoint = maxX;

            return math.distance(point, crossPoint);
        }

    }

}