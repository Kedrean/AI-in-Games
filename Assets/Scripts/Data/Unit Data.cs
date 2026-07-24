using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public enum Team
    {
        Human,
        Skeleton
    }

    public enum UnitClass
    {
        Knight,
        Barbarian,
        Mage,
        Ranger,

        SkeletonWarrior,
        SkeletonRogue,
        SkeletonMage,
        SkeletonMinion
    }

    /// <summary>
    /// Stores the immutable base data for a unit.
    /// Runtime values (HP, cooldowns, buffs, etc.) are handled by runtime components.
    /// </summary>
    [CreateAssetMenu(fileName = "New Unit", menuName = "Auto Battler/Unit Data")]
    public sealed class UnitData : ScriptableObject
    {
        [Header("General")]
        [SerializeField] private string _displayName = string.Empty;
        [SerializeField] private Team _team;
        [SerializeField] private UnitClass _unitClass;

        [Header("Base Stats")]
        [SerializeField, Min(1)] private int _maxHealth = 100;
        [SerializeField, Min(0)] private int _attack = 10;
        [SerializeField, Min(0)] private int _defense = 0;

        [SerializeField, Min(0f)] private float _moveSpeed = 3.5f;
        [SerializeField, Min(0.1f)] private float _attackSpeed = 1f;
        [SerializeField, Min(0.5f)] private float _attackRange = 2f;

        [Header("Critical Stats")]
        [SerializeField, Range(0f, 1f)]
        [Tooltip("Critical hit chance (0.25 = 25%).")]
        private float _criticalChance = 0.05f;

        [SerializeField, Min(1f)]
        [Tooltip("Critical damage multiplier (1.5 = 150% damage).")]
        private float _criticalMultiplier = 1.5f;

        [Header("References")]
        [Tooltip("Leave empty for melee units.")]
        [SerializeField] private GameObject _projectilePrefab;

        [SerializeField] private RuntimeAnimatorController _animatorController;

        [SerializeField] private List<AbilityData> _abilities = new();

        #region Properties

        public string DisplayName => _displayName;
        public Team Team => _team;
        public UnitClass UnitClass => _unitClass;

        public int MaxHealth => _maxHealth;
        public int Attack => _attack;
        public int Defense => _defense;

        public float MoveSpeed => _moveSpeed;
        public float AttackSpeed => _attackSpeed;
        public float AttackRange => _attackRange;

        public float CriticalChance => _criticalChance;
        public float CriticalMultiplier => _criticalMultiplier;

        public GameObject ProjectilePrefab => _projectilePrefab;
        public RuntimeAnimatorController AnimatorController => _animatorController;

        public IReadOnlyList<AbilityData> Abilities => _abilities;

        #endregion

#if UNITY_EDITOR
        private void OnValidate()
        {
            _maxHealth = Mathf.Max(1, _maxHealth);
            _attack = Mathf.Max(0, _attack);
            _defense = Mathf.Max(0, _defense);

            _moveSpeed = Mathf.Max(0f, _moveSpeed);
            _attackSpeed = Mathf.Max(0.1f, _attackSpeed);
            _attackRange = Mathf.Max(0.5f, _attackRange);

            _criticalMultiplier = Mathf.Max(1f, _criticalMultiplier);
        }
#endif
    }
}