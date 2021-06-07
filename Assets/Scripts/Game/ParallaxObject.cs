using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Game
{

    public class ParallaxObject : MonoBehaviour
    {

        [SerializeField]
        [Min(1f)]
        private float depth = 100f;
        [SerializeField]
        [Range(45f, 90f)]
        private float fov = 60f;

        [SerializeField]
        private float ratioDebug = 1f;

        private Vector2 originalPosition = Vector2.zero;

        private void Awake()
        {
            originalPosition = transform.position;
        }

        public void FixedUpdate()
        {
            Vector2 cameraPosition = Camera.main.transform.position;

            Vector2 newPosition = originalPosition;
            float distance = originalPosition.x - cameraPosition.x;

            float a = math.radians(fov * 0.5f);
            float h = depth / math.cos(a);
            float ratio = math.sin(a) * h;

            newPosition.x = cameraPosition.x + distance / ratio;
            transform.position = newPosition;

            ratioDebug = ratio;
        }

    }

}