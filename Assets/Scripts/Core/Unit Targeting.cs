using Assets.Scripts.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core
{
    /// <summary>
    /// Finds and stores possible enemy targets.
    /// Does not handle movement or combat.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UnitTargeting : MonoBehaviour
    {
        [SerializeField]
        private UnitController _controller;

        [SerializeField]
        private float _detectionRange = 20f;

        [SerializeField]
        private LayerMask _unitLayer;

        private readonly Collider[] _results = new Collider[32];

        private UnitController _currentTarget;

        public UnitController CurrentTarget => _currentTarget;

        private void Awake()
        {
            if (_controller == null)
            {
                _controller = GetComponent<UnitController>();
            }

            Debug.Assert(
                _controller != null,
                $"{name} is missing UnitController.",
                this);
        }

        private void Update()
        {
            if (_currentTarget != null &&
                _currentTarget.Health.IsAlive)
            {
                return;
            }

            FindTarget();
        }

        /// <summary>
        /// Finds the closest enemy unit within detection range.
        /// </summary>
        public void FindTarget()
        {
            int count = Physics.OverlapSphereNonAlloc(
                transform.position,
                _detectionRange,
                _results,
                _unitLayer);

            float closestDistance = Mathf.Infinity;
            UnitController closestTarget = null;

            for (int i = 0; i < count; i++)
            {
                Collider collider = _results[i];

                if (collider == null)
                    continue;

                UnitController unit =
                    collider.GetComponent<UnitController>();

                if (unit == null)
                    continue;

                if (!unit.Health.IsAlive)
                    continue;

                if (!IsEnemy(unit))
                    continue;

                float distance = Vector3.Distance(
                    transform.position,
                    unit.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = unit;
                }
            }

            _currentTarget = closestTarget;
        }

        /// <summary>
        /// Clears the current target.
        /// </summary>
        public void ClearTarget()
        {
            _currentTarget = null;
        }

        private bool IsEnemy(UnitController unit)
        {
            return unit.Data.Team != _controller.Data.Team;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(
                transform.position,
                _detectionRange);
        }
#endif
    }
}