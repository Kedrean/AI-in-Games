using Assets.Scripts.Abilities;
using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.Abilities
{
    /// <summary>
    /// Base class for abilities that are actively cast by a unit.
    /// Handles cooldown management while leaving the execution logic
    /// to derived classes.
    /// </summary>
    public abstract class ActiveAbility : Ability
    {
        private float _cooldownTimer;

        /// <summary>
        /// Returns true when the ability has finished cooling down.
        /// </summary>
        public bool IsReady => _cooldownTimer <= 0f;

        protected virtual void Update()
        {
            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
            }
        }

        public override bool CanExecute(UnitController caster, UnitController target)
        {
            if (!IsReady)
                return false;

            if (caster == null || target == null)
                return false;

            if (!target.Health.IsAlive)
                return false;

            float distance = Vector3.Distance(
                caster.transform.position,
                target.transform.position);

            return distance <= Data.Range;
        }

        /// <summary>
        /// Starts the cooldown after a successful cast.
        /// Derived classes should call base.Execute() first.
        /// </summary>
        public override void Execute(UnitController caster, UnitController target)
        {
            _cooldownTimer = Data.Cooldown;
        }

        /// <summary>
        /// Instantly refreshes the cooldown.
        /// Useful for testing or future gameplay effects.
        /// </summary>
        public void ResetCooldown()
        {
            _cooldownTimer = 0f;
        }

        /// <summary>
        /// Gets the remaining cooldown time.
        /// </summary>
        public float GetRemainingCooldown()
        {
            return Mathf.Max(0f, _cooldownTimer);
        }
    }
}