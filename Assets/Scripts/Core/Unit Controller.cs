using Assets.Scripts.Data;
using Assets.Scripts.Managers;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Core
{
    /// <summary>
    /// Central access point for all runtime components attached to a unit.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UnitController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField]
        private UnitData _unitData;

        [Header("Core Components")]
        [SerializeField]
        private Health _health;

        [SerializeField]
        private Movement _movement;

        [SerializeField]
        private Combat _combat;

        [SerializeField]
        private UnitBrain _brain;

        [SerializeField]
        private AnimationController _animationController;

        private bool _registered;
        private bool _isDying;

        public UnitData Data => _unitData;
        public Health Health => _health;
        public Movement Movement => _movement;
        public Combat Combat => _combat;
        public UnitBrain Brain => _brain;
        public AnimationController AnimationController => _animationController;

        private void Awake()
        {
            ValidateReferences();
        }

        private void Start()
        {
            if (TeamManager.Instance != null)
            {
                TeamManager.Instance.RegisterUnit(this);
                _registered = true;
            }

            if (_health != null)
            {
                _health.Died += HandleDeath;
            }

            // Prevent AI from acting until the spawn animation finishes.
            _brain.DisableBrain();

            // Play spawn animation.
            _animationController.PlaySpawn();
        }

        /// <summary>
        /// Called by an Animation Event at the end of the Spawn animation.
        /// </summary>
        public void EnableBrain()
        {
            if (_isDying)
                return;

            _brain.EnableBrain();
        }

        private void HandleDeath()
        {
            if (_isDying)
                return;

            _isDying = true;

            StartCoroutine(DeathRoutine());
        }

        /// <summary>
        /// Stops gameplay and plays the death animation.
        /// The unit is destroyed by an Animation Event.
        /// </summary>
        private IEnumerator DeathRoutine()
        {
            _brain.DisableBrain();
            _combat.StopAttack();
            _movement.Stop();

            _animationController.PlayDeath();

            // Wait until FinishDeath() is called by the animation.
            yield break;
        }

        /// <summary>
        /// Called by an Animation Event on the last frame
        /// of the Death animation.
        /// </summary>
        public void FinishDeath()
        {
            if (_registered && TeamManager.Instance != null)
            {
                TeamManager.Instance.UnregisterUnit(this);
                _registered = false;
            }

            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (_health != null)
            {
                _health.Died -= HandleDeath;
            }

            if (_registered && TeamManager.Instance != null)
            {
                TeamManager.Instance.UnregisterUnit(this);
                _registered = false;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            AutoAssignComponents();
        }
#endif

        private void AutoAssignComponents()
        {
            if (_health == null)
                _health = GetComponent<Health>();

            if (_movement == null)
                _movement = GetComponent<Movement>();

            if (_combat == null)
                _combat = GetComponent<Combat>();

            if (_brain == null)
                _brain = GetComponent<UnitBrain>();

            if (_animationController == null)
                _animationController = GetComponent<AnimationController>();
        }

        private void ValidateReferences()
        {
            Debug.Assert(
                _unitData != null,
                $"{name} is missing UnitData.",
                this);

            Debug.Assert(
                _health != null,
                $"{name} is missing Health.",
                this);

            Debug.Assert(
                _movement != null,
                $"{name} is missing Movement.",
                this);

            Debug.Assert(
                _combat != null,
                $"{name} is missing Combat.",
                this);

            Debug.Assert(
                _brain != null,
                $"{name} is missing UnitBrain.",
                this);

            Debug.Assert(
                _animationController != null,
                $"{name} is missing Animation Controller.",
                this);
        }
    }
}