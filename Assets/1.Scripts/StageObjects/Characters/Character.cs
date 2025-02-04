using UnityEngine;

/// <summary>
/// 조종 가능한 추상 캐릭터 클래스
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public abstract class Character : MonoBehaviour
{
    private bool _hasTransform = false;

    private Transform _transform = null;

    private Transform getTransform {
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

    private bool _hasRigidbody = false;

    private Rigidbody _rigidbody = null;

    private Rigidbody getRigidbody {
        get
        {
            if(_hasRigidbody == false)
            {
                _hasRigidbody = true;
                _rigidbody = GetComponent<Rigidbody>();
            }
            return _rigidbody;
        }
    }

    private bool _hasAnimator = false;

    private Animator _animator = null;

    protected Animator getAnimator {
        get
        {
            if(_hasAnimator == false)
            {
                _hasAnimator = true;
                _animator = GetComponent<Animator>();
            }
            return _animator;
        }
    }

    private bool _alive = true;

    public bool alive {
        get
        {
            return _alive;
        }
        set
        {
            _alive = value;
        }
    }

    private bool _dash = false;

    public bool dash {
        private get
        {
            return _dash;
        }
        set
        {
            _dash = value;
            if(_currentStamina < _maxStamina && Vector3.Distance(getRigidbody.velocity, Vector3.zero) <= float.Epsilon)
            {
                _currentStamina += Time.deltaTime * _recoverStamina;
                if(_currentStamina > _maxStamina)
                {
                    _currentStamina = _maxStamina;
                }
            }
        }
    }

    public float staminaRate {
        get
        {
            return _currentStamina / _maxStamina;
        }
    }

    [SerializeField]
    private float _currentStamina = 0;
    [Header("스태미나 회복력"), SerializeField, Range(float.Epsilon, byte.MaxValue)]
    private float _recoverStamina = 10;
    [Header("스태미나 최대치"), SerializeField, Range(float.Epsilon, byte.MaxValue)]
    private float _maxStamina = 100;
    [Header("돌진 기본 비용"), SerializeField, Range(0, 1)]
    private float _dashCost = 0.01f;
    [Header("돌진 기본 속도"), SerializeField, Range(float.Epsilon, byte.MaxValue)]
    private float _dashSpeed = 1;
    [Header("후진 속도 비율"), SerializeField, Range(0, 1)]
    private float _reverseRate = 0.5f;
    [Header("기본 이동 속도"), SerializeField, Range(0, byte.MaxValue)]
    private float _moveSpeed = 4;
    [Header("기본 회전 속도"), SerializeField, Range(0, byte.MaxValue)]
    private float _turnSpeed = 4;
    [Header("도약 기본 비용"), SerializeField, Range(0, 1)]
    private float _jumpCost = 0.1f;
    [Header("도약 기본 속도"), SerializeField, Range(0, byte.MaxValue)]
    private float _jumpSpeed = 4;
    [Header("도약 가능 거리"), SerializeField, Range(0, byte.MaxValue)]
    private float _jumpDistance = 0.501f;

#if UNITY_EDITOR
    [Header("도약 거리 색깔"), SerializeField]
    private Color _jumpColor = Color.blue;

    private void OnDrawGizmos()
    {
        Debug.DrawRay(getTransform.position, Vector3.down * _jumpDistance, _jumpColor);
    }
#endif

    private void Awake()
    {
        _currentStamina = _maxStamina;
    }

    public virtual bool TryTurnRight()
    {
        if (_alive == true)
        {
            getRigidbody.MoveRotation(getRigidbody.rotation * Quaternion.Euler(0, +_turnSpeed * Mathf.Rad2Deg * Time.deltaTime, 0));
            return true;
        }
        return false;
    }

    public virtual bool TryTurnLeft()
    {
        if (_alive == true)
        {
            getRigidbody.MoveRotation(getRigidbody.rotation * Quaternion.Euler(0, -_turnSpeed * Mathf.Rad2Deg * Time.deltaTime, 0));
            return true;
        }
        return false;
    }

    public virtual bool TryMoveForward()
    {
        if (_alive == true)
        {
            float speed = _moveSpeed;
            if(_dash == true)
            {
                speed += _dashSpeed * staminaRate;
                _currentStamina -= _currentStamina * _dashCost;
            }
            getRigidbody.MovePosition(getRigidbody.position + getTransform.forward.normalized * speed * Time.deltaTime);
            return true;
        }
        return false;
    }

    public virtual bool TryMoveBackward()
    {
        if (_alive == true)
        {
            float speed = _moveSpeed;
            if (_dash == true)
            {
                speed += _dashSpeed * staminaRate;
                _currentStamina -= _currentStamina * _dashCost;
            }
            getRigidbody.MovePosition(getRigidbody.position - getTransform.forward.normalized * speed * _reverseRate * Time.deltaTime);
            return true;
        }
        return false;
    }

    public virtual bool TryJump()
    {
        if (_alive == true && Physics.Raycast(getTransform.position, Vector3.down, _jumpDistance) == true)
        {
            getRigidbody.velocity += Vector3.up * _jumpSpeed * staminaRate;
            _currentStamina -= _currentStamina * _jumpCost;
            return getRigidbody.velocity.y >= _jumpDistance;
        }
        return false;
    }

    public abstract bool TryAttack();
}