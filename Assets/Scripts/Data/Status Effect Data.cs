using Assets.Scripts.Combat;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public enum StatusEffectType
    {
        Buff,
        Debuff,
        CrowdControl
    }

    public enum StatusEffectTypeID
    {
        AttackBoost,
        DefenseBoost,
        SpeedBoost,

        DamageOverTime,
        Slow,
        Stun,
        Silence,

        HealOverTime
    }

    [CreateAssetMenu(
        fileName = "New Status Effect",
        menuName = "Auto Battler/Status Effect Data")]
    public sealed class StatusEffectData : ScriptableObject
    {
        [Header("General")]
        [SerializeField]
        private string _displayName;

        [SerializeField]
        private StatusEffectType _effectType;

        [SerializeField]
        private StatusEffect _effect;

        [Header("Values")]
        [SerializeField]
        private float _duration = 5f;

        [SerializeField]
        private float _value = 10f;

        [SerializeField]
        private float _tickRate = 1f;


        public string DisplayName => _displayName;

        public StatusEffectType EffectType => _effectType;

        public StatusEffect Effect => _effect;

        public float Duration => _duration;

        public float Value => _value;

        public float TickRate => _tickRate;
    }
}