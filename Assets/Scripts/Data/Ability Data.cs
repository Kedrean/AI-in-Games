using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data
{
    /// <summary>
    /// Stores the immutable data for an ability.
    /// Runtime execution is handled by Ability classes.
    /// </summary>
    [CreateAssetMenu(fileName = "New Ability", menuName = "Auto Battler/Ability Data")]
    public sealed class AbilityData : ScriptableObject
    {
        [Header("General")]
        [SerializeField]
        private string _abilityName = string.Empty;

        [TextArea(2, 4)]
        [SerializeField]
        private string _description = string.Empty;


        [Header("Combat")]

        [Tooltip("Base damage dealt. Set to 0 for non-damaging abilities.")]
        [SerializeField, Min(0)]
        private int _damage = 0;

        [Tooltip("Maximum casting distance.")]
        [SerializeField, Min(0f)]
        private float _range = 2f;

        [Tooltip("Time before the ability can be used again.")]
        [SerializeField, Min(0f)]
        private float _cooldown = 1f;

        [Tooltip("Time required before the ability is executed.")]
        [SerializeField, Min(0f)]
        private float _castTime = 0f;


        [Header("Projectile")]

        [Tooltip("Projectile speed. Ignored for melee abilities.")]
        [SerializeField, Min(0f)]
        private float _projectileSpeed = 0f;


        [Header("Area of Effect")]

        [Tooltip("0 = Single Target")]
        [SerializeField, Min(0f)]
        private float _radius = 0f;


        [Header("Status Effects")]

        [Tooltip("Status effects applied when this ability succeeds.")]
        [SerializeField]
        private List<StatusEffectData> _statusEffects = new();


        #region Properties

        public string AbilityName => _abilityName;

        public string Description => _description;


        public int Damage => _damage;

        public float Range => _range;

        public float Cooldown => _cooldown;

        public float CastTime => _castTime;


        public float ProjectileSpeed => _projectileSpeed;

        public float Radius => _radius;


        public IReadOnlyList<StatusEffectData> StatusEffects =>
            _statusEffects;

        #endregion


#if UNITY_EDITOR

        private void OnValidate()
        {
            _damage = Mathf.Max(0, _damage);

            _range = Mathf.Max(0f, _range);

            _cooldown = Mathf.Max(0f, _cooldown);

            _castTime = Mathf.Max(0f, _castTime);

            _projectileSpeed = Mathf.Max(0f, _projectileSpeed);

            _radius = Mathf.Max(0f, _radius);
        }

#endif
    }
}