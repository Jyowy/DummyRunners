using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tools.LevelEditor
{

    public class Vertex : BaseElement
    {

        [SerializeField]
        private List<Edge> edges = new List<Edge>();

        public bool Check()
        {
            int prevEdges = edges.Count;
            for (int i = 0; i < edges.Count; ++i)
            {
                Edge edge = edges[i];
                if (edge == null
                    || edge.a == null
                    || edge.b == null)
                {
                    if (edge != null)
                    {
                        GameObject.DestroyImmediate(edge.gameObject);
                    }
                    edges.RemoveAt(i);
                    --i;
                }
            }
            return prevEdges != edges.Count;
        }

        public Vector2 Position
        {
            get => transform.position;
            set
            {
                Undo.RecordObject(transform, "Move");
                transform.position = value;
            }
        }

        public override Vector2 GetMeanPoint()
            => Position;

        public override void Move(Vector2 newPosition)
        {
            Undo.RecordObject(transform, "Move");
            Position = newPosition;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "Vertex", true, GizmoColor);
        }
#endif

        public List<Edge> GetEdges() => edges;

        public void AddEdge(Edge edge)
        {
            if (edges.Contains(edge))
                return;

            Undo.RecordObject(this, "AddEdge");
            edges.Add(edge);
        }

        public void RemoveEdge(Edge edge)
        {
            Undo.RecordObject(this, "RemoveEdge");
            edges.Remove(edge);
        }

        public bool HasEdgeWithVertex(Vertex other)
        {
            return edges.Find((edge) =>
                {
                    return (edge.a == this && edge.b == other)
                        || (edge.a == other && edge.b == this);
                }
            );
        }

    }

}
