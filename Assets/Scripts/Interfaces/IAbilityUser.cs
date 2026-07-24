using Assets.Scripts.Data;
using System.Collections.Generic;

namespace Assets.Scripts.Interfaces
{
    /// <summary>
    /// Represents a unit capable of using abilities.
    /// </summary>
    public interface IAbilityUser
    {
        IReadOnlyList<AbilityData> Abilities { get; }
    }
}