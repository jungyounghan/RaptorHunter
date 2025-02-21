using System;
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
public abstract class Controller : MonoBehaviour, IHittable
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

    protected Rigidbody getRigidbody
    {
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
            return _currentStamina / _fullStamina;
        }
    }

    [Header("스태미나 한도"), Range(Stat.MinFullStamina, Stat.MaxFullStamina)]
    protected float _fullStamina = 100;
    protected float _currentStamina = 0;  //현재 스태미나
    [Header("스태미나 회복 속도"), Range(Stat.MinRecoverStamina, Stat.MaxRecoverStamina)]
    protected float _recoverStamina = 10;
    [Header("돌진 속도"), Range(Stat.MinDashSpeed, Stat.MaxDashSpeed)]
    protected float _dashSpeed = 1;
    [Header("돌진 소모 비용"), Range(Stat.MinDashCost, Stat.MaxDashCost)]
    protected float _dashCost = 0.001f;
    [Header("후진 속도 비율"), Range(Stat.MinReverseRate, Stat.MaxReverseRate)]
    protected float _reverseRate = 0.5f;
    [Header("공격력"), SerializeField]
    protected uint _attackDamage = 1;
    [Header("최대 체력"), SerializeField]
    protected uint _fullLife = 10;
    protected uint _currentLife = 0;      //현재 체력

    public bool alive
    {
        get
        {
            return _currentLife > 0 || _currentLife == _fullLife;
        }
    }

    protected Action<float, float> _staminaAction = null;
    protected Action<uint, uint, Controller> _lifeAction = null;

    private static readonly float CenterDistance = 0.5f;
    private static readonly float LandDistance = 0.5f;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_currentStamina > _fullStamina)
        {
            _currentStamina = _fullStamina;
        }
        if (_currentLife > _fullLife)
        {
            _currentLife = _fullLife;
        }
    }
#endif

    protected bool IsGrounded()
    {
        return Physics.Raycast(getTransform.position + new Vector3(0, CenterDistance, 0), Vector3.down, CenterDistance + LandDistance) || getRigidbody.velocity == Vector3.zero;
    }

    protected float GetMoveSpeed(float direction, bool dash)
    {
        float speed = getNavMeshAgent.speed;
        if(dash == true)
        {
            speed += speed * _dashSpeed * staminaRate;
            _currentStamina -= _currentStamina * _dashCost;
            _staminaAction?.Invoke(_currentStamina, _fullStamina);
        }
        if(direction < 0)
        {
            speed *= _reverseRate;
        }
        return speed;
    }

    public void Initialize(Action<float, float> staminaAction, Action<uint, uint, Controller> lifeAction)
    {
        _staminaAction = staminaAction;
        _lifeAction = lifeAction;
    }

    public void Hit(Vector3 origin, Vector3 direction, uint damage)
    {
        if (alive == true)
        {
            if (_currentLife > damage)
            {
                _currentLife -= damage;
                getCharacter.DoHitAction(false);
            }
            else
            {
                _currentLife = 0;
                getCharacter.DoHitAction(_fullLife > 0);
            }
            _lifeAction?.Invoke(_currentLife, _fullLife, this);
        }
    }

    public void Revive()
    {
        _currentStamina = _fullStamina;
        _staminaAction?.Invoke(_currentStamina, _fullStamina);
        _currentLife = _fullLife;
        _lifeAction?.Invoke(_currentLife, _fullLife, this);
        getCharacter.DoReviveAction();
    }

    protected virtual void OnEnable()
    {
        StartCoroutine(DoProcess());
    }

    protected virtual void OnDisable()
    {
        StopAllCoroutines();
    }

    protected virtual void Update()
    {
        if (_currentStamina < _fullStamina)
        {
            _currentStamina += Time.deltaTime * _recoverStamina;
            if (_currentStamina > _fullStamina)
            {
                _currentStamina = _fullStamina;
            }
            _staminaAction?.Invoke(_currentStamina, _fullStamina);
        }
    }

    protected abstract IEnumerator DoProcess();

    public abstract void Set(Stat stat);
}