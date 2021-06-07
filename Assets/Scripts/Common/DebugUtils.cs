#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Common
{

    public static class DebugUtils
    {

        public static void DrawArrow(Vector2 start, Vector2 end, Color color, float capSize = 1f, bool bothCaps = false)
        {
            Handles.color = color;
            Handles.DrawLine(start, end);

            float angle = 15f;

            Vector2 direction = (end - start).normalized;
            Vector2 left = end + GeometryUtils.Rotate(-direction, angle) * capSize;
            Vector2 right = end + GeometryUtils.Rotate(-direction, -angle) * capSize;
            Handles.DrawLine(left, end);
            Handles.DrawLine(right, end);

            if (bothCaps)
            {
                left = start + GeometryUtils.Rotate(direction, angle) * capSize;
                right = start + GeometryUtils.Rotate(direction, -angle) * capSize;
                Handles.DrawLine(start, left);
                Handles.DrawLine(start, right);
            }
        }

        public static Vector2 Get2DWorldPointFromMouse(SceneView scene, Vector2 mousePosition)
        {
            Vector2 sceneSize;
            sceneSize.x = scene.position.width - 1f;
            sceneSize.y = scene.position.height - 20f;

            mousePosition.y = sceneSize.y - mousePosition.y;

            Vector2 positionInScreen;
            positionInScreen.x = mousePosition.x / sceneSize.x - 0.5f;
            positionInScreen.y = mousePosition.y / sceneSize.y - 0.5f;

            Vector2 cameraPosition = scene.camera.transform.position;
            Vector2 cameraRealSize;
            cameraRealSize.y = scene.camera.orthographicSize * 2f;
            cameraRealSize.x = cameraRealSize.y * scene.camera.aspect;

            Vector2 worldPosition;
            worldPosition.x = cameraPosition.x + positionInScreen.x * cameraRealSize.x;
            worldPosition.y = cameraPosition.y + positionInScreen.y * cameraRealSize.y;

            return worldPosition;
        }

    }

}

#endif