using Assets.Scripts.Core;
using Assets.Scripts.Combat;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Homing projectile used by all ranged attacks and projectile abilities.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Projectile : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 15f;

        private UnitController _target;
        private int _damage;

        /// <summary>
        /// Initializes the projectile after spawning.
        /// </summary>
        public void Initialize(UnitController target, int damage)
        {
            _target = target;
            _damage = damage;
        }

        private void Update()
        {
            if (_target == null || !_target.Health.IsAlive)
            {
                Destroy(gameObject);
                return;
            }

            Transform targetTransform = _target.transform;

            Vector3 direction =
                (targetTransform.position - transform.position).normalized;

            transform.position += direction * _speed * Time.deltaTime;

            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            float hitDistance = 0.15f;

            if (Vector3.Distance(transform.position, targetTransform.position) <= hitDistance)
            {
                DamageSystem.ApplyDamage(_target.Health, _damage);

                Destroy(gameObject);
            }
        }
    }
}