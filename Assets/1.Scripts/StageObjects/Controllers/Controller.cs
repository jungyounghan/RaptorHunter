using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Ư�� �÷��̾� ��ü�� ������ �� �ִ� �߻� ��Ʈ�ѷ� Ŭ���� 
/// </summary>
[RequireComponent(typeof(Character))]
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

    [Header("���� ���¹̳�"), SerializeField]
    private float _currentStamina = 0;
    [Header("���¹̳� ȸ����"), SerializeField, Range(float.Epsilon, byte.MaxValue)]
    private float _recoverStamina = 10;
    [Header("���¹̳� �ִ�ġ"), SerializeField, Range(float.Epsilon, byte.MaxValue)]
    private float _maxStamina = 100;
    [Header("���� �⺻ ���"), SerializeField, Range(0, 1)]
    private float _dashCost = 0.001f;
    [Header("���� �⺻ �ӵ�"), SerializeField, Range(float.Epsilon, byte.MaxValue)]
    private float _dashSpeed = 1;
    [Header("���� �ӵ� ����"), SerializeField, Range(0, 1)]
    private float _reverseRate = 0.5f;

    [Header("���ݷ�"), SerializeField]
    private uint _damage = 1;
    [Header("�ִ� ü��"), SerializeField]
    private uint _maxLife = 10;
    [Header("���� ü��"), SerializeField]
    private uint _currentLife = 0;

    private Action<float, float> _staminaAction = null;
    private Action<uint, uint> _lifeAction = null;

    private static readonly float CenterDistance = 0.5f;
    private static readonly float LandDistance = 0.2f;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_currentStamina > _maxStamina)
        {
            _currentStamina = _maxStamina;
        }
        if (_currentLife > _maxLife)
        {
            _currentLife = _maxLife;
        }
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
            _staminaAction?.Invoke(_currentStamina, _maxStamina);
        }
    }

    protected bool IsGrounded()
    {
        return Physics.Raycast(getTransform.position + new Vector3(0, CenterDistance, 0), Vector3.down, CenterDistance + LandDistance);
    }

    protected float GetMoveSpeed(float direction, bool dash)
    {
        float speed = getNavMeshAgent.speed;
        if(dash == true)
        {
            speed += speed * _dashSpeed * staminaRate;
            _currentStamina -= _currentStamina * _dashCost;
            _staminaAction?.Invoke(_currentStamina, _maxStamina);
        }
        if(direction < 0)
        {
            speed *= _reverseRate;
        }
        return speed;
    }

    public void Initialize(Action<float, float> staminaAction, Action<uint, uint> lifeAction)
    {
        _staminaAction = staminaAction;
        _lifeAction = lifeAction;
    }

    public void Hit(uint damage)
    {
        if (_currentLife > damage)
        {
            _currentLife -= damage;
            getCharacter.DoHitAction(false);
        }
        else
        {
            _currentLife = 0;
            getCharacter.DoHitAction(_maxLife > 0);
        }
        _lifeAction?.Invoke(_currentLife, _maxLife);
    }

    public void Revive()
    {
        _currentStamina = _maxStamina;
        _staminaAction?.Invoke(_currentStamina, _maxStamina);
        _currentLife = _maxLife;
        _lifeAction?.Invoke(_currentLife, _maxLife);
    }

    protected virtual void OnEnable()
    {
        StartCoroutine(DoProcess());
    }

    protected virtual void OnDisable()
    {
        StopAllCoroutines();
    }

    protected abstract IEnumerator DoProcess();
}