using UnityEngine;
using UnityEngine.AI;
using MyGame.SpellZoltraak;
using StarterAssets;

namespace MyGame.EnemyLogic
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField]
        NavMeshAgent _agent;

        [SerializeField]
        private Zoltraak _zoltraak;

        [SerializeField]
        private Transform _visualModel;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private ThirdPersonController _playerController;

        [SerializeField]
        private Transform _playerTransform;

        [SerializeField]
        private bool _autoFindPlayer = true;

        private const string PlayerTag = "Player";

        [SerializeField]
        protected float _visionRange = 10f;

        [SerializeField]
        protected float _attackRange = 5f;

        [SerializeField]
        private float _chaseStoppingDistance = 1.5f;

        [SerializeField]
        private float _patrolSpeed = 1.5f;

        [SerializeField]
        private float _chaseSpeed = 3.5f;

        [SerializeField]
        private Vector3[] _patrolPoints;

        private int _patrolIndex;

        private enum AIState { Patrol, Chase, Attack }
        private AIState _currentState;
        private AIState _previousState;
        private bool _isChargingSpell = false;
        private float _targetRotation = 0f;
        private float _rotationVelocity = 0f;
        private const float RotationSmoothTime = 0.12f;
        [SerializeField]
        private float _turnSpeed = 180f;

        [SerializeField]
        private string _speedParameterName = "Speed";

        [SerializeField]
        private string _idleStateName = "Idle01";

        [SerializeField]
        private string _moveStateName = "BattleWalkForward";

        [SerializeField]
        private string _attackStateName = "Attack01";

        private int _animIDSpeed;

    
        private void Awake()
        {
            if (_agent == null)
            {
                _agent = GetComponent<NavMeshAgent>();
            }

            if (_agent != null)
            {
                _agent.updateRotation = false;
                _agent.stoppingDistance = _chaseStoppingDistance;
                _agent.angularSpeed = 720f;
                _agent.speed = _patrolSpeed;
            }

            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }

            if (_animator != null)
            {
                ResolveAnimatorParameters();
            }

            TryAssignPlayer();

            if (_zoltraak == null)
            {
                _zoltraak = GetComponent<Zoltraak>();
            }
        }

        private void TryAssignPlayer()
        {
            if (_playerController != null)
            {
                _playerTransform = _playerController.transform;
                return;
            }

            if (_playerTransform != null)
            {
                _playerController = _playerTransform.GetComponent<ThirdPersonController>();
                return;
            }

            ThirdPersonController controller = FindObjectOfType<ThirdPersonController>();
            if (controller != null)
            {
                _playerController = controller;
                _playerTransform = controller.transform;
                return;
            }

            GameObject playerObj = GameObject.FindGameObjectWithTag(PlayerTag);
            if (playerObj != null)
            {
                _playerTransform = playerObj.transform;
                _playerController = _playerTransform.GetComponent<ThirdPersonController>();
                return;
            }

            GameObject[] players = GameObject.FindGameObjectsWithTag(PlayerTag);
            if (players.Length > 0)
            {
                _playerTransform = players[0].transform;
                _playerController = _playerTransform.GetComponent<ThirdPersonController>();
            }
        }

        private float distance = 0f;

        private void Start()
        {
            if (_autoFindPlayer)
            {
                TryAssignPlayer();
            }

            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
        }

        public void SetTarget(ThirdPersonController target)
        {
            _playerController = target;
            _playerTransform = target != null ? target.transform : null;
        }

        public void SetTarget(Transform target)
        {
            _playerTransform = target;
            _playerController = target != null ? target.GetComponent<ThirdPersonController>() : null;
        }

        private void Update()
        {
            if (_autoFindPlayer)
            {
                TryAssignPlayer();
            }

            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }

            Transform targetTransform = _playerTransform;
            if (targetTransform == null && _playerController != null)
            {
                targetTransform = _playerController.transform;
            }

            if (targetTransform == null)
            {
                _currentState = AIState.Patrol;
                UpdateAnimatorState(0f);
                return;
            }

            _playerTransform = targetTransform;
            distance = Vector3.Distance(transform.position, targetTransform.position);
            _previousState = _currentState;

            if (distance <= _attackRange)
            {
                _currentState = AIState.Attack;
            }
            else if (distance <= _visionRange)
            {
                _currentState = AIState.Chase;
            }
            else
            {
                _currentState = AIState.Patrol;
            }

            if (_previousState == AIState.Attack && _currentState != AIState.Attack)
            {
                ExitAttackState();
            }

            switch (_currentState)
            {
                case AIState.Patrol:
                    AiPatrol();
                    break;

                case AIState.Chase:
                    AiChase(_playerTransform.position);
                    break;

                case AIState.Attack:
                    AiAttack();
                    break;
            }

            UpdateAnimatorState();
        }

        private void AiPatrol()
        {
            if (_agent == null || _patrolPoints == null || _patrolPoints.Length == 0)
            {
                _agent.isStopped = true;
                return;
            }

            _agent.updateRotation = false;
            _agent.isStopped = false;
            _agent.speed = _patrolSpeed;

            Vector3 targetPoint = _patrolPoints[_patrolIndex];
            if (_agent.destination != targetPoint)
            {
                _agent.destination = targetPoint;
            }

            if (_agent.remainingDistance <= _agent.stoppingDistance + 0.25f)
            {
                _patrolIndex = (_patrolIndex + 1) % _patrolPoints.Length;
                _agent.destination = _patrolPoints[_patrolIndex];
            }

            Vector3 patrolDirection = _agent.destination - transform.position;
            RotateToward(patrolDirection);
        }

        private void AiChase(Vector3 playerPosition)
        {
            if (_agent == null)
            {
                return;
            }

            _agent.updateRotation = false;
            _agent.isStopped = false;
            _agent.speed = _chaseSpeed;
            _agent.stoppingDistance = Mathf.Max(_chaseStoppingDistance, _attackRange * 0.5f);
            _agent.destination = playerPosition;

            Vector3 directionToPlayer = playerPosition - transform.position;
            RotateToward(directionToPlayer);
        }

        private void AiAttack()
        {
            if (_agent == null)
            {
                return;
            }

            _agent.updateRotation = false;
            _agent.isStopped = true;

            Vector3 directionToPlayer = _playerTransform.position - transform.position;
            RotateToward(directionToPlayer);

            if (_isChargingSpell && !_zoltraak.IsCharging)
            {
                _isChargingSpell = false;
            }

            if (!_isChargingSpell)
            {
                _zoltraak.StartCharge();
                _isChargingSpell = true;
            }
            else
            {
                _zoltraak.HoldCharge();
            }
        }

        private void RotateToward(Vector3 direction)
        {
            if (_agent != null)
            {
                _agent.updateRotation = false;
            }

            Transform rotationTarget = _visualModel != null ? _visualModel : transform;
            direction.y = 0f;

            if (direction == Vector3.zero)
            {
                return;
            }

            Quaternion lookRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            float currentY = rotationTarget.eulerAngles.y;
            float targetY = lookRotation.eulerAngles.y;
            float delta = Mathf.DeltaAngle(currentY, targetY);
            float maxStep = _turnSpeed * Time.deltaTime;
            float adjusted = Mathf.Clamp(delta, -maxStep, maxStep);

            rotationTarget.rotation = Quaternion.Euler(0f, currentY + adjusted, 0f);

            if (_visualModel != null)
            {
                transform.rotation = rotationTarget.rotation;
            }
        }

        private void ResolveAnimatorParameters()
        {
            if (_animator == null)
            {
                return;
            }

            string[] candidateNames = new[]
            {
                _speedParameterName,
                "Speed",
                "MoveSpeed",
                "Movement",
                "Velocity",
                "MotionSpeed"
            };

            foreach (string candidate in candidateNames)
            {
                if (string.IsNullOrEmpty(candidate))
                {
                    continue;
                }

                int hash = Animator.StringToHash(candidate);
                if (HasAnimatorParameter(hash))
                {
                    _animIDSpeed = hash;
                    _speedParameterName = candidate;
                    return;
                }
            }

            _animIDSpeed = Animator.StringToHash(_speedParameterName);
        }

        private bool HasAnimatorParameter(int hash)
        {
            if (_animator == null)
            {
                return false;
            }

            foreach (AnimatorControllerParameter parameter in _animator.parameters)
            {
                if (parameter.nameHash == hash)
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateAnimatorState(float overrideSpeed = -1f)
        {
            if (_animator == null)
            {
                return;
            }

            if (_animIDSpeed != 0 && HasAnimatorParameter(_animIDSpeed))
            {
                float moveAmount = overrideSpeed >= 0f ? overrideSpeed : 0f;
                if (overrideSpeed < 0f)
                {
                    if (_agent != null)
                    {
                        moveAmount = _agent.velocity.magnitude / Mathf.Max(_agent.speed, 0.01f);
                    }
                    else
                    {
                        moveAmount = _currentState == AIState.Patrol || _currentState == AIState.Chase ? 1f : 0f;
                    }
                }

                _animator.SetFloat(_animIDSpeed, moveAmount);
                return;
            }

            string stateName = _idleStateName;
            if (_currentState == AIState.Chase)
            {
                stateName = _moveStateName;
            }
            else if (_currentState == AIState.Attack)
            {
                stateName = _attackStateName;
            }

            if (!string.IsNullOrEmpty(stateName))
            {
                _animator.Play(stateName);
            }
        }

        private void ExitAttackState()
        {
            if (_isChargingSpell)
            {
                _zoltraak.ReleaseCharge();
                _isChargingSpell = false;
            }
        }


    }
}