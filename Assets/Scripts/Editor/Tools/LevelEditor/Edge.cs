using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Tools.LevelEditor
{

    public class Edge : BaseElement
    {

        public Vertex a = null;
        public Vertex b = null;

        public bool bezier = false;

        [System.Serializable]
        public class BezierAdjuster
        {
            public Vector2 direction = Vector2.zero;
            public float strength = 1f;
        }

        public BezierAdjuster a_puller = new BezierAdjuster();
        public BezierAdjuster b_puller = new BezierAdjuster();

        public bool Check()
        {
            bool destroyed = false;

            if (a == null
                || b == null)
            {
                if (a != null)
                    a.RemoveEdge(this);
                if (b != null)
                    b.RemoveEdge(this);

                GameObject.DestroyImmediate(gameObject);
                destroyed = true;
            }

            return destroyed;
        }

        public override Vector2 GetMeanPoint()
            => (a.Position + b.Position) * 0.5f;

        public override void Move(Vector2 newPosition)
        {
            Vector2 deltaMove = newPosition - GetMeanPoint();
            a.Move(a.Position + deltaMove);
            b.Move(b.Position + deltaMove);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!bezier)
            {
                Handles.color = GizmoColor;
                Handles.DrawLine(a.Position, b.Position);
            }
            else
            {
                Handles.DrawBezier(
                    a.Position,
                    b.Position,
                    a_puller.direction * a_puller.strength,
                    b_puller.direction * b_puller.strength,
                    GizmoColor,
                    EditorGUIUtility.whiteTexture,
                    1f
                );

                Color color = GizmoColor;
                color.a = 0.5f;
                Handles.color = color;
                float size = 5f;
                float space = size * 0.5f;
                float accSize = 0f;
                float totalSize = math.distance(a.Position, b.Position);
                Vector2 direction = (b.Position - a.Position).normalized;
                Vector2 startPoint = a.Position;
                while (accSize < totalSize)
                {
                    Handles.DrawLine(startPoint, startPoint + direction * math.min(size, totalSize - accSize));
                    float distance = size + space;
                    startPoint += direction * distance;

                    accSize += distance;
                }
            }
        }
#endif

        public void SetVertexes(Vertex a, Vertex b)
        {
            if (a == b
                || a == null
                || b == null)
                return;

            this.a = a;
            this.b = b;

            a.AddEdge(this);
            b.AddEdge(this);
        }


    }

}