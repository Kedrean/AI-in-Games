using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    /// <summary>
    /// Represents an object that can be targeted by AI.
    /// </summary>
    public interface ITargetable
    {
        Transform AimPoint { get; }

        bool IsAlive { get; }
    }
}