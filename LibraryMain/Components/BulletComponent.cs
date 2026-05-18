using UnityEngine;

namespace BaldiTestMod
{
    public class BulletComponent : MonoBehaviour
    {
        private float speed = 500f;
        private Vector3 direction;
        private bool hasHit = false;
        private Baldi baldi;

        public static Dictionary<Entity, int> hitCounts = new Dictionary<Entity, int>();
        public static Dictionary<Entity, MovementModifier> appliedModifiers = new Dictionary<Entity, MovementModifier>();

        public void Initialize(Vector3 dir, float spd, Baldi baldi)
        {
            this.direction = dir.normalized;
            this.speed = 150f;
            this.baldi = baldi;
        }

        public void Update()
        {
            if (!hasHit)
            {
                transform.position += direction * speed * Time.deltaTime;
            }
        }

        public void OnChildTriggerEnter(Collider other)
        {
            if (hasHit) return;

            Entity target = other.GetComponent<Entity>();
            if (target == null) target = other.GetComponentInParent<Entity>();
            if (target == null) return;

            if (other.CompareTag("Player") || other.CompareTag("NPC") && (other.transform != baldi.transform))
            {
                hasHit = true;
                RegisterHit(target);
                Destroy(gameObject, 0.05f);
            }
        }

        void RegisterHit(Entity target)
        {
            if (!hitCounts.ContainsKey(target))
            {
                hitCounts[target] = 0;
            }
            hitCounts[target]++;
            int currentHits = hitCounts[target];

            ActivityModifier actMod = target.ExternalActivity;

            if (appliedModifiers.ContainsKey(target))
            {
                actMod.moveMods.Remove(appliedModifiers[target]);
            }

            float slowFactor;
            switch (currentHits)
            {
                case 1: slowFactor = 0.75f; break;
                case 2: slowFactor = 0.4f; break;
                case 3: slowFactor = 0f; break;
                default: slowFactor = 1f; break;
            }

            MovementModifier newMod = new MovementModifier(Vector3.zero, slowFactor);
            actMod.moveMods.Add(newMod);
            appliedModifiers[target] = newMod;
        }
    }

    public class BulletCollisionProxy : MonoBehaviour
    {
        public BulletComponent owner;

        void OnTriggerEnter(Collider other)
        {
            if (owner != null)
            {
                owner.OnChildTriggerEnter(other);
            }
        }
    }
}