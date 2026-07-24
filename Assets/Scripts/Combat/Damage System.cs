using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Performs all damage and healing calculations.
    /// </summary>
    public static class DamageSystem
    {
        /// <summary>
        /// Calculates the final damage dealt after defense and critical hits.
        /// </summary>
        /// <param name="attack">Attacker's Attack stat.</param>
        /// <param name="defense">Target's Defense stat.</param>
        /// <param name="abilityDamage">Additional damage from an ability.</param>
        /// <param name="isCritical">Whether the hit is a critical strike.</param>
        /// <param name="criticalMultiplier">Critical damage multiplier.</param>
        /// <returns>Final damage value.</returns>
        public static int CalculateDamage(
            int attack,
            int defense,
            int abilityDamage,
            bool isCritical,
            float criticalMultiplier)
        {
            int damage = Mathf.Max(1, attack + abilityDamage - defense);

            if (isCritical)
            {
                damage = Mathf.RoundToInt(damage * criticalMultiplier);
            }

            return damage;
        }

        /// <summary>
        /// Applies damage to a target.
        /// </summary>
        public static void ApplyDamage(IDamageable target, int damage)
        {
            if (target == null || !target.IsAlive)
                return;

            target.TakeDamage(damage);
        }

        /// <summary>
        /// Applies healing to a target.
        /// </summary>
        public static void ApplyHealing(IDamageable target, int amount)
        {
            if (target == null || !target.IsAlive)
                return;

            target.Heal(amount);
        }
    }
}