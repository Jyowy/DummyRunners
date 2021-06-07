using UnityEngine;

namespace Stadiums
{

    public class SpawnPoint : MonoBehaviour
    {

        [SerializeField]
        private bool faceRight = true;

        public bool FaceRight => faceRight;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "StartPoint");
        }
#endif

    }

}