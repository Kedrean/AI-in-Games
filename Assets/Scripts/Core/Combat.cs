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

            StopAttack();

            _attackCoroutine = StartCoroutine(AttackRoutine(target));
        }

        /// <summary>
        /// Stops attacking.
        /// </summary>
        public void StopAttack()
        {
            if (_attackCoroutine == null)
                return;

            StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
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
            UnitData defender = target.Data;

            float attackInterval = 1f / attacker.AttackSpeed;
            WaitForSeconds attackDelay = new WaitForSeconds(attackInterval);

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
                        defender.Defense,
                        0,
                        critical,
                        attacker.CriticalMultiplier);

                    PerformAttack(target, damage);

                    if (_controller.AnimationController != null)
                    {
                        _controller.AnimationController.PlayAttack();
                    }
                }

                yield return attackDelay;
            }

            _attackCoroutine = null;
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

                GameObject projectileObject = Instantiate(
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