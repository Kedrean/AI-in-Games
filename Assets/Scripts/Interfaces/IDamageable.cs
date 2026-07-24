namespace Assets.Scripts.Interfaces
{
    /// <summary>
    /// Represents an object that can receive damage and healing.
    /// </summary>
    public interface IDamageable
    {
        int CurrentHealth { get; }
        int MaxHealth { get; }
        bool IsAlive { get; }

        void TakeDamage(int amount);
        void Heal(int amount);
    }
}