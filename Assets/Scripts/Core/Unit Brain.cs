using System.Collections.Generic;
using Assets.Scripts.Data;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Core
{
    /// <summary>
    /// Handles target selection and high-level combat decisions.
    /// Does not move the unit or deal damage directly.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UnitBrain : MonoBehaviour
    {
        [SerializeField]
        private UnitController _controller;

        private UnitController _currentTarget;

        /// <summary>
        /// The unit's current combat target.
        /// </summary>
        public UnitController CurrentTarget => _currentTarget;

        private void Awake()
        {
            if (_controller == null)
            {
                _controller = GetComponent<UnitController>();
            }

            Debug.Assert(
                _controller != null,
                $"{nameof(UnitBrain)} requires a UnitController.",
                this);
        }

        /// <summary>
        /// Assigns a new target.
        /// </summary>
        public void SetTarget(UnitController target)
        {
            if (target == null)
                return;

            if (!target.Health.IsAlive)
                return;

            _currentTarget = target;
        }

        /// <summary>
        /// Clears the current target.
        /// </summary>
        public void ClearTarget()
        {
            _currentTarget = null;
            _controller.Combat.StopAttack();
        }

        private void Update()
        {
            if (_currentTarget == null)
            {
                AcquireTarget();

                if (_currentTarget == null)
                    return;
            }

            if (!_currentTarget.Health.IsAlive)
            {
                ClearTarget();
                AcquireTarget();

                if (_currentTarget == null)
                    return;
            }

            float distance = Vector3.Distance(
                transform.position,
                _currentTarget.transform.position);

            if (distance <= _controller.Data.AttackRange)
            {
                _controller.Movement.Stop();
                _controller.Combat.Attack(_currentTarget);
            }
            else
            {
                _controller.Combat.StopAttack();
                _controller.Movement.MoveTo(_currentTarget);
            }
        }

        /// <summary>
        /// Finds the nearest living enemy.
        /// </summary>
        private void AcquireTarget()
        {
            if (TeamManager.Instance == null)
                return;

            Team enemyTeam =
                _controller.Data.Team == Team.Human
                    ? Team.Skeleton
                    : Team.Human;

            IReadOnlyList<UnitController> enemies =
                TeamManager.Instance.GetLivingTeam(enemyTeam);

            float closestDistance = float.MaxValue;
            UnitController closestTarget = null;

            foreach (UnitController enemy in enemies)
            {
                if (enemy == null || !enemy.Health.IsAlive)
                    continue;

                float distance = Vector3.Distance(
                    transform.position,
                    enemy.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = enemy;
                }
            }

            _currentTarget = closestTarget;
        }
    }
}