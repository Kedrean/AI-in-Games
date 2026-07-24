using Assets.Scripts.Core;
using Assets.Scripts.Data;

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Runtime instance of an active status effect.
    /// Created from StatusEffectData when an ability applies an effect.
    /// </summary>
    public sealed class StatusEffect
    {
        public StatusEffectData Data { get; }

        public UnitController Source { get; }

        public float RemainingDuration { get; private set; }

        public bool IsExpired =>
            RemainingDuration <= 0f;


        public StatusEffect(
            StatusEffectData data,
            UnitController source)
        {
            Data = data;
            Source = source;
            RemainingDuration = data.Duration;
        }


        public void Tick(float deltaTime)
        {
            RemainingDuration -= deltaTime;
        }


        public void Refresh()
        {
            RemainingDuration = Data.Duration;
        }
    }
}