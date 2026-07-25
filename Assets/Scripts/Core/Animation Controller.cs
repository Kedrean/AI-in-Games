using UnityEngine;

namespace Assets.Scripts.Core
{
    /// <summary>
    /// Handles all animation requests for a unit.
    /// Contains no gameplay logic.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public sealed class AnimationController : MonoBehaviour
    {
        private static readonly int SpawnHash = Animator.StringToHash("Spawn");
        private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int DeathHash = Animator.StringToHash("Death");

        [SerializeField]
        private Animator _animator;

        private void Awake()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }

            Debug.Assert(
                _animator != null,
                $"{nameof(AnimationController)} requires an Animator.",
                this);
        }

        /// <summary>
        /// Plays the spawn animation.
        /// </summary>
        public void PlaySpawn()
        {
            _animator.ResetTrigger(AttackHash);
            _animator.ResetTrigger(DeathHash);

            _animator.SetTrigger(SpawnHash);
        }

        /// <summary>
        /// Returns the unit to its idle state.
        /// </summary>
        public void PlayIdle()
        {
            _animator.SetBool(IsMovingHash, false);
        }

        /// <summary>
        /// Enables or disables the walking animation.
        /// </summary>
        public void SetMoving(bool moving)
        {
            _animator.SetBool(IsMovingHash, moving);
        }

        /// <summary>
        /// Plays the basic attack animation.
        /// </summary>
        public void PlayAttack()
        {
            _animator.SetTrigger(AttackHash);
        }

        /// <summary>
        /// Plays the death animation.
        /// </summary>
        public void PlayDeath()
        {
            _animator.ResetTrigger(AttackHash);
            _animator.SetBool(IsMovingHash, false);
            _animator.SetTrigger(DeathHash);
        }
    }
}