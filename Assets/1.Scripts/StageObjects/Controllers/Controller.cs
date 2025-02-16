using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 특정 플레이어 객체를 조종할 수 있는 추상 컨트롤러 클래스 
/// </summary>
[RequireComponent(typeof(Character))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
[DisallowMultipleComponent]
public abstract class Controller : MonoBehaviour
{
    private bool _hasTransform = false;

    private Transform _transform = null;

    protected Transform getTransform {
        get
        {
            if (_hasTransform == false)
            {
                _hasTransform = true;
                _transform = transform;
            }
            return _transform;
        }
    }

    private bool _hasCharacter = false;

    private Character _character = null;

    protected Character getCharacter {
        get
        {
            if (_hasCharacter == false)
            {
                _hasCharacter = TryGetComponent(out _character);
            }
            return _character;
        }
    }

    private bool _hasRigidbody = false;

    private Rigidbody _rigidbody = null;

    protected Rigidbody getRigidbody {
        get
        {
            if(_hasRigidbody == false)
            {
                _hasRigidbody = TryGetComponent(out _rigidbody);
            }
            return _rigidbody;
        }
    }

    private bool _hasNavMeshAgent = false;

    private NavMeshAgent _navMeshAgent = null;

    protected NavMeshAgent getNavMeshAgent
    {
        get
        {
            if(_hasNavMeshAgent == false)
            {
                _hasNavMeshAgent = TryGetComponent(out _navMeshAgent);
            }
            return _navMeshAgent;
        }
    }

    private float staminaRate {
        get
        {
            return _currentStamina / _maxStamina;
        }
    }

    [Header("현재 스태미나"), SerializeField]
    private float _currentStamina = 0;
    [Header("스태미나 회복력"), SerializeField, Range(float.Epsilon, byte.MaxValue)]
    private float _recoverStamina = 10;
    [Header("스태미나 최대치"), SerializeField, Range(float.Epsilon, byte.MaxValue)]
    private float _maxStamina = 100;
    [Header("돌진 기본 비용"), SerializeField, Range(0, 1)]
    private float _dashCost = 0.001f;
    [Header("돌진 기본 속도"), SerializeField, Range(float.Epsilon, byte.MaxValue)]
    private float _dashSpeed = 1;
    [Header("후진 속도 비율"), SerializeField, Range(0, 1)]
    private float _reverseRate = 0.5f;
    [Header("도약 기본 비용"), SerializeField, Range(0, 1)]
    private float _jumpCost = 0.1f;
    [Header("도약 기본 속도"), SerializeField, Range(0, byte.MaxValue)]
    private float _jumpSpeed = 5;
    [Header("도약 가능 거리"), SerializeField, Range(0, 1)]
    private float _jumpDistance = 0.1f;

    private static readonly float AddJumpDistance = 0.2f;

#if UNITY_EDITOR
    [Header("도약 거리 색깔"), SerializeField]
    private Color _jumpColor = Color.blue;

    private void OnDrawGizmos()
    {
        Gizmos.color = _jumpColor;
        Gizmos.DrawRay(getTransform.position + new Vector3(0, _jumpDistance, 0), Vector3.down * (_jumpDistance + AddJumpDistance));
    }
#endif

    private void Update()
    {
        if (_currentStamina < _maxStamina)
        {
            _currentStamina += Time.deltaTime * _recoverStamina;
            if (_currentStamina > _maxStamina)
            {
                _currentStamina = _maxStamina;
            }
        }
    }

    protected bool IsGrounded()
    {
        return Physics.Raycast(getTransform.position + new Vector3(0, _jumpDistance, 0), Vector3.down, _jumpDistance + AddJumpDistance);
    }

    protected float GetMoveSpeed(float direction, bool dash)
    {
        float speed = getNavMeshAgent.speed;
        if(dash == true)
        {
            speed += speed * _dashSpeed * staminaRate;
            _currentStamina -= _currentStamina * _dashCost;
        }
        if(direction < 0)
        {
            speed *= _reverseRate;
        }
        return speed;
    }

    protected IEnumerator DoJump(float walk, bool dash)
    {
        if (IsGrounded() == true)
        {
            float value = _jumpSpeed * staminaRate;
            _currentStamina -= _currentStamina * _jumpCost;
            if (value >= _jumpDistance /*&& getRigidbody.SweepTest(getRigidbody.velocity.normalized, out RaycastHit hit, getTransform.forward.magnitude * 0.5f) == false*/)
            {
                float speed = getNavMeshAgent.speed;
                if (dash == true)
                {
                    speed += speed * _dashSpeed * staminaRate;
                }
                if (walk < 0)
                {
                    speed *= _reverseRate;
                }
                getNavMeshAgent.isStopped = true;
                getNavMeshAgent.enabled = false;
                Vector3 forward = getTransform.forward;
                getRigidbody.velocity = new Vector3(forward.x * walk * speed, value, forward.z * walk * speed);
                getCharacter.DoJumpAction();
                yield return new WaitForSeconds(AddJumpDistance);
                while (IsGrounded() == false)
                {
                    if (getRigidbody.SweepTest(getRigidbody.velocity.normalized, out RaycastHit hit, getCharacter.GetForwardSize()))
                    {
                        getRigidbody.velocity = new Vector3(0, getRigidbody.velocity.y, 0);
                    }
                    yield return null;
                }
                getCharacter.DoLandAction();
                getRigidbody.velocity = Vector3.zero;
                getNavMeshAgent.enabled = true;
            }
        }
    }

    protected virtual void OnEnable()
    {
        _currentStamina = _maxStamina;
        StartCoroutine(DoProcess());
    }

    protected virtual void OnDisable()
    {
        _currentStamina = 0;
        StopAllCoroutines();
    }

    protected abstract IEnumerator DoProcess();
}