using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Common;

namespace World.TimeAttacks
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Route))]
    public class RouteDrawer : Editor
    {

        private enum Mode
        {
            View,
            Move,
            Add,
            Delete
        }

        private Mode mode = Mode.View;
        private bool tryBestFit = true;

        private List<Vector2> storedPoints = new List<Vector2>();
        private List<Vector2> currentPoints = null;

        private static readonly float minDistance = 1.5f;
        private static readonly float minDistanceSq = minDistance * minDistance;

        public override void OnInspectorGUI()
        {
            var points = serializedObject.FindProperty("points");

            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(mode == Mode.View, "View", EditorStyles.miniButton))
            {
                mode = Mode.View;
            }
            if (GUILayout.Toggle(mode == Mode.Move, "Move", EditorStyles.miniButton))
            {
                mode = Mode.Move;
            }
            if (GUILayout.Toggle(mode == Mode.Add, "+", EditorStyles.miniButton))
            {
                mode = Mode.Add;
            }
            if (GUILayout.Toggle(mode == Mode.Delete, "-", EditorStyles.miniButton))
            {
                mode = Mode.Delete;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);

            if (mode == Mode.Add)
            {
                GUILayout.BeginHorizontal();

                tryBestFit = GUILayout.Toggle(tryBestFit, "Try best fit");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(10f);
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear"))
            {
                points.arraySize = 0;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);

            EditorGUILayout.PropertyField(points);

            serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
            Route route = (Route)target;

            Vector2 parentPosition = route.transform.position;
            var points = route.points;

            Color color = Color.yellow;
            color.a = 0.1f;
            Handles.color = color;

            for (int i = 0; i < points.Count; ++i)
            {
                Vector2 point = points[i] + parentPosition;

                if (mode == Mode.Move)
                {
                    points[i] = (Vector2)Handles.PositionHandle(point, Quaternion.identity)
                        - parentPosition;
                }
                else if (mode == Mode.Delete)
                {
                    Handles.DrawSolidArc(point, Vector3.back, Vector3.up, 360f, minDistance);
                }
                else
                {
                    Handles.DrawSolidArc(point, Vector3.back, Vector3.up, 360f, 0.25f);
                }
            }

            for (int i = 0; i < points.Count - 1; ++i)
            {
                Vector2 start = points[i] + parentPosition;
                Vector2 end = points[i + 1] + parentPosition;
                DebugUtils.DrawArrow(start, end, Color.yellow, 1.25f, false);
            }

            if (mode != Mode.Move
                && Event.current.type == EventType.MouseDown
                && Event.current.button == 0)
            {
                Vector2 mouse = Event.current.mousePosition;

                var ray = HandleUtility.GUIPointToWorldRay(mouse);
                Vector2 point = ray.origin;

                point -= parentPosition;

                if (mode == Mode.Add)
                {
                    AddPoint(points, point);
                }
                else if (mode == Mode.Delete)
                {
                    DeletePoint(points, point);
                }

                Event.current.Use();
            }
            else
            {
                CheckHotKeys(route);
            }

            currentPoints = points;
        }

        private void CheckHotKeys(Route route)
        {
            if (Event.current.type == EventType.KeyDown)
            {
                var key = Event.current.keyCode;
                if (key == KeyCode.U)
                {
                    route.points = new List<Vector2>(storedPoints);
                }
                else if (key == KeyCode.B)
                {
                    tryBestFit = !tryBestFit;
                }
                else if (key == KeyCode.Plus
                    || key == KeyCode.KeypadPlus
                    || key == KeyCode.A)
                {
                    mode = Mode.Add;
                }
                else if (key == KeyCode.Minus
                    || key == KeyCode.KeypadMinus
                    || key == KeyCode.Delete
                    || key == KeyCode.D)
                {
                    mode = Mode.Delete;
                }
                else if (key == KeyCode.M)
                {
                    mode = Mode.Move;
                }
                else if (key == KeyCode.Return
                    || key == KeyCode.KeypadEnter
                    || key == KeyCode.V)
                {
                    mode = Mode.View;
                }
                else
                {
                    return;
                }

                Event.current.Use();
                EditorUtility.SetDirty(target);
            }
        }

        private void AddPoint(List<Vector2> points, Vector2 point)
        {
            int index = points.Count;

            if (tryBestFit)
            {
                index = GetBestFitForNewPoint(points, point);
            }

            StorePoints();
            if (index < points.Count)
            {
                points.Insert(index, point);
            }
            else
            {
                points.Add(point);
            }
        }

        private int GetBestFitForNewPoint(List<Vector2> points, Vector2 point)
        {
            int count = points.Count;

            if (count <= 1)
            {
                return count;
            }

            for (int i = 0; i < points.Count - 1; ++i)
            {
                bool inRange = GeometryUtils.IsPointInRange(points[i], points[i + 1], point);
                if (inRange)
                {
                    float distance = GeometryUtils.GetDistanceFromPointToLine(points[i], points[i + 1], point);
                    if (distance < minDistance)
                    {
                        return i + 1;
                    }
                }
            }

            int closestIndex = 0;
            int secondClosestIndex = 0;

            float closestDistance = Mathf.Infinity;
            float secondClosestDistance = Mathf.Infinity;
            for (int i = 0; i < points.Count; ++i)
            {
                float distance = math.distancesq(points[i], point);
                if (distance < closestDistance)
                {
                    secondClosestDistance = closestDistance;
                    secondClosestIndex = closestIndex;

                    closestDistance = distance;
                    closestIndex = i;
                }
                else if (distance < secondClosestDistance)
                {
                    secondClosestDistance = closestDistance;
                    secondClosestIndex = closestIndex;
                }
            }

            int min = closestIndex;
            int max = min + 1;

            if (math.abs(closestIndex - secondClosestIndex) <= 1)
            {
                min = math.min(closestIndex, secondClosestIndex);
                max = min + 1;
            }
            else if (closestIndex == points.Count - 1)
            {
                max = closestIndex;
                min = max - 1;
            }

            Vector2 start = points[min];
            Vector2 end = points[max];
            Vector2 direction = (end - start).normalized;

            float startDot = math.dot(direction, (point - start).normalized);
            float endDot = math.dot(direction, (point - end).normalized);

            int index = max;
            if (startDot < 0f)
            {
                index = min;
            }
            else if (endDot >= 0f)
            {
                index = max + 1;
            }

            return index;
        }

        private void DeletePoint(List<Vector2> points, Vector2 point)
        {
            if (!IsPointCloseToAnyPoint(points, point, out int index))
                return;

            points.RemoveAt(index);
        }

        private bool IsPointCloseToAnyPoint(List<Vector2> points, Vector2 point, out int index)
        {
            index = -1;

            for (int i = 0; i < points.Count; ++i)
            {
                float distanceSq = math.distancesq(points[i], point);
                if (distanceSq <= minDistanceSq)
                {
                    index = i;
                    break;
                }
            }

            return index >= 0;
        }

        private void StorePoints()
            => storedPoints = new List<Vector2>(currentPoints);

    }

}