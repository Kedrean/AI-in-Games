using Assets.Scripts.Abilities;
using Assets.Scripts.Core;

namespace Assets.Scripts.Abilities
{
    /// <summary>
    /// Base class for passive abilities.
    /// Passive abilities do not have cooldowns and cannot be manually cast.
    /// They react to gameplay events or continuously apply effects.
    /// </summary>
    public abstract class PassiveAbility : Ability
    {
        /// <summary>
        /// Passive abilities are always considered available.
        /// </summary>
        public override bool CanExecute(UnitController caster, UnitController target)
        {
            return true;
        }

        /// <summary>
        /// Called once when the passive is initialized.
        /// </summary>
        public virtual void Initialize(UnitController owner)
        {
        }

        /// <summary>
        /// Executes the passive's effect.
        /// Derived classes define the actual behavior.
        /// </summary>
        public abstract override void Execute(UnitController caster, UnitController target);
    }
}