using Assets.Scripts.Combat;
using Assets.Scripts.Core;
using Assets.Scripts.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Abilities
{
    /// <summary>
    /// Base class for every ability in the game.
    /// AbilityData stores immutable configuration.
    /// Runtime execution is handled by derived abilities.
    /// </summary>
    public abstract class Ability : MonoBehaviour
    {
        [Header("Ability Data")]
        [SerializeField]
        private AbilityData _abilityData;

        public AbilityData Data => _abilityData;


        public abstract bool CanExecute(
            UnitController caster,
            UnitController target);


        public abstract void Execute(
            UnitController caster,
            UnitController target);


        /// <summary>
        /// Creates runtime status effects from this ability's data.
        /// Effects are returned to the caller for application.
        /// </summary>
        protected List<StatusEffect> CreateStatusEffects(
            UnitController caster)
        {
            List<StatusEffect> effects = new();

            if (Data == null)
                return effects;


            foreach (StatusEffectData effectData in Data.StatusEffects)
            {
                if (effectData == null)
                    continue;

                effects.Add(
                    new StatusEffect(
                        effectData,
                        caster));
            }

            return effects;
        }


#if UNITY_EDITOR

        protected virtual void OnValidate()
        {
            if (_abilityData == null)
            {
                Debug.LogWarning(
                    $"{name} is missing an AbilityData reference.",
                    this);
            }
        }

#endif
    }
}