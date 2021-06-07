using Player;
using UnityEngine;

namespace World.Level
{

    public class Level : MonoBehaviour
    {

        [SerializeField]
        private Transform levelRoot = null;
        [SerializeField]
        private SpawnPoint spawnPoint = null;

        private void Start()
        {
            PlayerManager.InstantiatePlayer(levelRoot, spawnPoint);
        }

    }

}