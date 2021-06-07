using UnityEngine;

namespace World.Level
{

    public class SpawnPoint : MonoBehaviour
    {

        [SerializeField]
        private bool faceRight = true;

        public bool FaceRight => faceRight;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "SpawnPoint.png", true);
        }
#endif

    }

}