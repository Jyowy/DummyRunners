using UnityEngine;

namespace Tools.LevelEditor
{

    public abstract class BaseElement : MonoBehaviour
    {

        public virtual Vector2 GetMeanPoint()
            => Vector2.zero;

        public virtual void Move(Vector2 newPosition) { }

        public void Select()
        {
            selected = true;
            OnSelect();
        }

        protected virtual void OnSelect() { }

        public void Deselect()
        {
            selected = false;
            OnDeselect();
        }

        protected virtual void OnDeselect() { }

        public bool selected { get; private set; } = false;
        protected virtual Color GizmoColor => selected
            ? Color.white
            : Color.yellow;

    }

}
