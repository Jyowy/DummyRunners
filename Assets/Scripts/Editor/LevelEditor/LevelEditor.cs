using Common;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tools.LevelEditor
{

    public class LevelEditor : EditorWindow
    {

        private enum EditTarget
        {
            Vertex,
            Edge,
            Face,
            Object
        }

        static public void UnityMenuShow()
        {
            var window = GetWindow<LevelEditor>();

            if (window != null)
            {
                window.Show();
                window.titleContent = new GUIContent("Level Editor");
            }
        }

        private void OnEnable()
        {
            SceneView.beforeSceneGui += OnSceneGUI;
            EditorSceneManager.activeSceneChangedInEditMode += OnSceneChanged;
            ReadSceneData();
        }

        private void OnDisable()
        {
            SceneView.beforeSceneGui -= OnSceneGUI;
            EditorSceneManager.activeSceneChangedInEditMode -= OnSceneChanged;
        }

        private void OnSceneChanged(Scene a, Scene b)
        {
            ReadSceneData();
        }

        private void ReadSceneData()
        {
            Scene scene = EditorSceneManager.GetActiveScene();

            var roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; ++i)
            {
                if (roots[i].name.Equals("OrphanRoot"))
                {
                    orphanRoot = roots[i].transform;
                }
            }
            if (orphanRoot == null)
            {
                var newObject = new GameObject("OrphanRoot");
                orphanRoot = newObject.transform;
            }

            orphanVertexesRoot = CheckIfRootExistsAndCreateIfDoesnt(orphanRoot, "OrphanVertexes");
            orphanEdgesRoot = CheckIfRootExistsAndCreateIfDoesnt(orphanRoot, "OrphanEdges");

            this.orphanVertexes.Clear();
            var orphanVertexes = orphanVertexesRoot.GetComponentsInChildren<Vertex>();
            this.orphanVertexes.AddRange(orphanVertexes);
            this.orphanVertexes.ForEach((vertex) => vertex.Deselect());

            this.orphanEdges.Clear();
            var orphanEdges = orphanEdgesRoot.GetComponentsInChildren<Edge>();
            this.orphanEdges.AddRange(orphanEdges);
            this.orphanEdges.ForEach((edge) => edge.Deselect());

            CheckData();
        }

        private Transform CheckIfRootExistsAndCreateIfDoesnt(Transform parent, string rootName)
        {
            Transform root = null;

            for (int i = 0; i < parent.childCount; ++i)
            {
                var child = parent.GetChild(i);
                if (child.name.Equals(rootName))
                {
                    root = child.transform;
                    break;
                }
            }

            if (root == null)
            {
                var newObject = new GameObject(rootName);
                newObject.transform.SetParent(parent);
                root = newObject.transform;
            }

            return root;
        }

        private EditTarget target = EditTarget.Vertex;

        private const float buttonSize = 30f;
        private readonly GUILayoutOption[] modeButtons = new GUILayoutOption[] { GUILayout.Width(buttonSize), GUILayout.Height(buttonSize) };

        private Transform orphanRoot = null;
        private Transform orphanVertexesRoot = null;
        private Transform orphanEdgesRoot = null;

        private readonly List<Vertex> orphanVertexes = new List<Vertex>();
        private readonly List<Edge> orphanEdges = new List<Edge>();
        private readonly List<Face> orphanFaces = new List<Face>();
        private readonly List<Object> objects = new List<Object>();

        private float selectRadius = 2f;
        private float radiusIncrement = 0.5f;

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnGUI()
        {
            CheckData();

            DrawTarget();
            DrawEditMode();
            DrawCustomTargetMenu();
            DrawHierarchy();
        }

        private void CheckData()
        {
            bool dirty = false;

            for (int i = 0; i < orphanVertexes.Count; ++i)
            {
                if (orphanVertexes[i] == null)
                {
                    orphanVertexes.RemoveAt(i);
                    --i;
                    dirty = true;
                }
                else
                {
                    dirty |= orphanVertexes[i].Check();
                }
            }

            for (int i = 0; i < orphanEdges.Count; ++i)
            {
                if (orphanEdges[i] == null
                    || orphanEdges[i].Check())
                {
                    orphanEdges.RemoveAt(i);
                    --i;
                    dirty = true;
                }
            }

            if (dirty)
            {
                Repaint();
            }
        }

        private enum EditMode
        {
            Select,
            Move,
            Add
        }

        private void DrawTarget()
        {
            EditorGUILayout.Space(5f);
            EditorGUILayout.LabelField("Target", EditorStyles.boldLabel);
            EditorGUILayout.Space(5f);

            var vertexButton = new GUIContent(EditorGUIUtility.IconContent("Assets/Icons/Vertex.png").image, "Vertex (V)");
            var edgeButton = new GUIContent(EditorGUIUtility.IconContent("Assets/Icons/Edge.png").image, "Edge (E)");
            var faceButton = new GUIContent(EditorGUIUtility.IconContent("Assets/Icons/Face.png").image, "Face (F)");
            var objectButton = new GUIContent(EditorGUIUtility.IconContent("Assets/Icons/Object.png").image, "Object (O)");

            EditTarget prevTarget = target;

            EditorGUILayout.BeginHorizontal();

            GUI.enabled = target != EditTarget.Vertex;
            bool vertexTarget = GUILayout.Toggle(target == EditTarget.Vertex, vertexButton, EditorStyles.helpBox, modeButtons);
            if (vertexTarget && target != EditTarget.Vertex)
            {
                target = EditTarget.Vertex;
            }

            GUI.enabled = target != EditTarget.Edge;
            bool edgeTarget = GUILayout.Toggle(target == EditTarget.Edge, edgeButton, EditorStyles.helpBox, modeButtons);
            if (edgeTarget && target != EditTarget.Edge)
            {
                target = EditTarget.Edge;
            }

            GUI.enabled = target != EditTarget.Face;
            bool faceTarget = GUILayout.Toggle(target == EditTarget.Face, faceButton, EditorStyles.helpBox, modeButtons);
            if (faceTarget && target != EditTarget.Face)
            {
                target = EditTarget.Face;
            }

            GUI.enabled = target != EditTarget.Object;
            bool objectTarget = GUILayout.Toggle(target == EditTarget.Object, objectButton, EditorStyles.helpBox, modeButtons);
            if (objectTarget && target != EditTarget.Object)
            {
                target = EditTarget.Object;
            }

            GUI.enabled = true;
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            CheckTargetHotKeys();

            if (target != prevTarget)
            {
                ClearSelection();
            }
        }

        private void CheckTargetHotKeys()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                bool use = true;
                EditTarget prevTarget = target;

                KeyCode key = Event.current.keyCode;
                if (key == KeyCode.V)
                {
                    target = EditTarget.Vertex;
                }
                else if (key == KeyCode.E)
                {
                    target = EditTarget.Edge;
                }
                else if (key == KeyCode.F)
                {
                    target = EditTarget.Face;
                }
                else if (key == KeyCode.O)
                {
                    target = EditTarget.Object;
                }
                else
                {
                    use = false;
                }

                if (use)
                {
                    if (target != prevTarget)
                    {
                        ClearSelection();
                    }

                    Event.current.Use();
                    Repaint();
                }
            }
        }

        private EditMode editMode = EditMode.Select;

        private void DrawEditMode()
        {
            EditorGUILayout.Space(5f);
            EditorGUILayout.LabelField("Edit Mode", EditorStyles.boldLabel);
            EditorGUILayout.Space(5f);

            var selectButton = new GUIContent(EditorGUIUtility.IconContent("Assets/Icons/Select.png").image, "Select (S)");
            var moveButton = new GUIContent(EditorGUIUtility.IconContent("Assets/Icons/Move.png").image, "Move (W)");
            var addButton = new GUIContent(EditorGUIUtility.IconContent("Assets/Icons/Plus.png").image, "Add (A)");

            EditorGUILayout.BeginHorizontal();

            GUI.enabled = editMode != EditMode.Select;
            bool selectMode = GUILayout.Toggle(editMode == EditMode.Select, selectButton, EditorStyles.helpBox, modeButtons);
            if (selectMode && editMode != EditMode.Select)
            {
                editMode = EditMode.Select;
            }

            GUI.enabled = editMode != EditMode.Move;
            bool moveMode = GUILayout.Toggle(editMode == EditMode.Move, moveButton, EditorStyles.helpBox, modeButtons);
            if (moveMode && editMode != EditMode.Move)
            {
                editMode = EditMode.Move;
            }

            GUI.enabled = editMode != EditMode.Add;
            bool addMode = GUILayout.Toggle(editMode == EditMode.Add, addButton, EditorStyles.helpBox, modeButtons);
            if (addMode && editMode != EditMode.Add)
            {
                editMode = EditMode.Add;
            }

            GUI.enabled = true;
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            if (editMode == EditMode.Select)
            {
                EditorGUILayout.BeginHorizontal();
                float prevSelectRadius = selectRadius;
                selectRadius = EditorGUILayout.FloatField("Select Radius", selectRadius);
                selectRadius = math.max(selectRadius, 0f);
                if (selectRadius != prevSelectRadius)
                {
                    SceneView.RepaintAll();
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            CheckEditModeHotKeys();
        }

        private void CheckEditModeHotKeys()
        {
            bool use = false;

            if (Event.current.type == EventType.KeyDown
                && !Event.current.control)
            {
                use = true;

                KeyCode key = Event.current.keyCode;
                if (key == KeyCode.S)
                {
                    editMode = EditMode.Select;
                }
                else if (key == KeyCode.W)
                {
                    editMode = EditMode.Move;
                }
                else if (key == KeyCode.A)
                {
                    editMode = EditMode.Add;
                }
                else
                {
                    use = false;
                }
            }
            else if (Event.current.type == EventType.ScrollWheel
                && Event.current.shift)
            {
                use = true;
                selectRadius = math.max(selectRadius + radiusIncrement * math.sign(Event.current.delta.y), 0f);
            }

            if (use)
            {
                Event.current.Use();
                Repaint();
            }
        }

        private void DrawCustomTargetMenu()
        {
            if (target == EditTarget.Vertex)
            {
                DrawVertexMenu();
            }

            CheckCustomTargetHotKeys();
        }

        private void CheckCustomTargetHotKeys()
        {
            bool use = false;

            if (Event.current.type == EventType.KeyDown
                && !Event.current.control)
            {
                KeyCode key = Event.current.keyCode;
                if (target == EditTarget.Vertex)
                {
                    if (key == KeyCode.L)
                    {
                        use = true;
                        ConnectSelectedVertexes();
                    }
                }

                if (Event.current.shift
                    && (key == KeyCode.UpArrow
                        || key == KeyCode.DownArrow))
                {
                    Debug.LogFormat("Align Horizontally HotKey");
                    use = true;
                    AlignSelectionHorizontally();
                }
                if (Event.current.shift
                    && (key == KeyCode.LeftArrow
                        || key == KeyCode.RightArrow))
                {
                    Debug.LogFormat("Align Vertically HotKey");
                    use = true;
                    AlignSelectionVertically();
                }
            }

            if (use)
            {
                Event.current.Use();
                Repaint();
            }
        }

        private void DrawVertexMenu()
        {
            EditorGUILayout.Space(5f);
            EditorGUILayout.LabelField("Vertex menu", EditorStyles.boldLabel);
            EditorGUILayout.Space(5f);

            var linkButton = new GUIContent(EditorGUIUtility.IconContent("Assets/Icons/Link.png").image, "Link (L)");
            var horizontalAlignButton = new GUIContent(EditorGUIUtility.IconContent("Assets/Icons/HorizontalAlign.png").image, "Horizontal Align (Alt+H)");
            var verticalAlignButton = new GUIContent(EditorGUIUtility.IconContent("Assets/Icons/VerticalAlign.png").image, "Vertical Align (Alt+V)");

            EditorGUILayout.BeginHorizontal();

            GUI.enabled = selection.Count >= 2;
            if (GUILayout.Button(linkButton, modeButtons))
            {
                ConnectSelectedVertexes();
            }
            if (GUILayout.Button(horizontalAlignButton, modeButtons))
            {
                AlignSelectionHorizontally();
            }
            if (GUILayout.Button(verticalAlignButton, modeButtons))
            {
                AlignSelectionVertically();
            }
            GUI.enabled = true;

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void ConnectSelectedVertexes()
        {
            if (target != EditTarget.Vertex
                || selection.Count < 2)
                return;

            for (int i = 0; i < selection.Count - 1; ++i)
            {
                if (!selection[i].TryGetComponent(out Vertex a))
                    continue;

                for (int j = i + 1; j < selection.Count; ++j)
                {
                    if (selection[j].TryGetComponent(out Vertex b)
                        && !a.HasEdgeWithVertex(b))
                    {
                        AddOrphanEdge(a, b);
                    }
                }
            }

            SceneView.RepaintAll();
        }

        private void AlignSelectionHorizontally()
        {
            if (selection.Count < 2)
                return;

            Vector2 referencePoint = selection[0].GetComponent<BaseElement>().GetMeanPoint();
            
            for (int i = 1; i < selection.Count; ++i)
            {
                if (!selection[i].TryGetComponent(out BaseElement baseElement))
                    continue;

                Vector2 position = baseElement.GetMeanPoint();
                position.y = referencePoint.y;
                baseElement.Move(position);
            }
        }

        private void AlignSelectionVertically()
        {
            if (selection.Count < 2)
                return;

            Vector2 referencePoint = selection[0].GetComponent<BaseElement>().GetMeanPoint();

            for (int i = 1; i < selection.Count; ++i)
            {
                if (!selection[i].TryGetComponent(out BaseElement baseElement))
                    continue;

                Vector2 position = baseElement.GetMeanPoint();
                position.x = referencePoint.x;
                baseElement.Move(position);
            }
        }

        private Vector2 orphanVertexesScroll = Vector2.zero;

        private void DrawHierarchy()
        {
            bool dirty = false;

            EditorGUILayout.Space(5f);
            EditorGUILayout.LabelField("Hierarchy", EditorStyles.boldLabel);
            EditorGUILayout.Space(5f);

            dirty |= DrawOrphanVertexes();

            EditorGUILayout.LabelField("Objects", EditorStyles.boldLabel);

            if (dirty)
            {
                SceneView.RepaintAll();
            }
        }

        private bool orphanVertexesExpanded = false;

        private bool DrawOrphanVertexes()
        {
            bool dirty = false;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            orphanVertexesExpanded = EditorGUILayout.Foldout(orphanVertexesExpanded, "Orphan Vertexes", true);
            EditorGUILayout.EndHorizontal();

            if (orphanVertexesExpanded)
            {
                EditorGUILayout.Space(5f);
                EditorGUI.indentLevel++;
                EditorGUIUtility.labelWidth = 50f;

                const float maxHeight = 250f;
                float scrollHeight = math.min(orphanVertexes.Count * 22.5f, maxHeight);
                orphanVertexesScroll = EditorGUILayout.BeginScrollView(orphanVertexesScroll, GUILayout.MaxHeight(scrollHeight));

                for (int i = 0; i < orphanVertexes.Count; ++i)
                {
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Space(EditorGUI.indentLevel * 25f);

                    Vertex vertex = orphanVertexes[i];
                    bool select = GUILayout.Button((i + 1).ToString(), GUILayout.Width(35f));
                    Vector2 position = vertex.Position;
                    position.x = EditorGUILayout.FloatField("x", vertex.Position.x, GUILayout.Width(100f));
                    position.y = EditorGUILayout.FloatField("y", vertex.Position.y, GUILayout.Width(100f));
                    vertex.Position = position;

                    dirty |= !vertex.Equals(orphanVertexes[i]);

                    if (select)
                    {
                        SelectOrphanVertex(i);
                    }

                    orphanVertexes[i] = vertex;

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;

                EditorGUILayout.EndScrollView();

                if (orphanVertexes.Count > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(115f);
                    GUI.color = Color.red;
                    bool deleteAll = GUILayout.Button("Delete all");
                    GUI.color = Color.white;
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    if (deleteAll)
                    {
                        DeleteAllOrphanVertexes();
                        dirty = true;
                    }
                }
            }

            return dirty;
        }

        private void DeleteAllOrphanVertexes()
        {
            orphanVertexes.ForEach(
                (vertex) =>
                {
                    GameObject.DestroyImmediate(vertex.gameObject);
                }
            );
            orphanVertexes.Clear();
        }

        private bool selectDragging = false;

        private void OnSceneGUI(SceneView scene)
        {
            Vector2 worldPosition = DebugUtils.Get2DWorldPointFromMouse(scene, Event.current.mousePosition);

            CheckData();
            CheckTargetHotKeys();
            CheckEditModeHotKeys();
            CheckCustomTargetHotKeys();

            if (editMode == EditMode.Select)
            {
                Handles.color = Color.white;
                Handles.DrawWireArc(worldPosition, Vector3.back, Vector3.up, 360f, selectRadius);
            }
            else if (editMode == EditMode.Move)
            {
                EditorGUIUtility.AddCursorRect(new Rect(Vector2.zero, scene.position.size), MouseCursor.MoveArrow);
            }
            else if (editMode == EditMode.Add)
            {
                EditorGUIUtility.AddCursorRect(new Rect(Vector2.zero, scene.position.size), MouseCursor.ArrowPlus);
            }

            scene.Repaint();

            if (Event.current.isMouse)
            {
                bool use = UpdateEditModeInput(worldPosition);

                if (use)
                {
                    Event.current.Use();
                }
            }
        }

        private bool moving = false;
        private readonly List<Vector2> selectionStartPositions = new List<Vector2>();
        private Vector2 moveStartPoint = Vector2.zero;

        private bool UpdateEditModeInput(Vector2 worldPosition)
        {
            bool use = false;

            if (editMode == EditMode.Add
                && Event.current.type == EventType.MouseDown
                && Event.current.button == 0)
            {
                AddOrphanVertex(worldPosition, target != EditTarget.Vertex);
                use = true;
            }
            else if (editMode == EditMode.Select
                && ((selectDragging && Event.current.type == EventType.MouseDrag)
                    || (Event.current.type == EventType.MouseDown && Event.current.button == 0)))
            {
                bool addToSelection = Event.current.type == EventType.MouseDrag
                    || Event.current.shift;
                if (target == EditTarget.Vertex)
                {
                    SelectAllOrphanVertexesInside(worldPosition, addToSelection);
                }
                else if (target == EditTarget.Edge)
                {
                    SelectAllOrphanEdgesInside(worldPosition, addToSelection);
                }
                use = true;
                selectDragging = true;
            }
            else if (editMode == EditMode.Select
                && Event.current.type == EventType.MouseUp
                && selectDragging)
            {
                selectDragging = false;
                use = true;
            }
            else if (editMode == EditMode.Move)
            {
                if (selection.Count == 0)
                {
                    editMode = EditMode.Select;
                }
                else
                {
                    if (Event.current.isMouse)
                    {
                        if (Event.current.type == EventType.MouseDown
                            && Event.current.button == 0)
                        {
                            moving = true;
                            moveStartPoint = worldPosition;
                            GetSelectionStartPositions();
                            use = true;
                        }
                        else if (moving
                            && Event.current.type == EventType.MouseDrag)
                        {
                            Vector2 newPosition = worldPosition;
                            if (Event.current.shift)
                            {
                                if (math.abs(newPosition.x - moveStartPoint.x)
                                    > math.abs(newPosition.y - moveStartPoint.y))
                                    newPosition.y = moveStartPoint.y;
                                else
                                    newPosition.x = moveStartPoint.x;
                            }
                            Vector2 deltaMove = newPosition - moveStartPoint;
                            MoveSelection(deltaMove);
                            use = true;
                        }
                        else if (Event.current.type == EventType.MouseUp)
                        {
                            moveStartPoint = Vector2.zero;
                            moving = false;
                            SceneModified();

                            use = true;
                        }
                    }
                }
            }

            return use;
        }

        private void GetSelectionStartPositions()
        {
            selectionStartPositions.Clear();
            for (int i = 0; i < selection.Count; ++i)
            {
                if (!selection[i].TryGetComponent(out BaseElement baseElement))
                    continue;

                if (baseElement is Vertex vertex)
                {
                    selectionStartPositions.Add(vertex.Position);
                }
                else if (baseElement is Edge edge)
                {
                    selectionStartPositions.Add(edge.a.Position);
                    selectionStartPositions.Add(edge.b.Position);
                }
            }
        }

        private void MoveSelection(Vector2 deltaMove)
        {
            int selectionIndex = 0;
            for (int i = 0; i < selection.Count; ++i)
            {
                if (!selection[i].TryGetComponent(out BaseElement baseElement))
                    continue;

                if (baseElement is Vertex vertex)
                {
                    vertex.Move(selectionStartPositions[selectionIndex] + deltaMove);
                    selectionIndex++;
                }
                else if (baseElement is Edge edge)
                {
                    edge.a.Move(selectionStartPositions[selectionIndex] + deltaMove);
                    selectionIndex++;
                    edge.b.Move(selectionStartPositions[selectionIndex] + deltaMove);
                    selectionIndex++;
                }
            }
        }

        private Vertex lastVertex = null;

        private void AddOrphanVertex(Vector2 worldPosition, bool createEdges)
        {
            Undo.RegisterFullObjectHierarchyUndo(orphanRoot, "RootChange");

            string name = string.Format("Vertex {0}", orphanVertexes.Count + 1);
            GameObject newObject = new GameObject(name, typeof(Vertex))
            {
                tag = "LevelEditor"
            };
            newObject.transform.SetParent(orphanVertexesRoot);
            Vertex newVertex = newObject.GetComponent<Vertex>();
            newVertex.Position = worldPosition;
            orphanVertexes.Add(newVertex);

            if (createEdges
                && lastVertex != null)
            {
                AddOrphanEdge(lastVertex, newVertex);
            }

            lastVertex = newVertex;

            Repaint();
            SceneModified();
        }

        private void AddOrphanEdge(Vertex a, Vertex b)
        {
            if (a == b
                || a.HasEdgeWithVertex(b))
                return;

            Undo.RegisterFullObjectHierarchyUndo(orphanRoot, "RootChange");
            string name = string.Format("Edge {0}", orphanEdges.Count + 1);
            GameObject newObject = new GameObject(name, typeof(Edge))
            {
                tag = "LevelEditor"
            };
            newObject.transform.SetParent(orphanEdgesRoot);
            Edge newEdge = newObject.GetComponent<Edge>();
            newEdge.SetVertexes(a, b);
            orphanEdges.Add(newEdge);

            SceneModified();
        }

        private void SelectOrphanVertex(int index)
        {
            if (index < 0
                || index >= orphanVertexes.Count)
                return;

            ClearSelection();
            Selection.activeObject = orphanVertexes[index].gameObject;
            editMode = EditMode.Move;
            SceneView.lastActiveSceneView.Focus();
        }

        private readonly List<GameObject> selection = new List<GameObject>();

        private void SelectAllOrphanVertexesInside(Vector2 worldPosition, bool addToSelection)
        {
            if (!addToSelection)
            {
                ClearSelection();
            }    

            float radiussq = selectRadius * selectRadius;
            for (int i = 0; i < orphanVertexes.Count; ++i)
            {
                GameObject gameObject = orphanVertexes[i].gameObject;
                float distancesq = math.distancesq(worldPosition, orphanVertexes[i].Position);
                if (distancesq < radiussq
                    && !selection.Contains(gameObject))
                {
                    selection.Add(gameObject);
                }
            }

            Selection.objects = selection.ToArray();
            OnSelectionUpdated();
        }

        private void SelectAllOrphanEdgesInside(Vector2 worldPosition, bool addToSelection)
        {
            if (!addToSelection)
            {
                ClearSelection();
            }

            for (int i = 0; i < orphanEdges.Count; ++i)
            {
                GameObject gameObject = orphanEdges[i].gameObject;
                float distance = GeometryUtils.GetDistanceFromPointToLine(orphanEdges[i].a.Position, orphanEdges[i].b.Position, worldPosition);
                if (distance < selectRadius
                    && !selection.Contains(gameObject))
                {
                    selection.Add(gameObject);
                }
            }

            Selection.objects = selection.ToArray();
            OnSelectionUpdated();
        }

        private void OnSelectionChange()
        {
            ClearSelection();
            
            foreach(GameObject gameObject in Selection.gameObjects)
            {
                if (!gameObject.TryGetComponent(out BaseElement _))
                    continue;

                selection.Add(gameObject);
            }

            OnSelectionUpdated();
        }

        private void ClearSelection()
        {
            selection.ForEach((gameObject) =>
                {
                    if (gameObject != null)
                    {
                        gameObject.GetComponent<BaseElement>().Deselect();
                    }
                }
            );
            selection.Clear();
        }

        private void OnSelectionUpdated()
        {
            selection.ForEach((gameObject)
                => gameObject.GetComponent<BaseElement>().Select()
            );
            Repaint();
        }

        private void SceneModified()
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

    }

}