using Player;
using System.Collections.Generic;
using UnityEngine;

namespace World.TimeAttacks
{

    public class TimeAttackRoutePoint : MonoBehaviour
    {

        [SerializeField]
        private ParticleSystem nextPoint = null;
        [SerializeField]
        private ParticleSystem pointGathered = null;

        private System.Action onReached = null;

        private bool active = false;

        public void Activate(System.Action onReached)
        {
            gameObject.SetActive(true);
            active = true;

            nextPoint.Play(false);

            this.onReached = onReached;
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
            active = false;

            nextPoint.Stop(false);
            nextPoint.SetParticles(null);
            pointGathered.Stop();

            onReached = null;
        }

        public void SetNextPoint(Vector2 position)
        {
            transform.position = position;

            nextPoint.Stop(false);
            nextPoint.Play(false);
        }

        private void OnParticleCollision(GameObject other)
        {
            if (!active)
                return;

            OnPointGathered();
        }

        private void OnPointGathered()
        {
            nextPoint.Stop(false);
            nextPoint.SetParticles(null);

            pointGathered.transform.position = transform.position;
            pointGathered.Play();

            onReached?.Invoke();
        }



    }

}
