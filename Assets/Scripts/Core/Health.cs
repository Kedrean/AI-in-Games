using Assets.Scripts.Data;
using Assets.Scripts.Interfaces;
using System;
using UnityEngine;

namespace Assets.Scripts.Core
{
    /// <summary>
    /// Manages a unit's runtime health.
    /// </summary>
    public sealed class Health : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private UnitData _unitData;

        public event Action<int, int> HealthChanged;
        public event Action<int> Damaged;
        public event Action<int> Healed;
        public event Action Died;

        public int CurrentHealth { get; private set; }

        public int MaxHealth => _unitData.MaxHealth;

        public bool IsAlive => CurrentHealth > 0;

        private bool _isDead;

        private void Awake()
        {
            if (_unitData == null)
            {
                Debug.LogError($"{name} is missing UnitData.", this);
                enabled = false;
                return;
            }

            CurrentHealth = _unitData.MaxHealth;
        }

        public void TakeDamage(int amount)
        {
            if (_isDead)
                return;

            amount = Mathf.Max(0, amount);

            if (amount == 0)
                return;

            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);

            Damaged?.Invoke(amount);
            HealthChanged?.Invoke(CurrentHealth, MaxHealth);

            if (CurrentHealth == 0)
            {
                _isDead = true;
                Died?.Invoke();
            }
        }

        public void Heal(int amount)
        {
            if (_isDead)
                return;

            amount = Mathf.Max(0, amount);

            if (amount == 0)
                return;

            int previousHealth = CurrentHealth;

            CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);

            int healedAmount = CurrentHealth - previousHealth;

            if (healedAmount <= 0)
                return;

            Healed?.Invoke(healedAmount);
            HealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        public void RestoreFullHealth()
        {
            if (_isDead)
                return;

            CurrentHealth = MaxHealth;
            HealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }
    }
}