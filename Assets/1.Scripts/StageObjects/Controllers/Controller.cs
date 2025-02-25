using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 특정 플레이어 객체를 조종할 수 있는 추상 컨트롤러 클래스 
/// </summary>
[RequireComponent(typeof(Character))]
[RequireComponent(typeof(Collider))]
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

    public Character character {
        get
        {
            if (_hasCharacter == false)
            {
                _hasCharacter = TryGetComponent(out _character);
            }
            return _character;
        }
    }

    private bool _hasCollider = false;

    private Collider _collider = null;

    protected Collider getCollider {

        get
        {
            if (_hasCollider == false)
            {
                _hasCollider = TryGetComponent(out _collider);
            }
            return _collider;
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

    protected float staminaRate {
        get
        {
            return _currentStamina / _fullStamina;
        }
    }

    [Header("스태미나 한도"), Range(Stat.MinFullStamina, Stat.MaxFullStamina)]
    protected float _fullStamina = 100;
    [Header("현재 스태미나")]
    protected float _currentStamina = 0;
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
    [Header("현재 체력"), SerializeField]
    protected uint _currentLife = 0;

    public bool alive
    {
        get
        {
            return _currentLife > 0 || _currentLife == _fullLife;
        }
    }

    protected Action<uint, uint, Controller> _lifeAction = null;

    private static readonly string FloorTag = "Floor";
    protected static readonly string StairTag = "Stair";

    protected static readonly float RotationSpeed = 40;

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

    protected bool landing
    {
        private set;
        get;
    }

    private void OnTriggerStay(Collider other)
    {
        if (alive == true && (other.tag == FloorTag || other.tag == StairTag))
        {
            landing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == FloorTag || other.tag == StairTag)
        {
            landing = false;
        }
    }

    public bool IsHuman()
    {
        return character.IsHuman();
    }

    public Vector3 GetHitPoint()
    {
        return getCollider.bounds.center;
    }

    protected virtual void OnEnable()
    {
        getNavMeshAgent.enabled = false;
        StartCoroutine(DoProcess());
    }

    protected virtual void OnDisable()
    {
        StopAllCoroutines();
    }

    public virtual void Hit(Vector3 origin, Vector3 direction, uint damage)
    {
        if (alive == true)
        {
            if (_currentLife > damage)
            {
                _currentLife -= damage;
                character.DoHitAction(false);
            }
            else
            {
                landing = false;
                getCollider.enabled = false;
                getNavMeshAgent.enabled = false;
                _currentLife = 0;
                character.DoHitAction(_fullLife > 0);
            }
            _lifeAction?.Invoke(_currentLife, _fullLife, this);
        }
    }

    protected abstract IEnumerator DoProcess();

    public abstract void Set(Stat stat);

    public abstract void Revive();
}