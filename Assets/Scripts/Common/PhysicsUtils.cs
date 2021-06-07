using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{

    public static class PhysicsUtils
    {

        public static bool FindCollision(Vector2 start, Vector2 direction, float distance, LayerMask layers)
        {
            Vector2 end = start + direction * distance;
            var hit = Physics2D.Linecast(start, end, layers);
            return hit.collider != null;
        }

        public static float GetDistance(Vector2 start, Vector2 direction, float distance, LayerMask layers)
        {
            Vector2 end = start + direction * distance;
            var hit = Physics2D.Linecast(start, end, layers);
            return hit.collider != null
                ? hit.distance
                : Mathf.Infinity;
        }

        public static Collider2D[] GetAllCollidersInRadius(Vector2 center, float radius, LayerMask layers)
        {
            return Physics2D.OverlapCircleAll(center, radius, layers);
        }

    }

}