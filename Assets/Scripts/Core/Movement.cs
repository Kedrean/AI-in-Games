using Assets.Scripts.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Core
{
    /// <summary>
    /// Handles unit movement only.
    /// Target selection and combat are handled elsewhere.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class Movement : MonoBehaviour
    {
        [SerializeField] private UnitController _controller;
        [SerializeField] private NavMeshAgent _agent;

        private UnitController _target;

        private void Awake()
        {
            if (_controller == null)
                _controller = GetComponent<UnitController>();

            if (_agent == null)
                _agent = GetComponent<NavMeshAgent>();

            Debug.Assert(_controller != null, "Missing UnitController.", this);
            Debug.Assert(_agent != null, "Missing NavMeshAgent.", this);

            _agent.speed = _controller.Data.MoveSpeed;
            _agent.stoppingDistance = 0f;
            _agent.autoBraking = true;
        }

        private void Update()
        {
            if (_target == null)
                return;

            if (!_target.Health.IsAlive)
            {
                Stop();
                return;
            }

            float attackRange = _controller.Data.AttackRange;
            float distance = Vector3.Distance(
                transform.position,
                _target.transform.position);

            // Melee units
            if (attackRange <= 2f)
            {
                _agent.stoppingDistance = attackRange * 0.9f;
                _agent.SetDestination(_target.transform.position);

                _controller.AnimationController.SetMoving(
                    _agent.remainingDistance > _agent.stoppingDistance);
            }
            // Ranged units
            else
            {
                float desiredDistance = attackRange * 0.9f;

                if (distance > desiredDistance)
                {
                    _agent.isStopped = false;
                    _agent.stoppingDistance = desiredDistance;
                    _agent.SetDestination(_target.transform.position);

                    _controller.AnimationController.SetMoving(true);
                }
                else if (distance < desiredDistance * 0.75f)
                {
                    Vector3 direction =
                        (transform.position - _target.transform.position).normalized;

                    Vector3 destination =
                        transform.position + direction * 2f;

                    NavMeshHit hit;

                    if (NavMesh.SamplePosition(
                        destination,
                        out hit,
                        2f,
                        NavMesh.AllAreas))
                    {
                        _agent.isStopped = false;
                        _agent.SetDestination(hit.position);
                    }

                    _controller.AnimationController.SetMoving(true);
                }
                else
                {
                    Stop();
                }
            }

            FaceTarget(_target);
        }

        /// <summary>
        /// Begins moving toward a target.
        /// </summary>
        public void MoveTo(UnitController target)
        {
            _target = target;

            if (_target == null)
                return;

            _agent.isStopped = false;
        }

        /// <summary>
        /// Stops all movement.
        /// </summary>
        public void Stop()
        {
            _target = null;

            _agent.isStopped = true;
            _agent.ResetPath();

            _controller.AnimationController.SetMoving(false);
        }

        /// <summary>
        /// Smoothly rotates the unit to face its target.
        /// </summary>
        public void FaceTarget(UnitController target)
        {
            if (target == null)
                return;

            Vector3 direction =
                target.transform.position - transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                return;

            Quaternion rotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotation,
                10f * Time.deltaTime);
        }
    }
}