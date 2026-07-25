using Assets.Scripts.Abilities;
using Assets.Scripts.Combat;
using Assets.Scripts.Data;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Core
{
    /// <summary>
    /// Handles a unit's basic attacks and ability execution.
    /// Does not perform target selection or movement.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Combat : MonoBehaviour
    {
        [SerializeField]
        private UnitController _controller;

        [SerializeField]
        private Transform _projectileSpawnPoint;

        private Coroutine _attackCoroutine;
        private UnitController _currentTarget;

        // Stores the current attack until the animation releases it.
        private UnitController _pendingTarget;
        private int _pendingDamage;

        public bool IsAttacking => _attackCoroutine != null;

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

        /// <summary>
        /// Begins attacking the specified target.
        /// </summary>
        public void Attack(UnitController target)
        {
            if (target == null)
                return;

            if (!target.Health.IsAlive)
                return;

            if (_attackCoroutine != null &&
                _currentTarget == target)
            {
                return;
            }

            StopAttack();

            _currentTarget = target;
            _attackCoroutine = StartCoroutine(AttackRoutine(target));
        }

        /// <summary>
        /// Stops attacking.
        /// </summary>
        public void StopAttack()
        {
            if (_attackCoroutine != null)
            {
                StopCoroutine(_attackCoroutine);
            }

            _attackCoroutine = null;
            _currentTarget = null;
            _pendingTarget = null;
            _pendingDamage = 0;
        }

        /// <summary>
        /// Executes an active ability.
        /// </summary>
        public void UseAbility(Ability ability, UnitController target)
        {
            if (ability == null || target == null)
                return;

            if (!ability.CanExecute(_controller, target))
                return;

            ability.Execute(_controller, target);
        }

        private IEnumerator AttackRoutine(UnitController target)
        {
            UnitData attacker = _controller.Data;

            while (target != null && target.Health.IsAlive)
            {
                float distance = Vector3.Distance(
                    transform.position,
                    target.transform.position);

                if (distance <= attacker.AttackRange)
                {
                    bool critical =
                        Random.value <= attacker.CriticalChance;

                    int damage = DamageSystem.CalculateDamage(
                        attacker.Attack,
                        target.Data.Defense,
                        0,
                        critical,
                        attacker.CriticalMultiplier);

                    _pendingTarget = target;
                    _pendingDamage = damage;

                    _controller.AnimationController.PlayAttack();

                    yield return new WaitForSeconds(
                        1f / attacker.AttackSpeed);
                }
                else
                {
                    yield return null;
                }
            }

            _attackCoroutine = null;
            _currentTarget = null;
        }

        /// <summary>
        /// Called by an Animation Event on the attack animation.
        /// </summary>
        public void ReleaseAttack()
        {
            if (_pendingTarget == null)
                return;

            if (!_pendingTarget.Health.IsAlive)
                return;

            PerformAttack(
                _pendingTarget,
                _pendingDamage);

            _pendingTarget = null;
            _pendingDamage = 0;
        }

        private void PerformAttack(UnitController target, int damage)
        {
            UnitData attacker = _controller.Data;

            // Ranged attack
            if (attacker.ProjectilePrefab != null)
            {
                Vector3 spawnPosition =
                    _projectileSpawnPoint != null
                    ? _projectileSpawnPoint.position
                    : transform.position;

                GameObject projectileObject =
                    Instantiate(
                        attacker.ProjectilePrefab,
                        spawnPosition,
                        Quaternion.identity);

                Projectile projectile =
                    projectileObject.GetComponent<Projectile>();

                if (projectile != null)
                {
                    projectile.Initialize(
                        target,
                        damage);
                }
                else
                {
                    Debug.LogWarning(
                        $"{attacker.DisplayName}'s projectile prefab has no Projectile component.",
                        this);
                }
            }
            // Melee attack
            else
            {
                DamageSystem.ApplyDamage(
                    target.Health,
                    damage);
            }
        }
    }
}