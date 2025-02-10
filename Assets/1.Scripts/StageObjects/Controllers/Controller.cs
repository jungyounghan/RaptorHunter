using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 특정 플레이어 객체를 조종할 수 있는 추상 컨트롤러 클래스 
/// </summary>
[RequireComponent(typeof(Rigidbody))]
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

    private bool _hasRigidbody = false;

    private Rigidbody _rigidbody = null;

    protected Rigidbody getRigidbody
    {
        get
        {
            if (_hasRigidbody == false)
            {
                _hasRigidbody = TryGetComponent(out _rigidbody);
            }
            return _rigidbody;
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

    [SerializeField]
    protected float _currentStamina = 0;
    [Header("스태미나 회복력"), SerializeField, Range(float.Epsilon, byte.MaxValue)]
    protected float _recoverStamina = 10;
    [Header("스태미나 최대치"), SerializeField, Range(float.Epsilon, byte.MaxValue)]
    protected float _maxStamina = 100;
    [Header("돌진 기본 비용"), SerializeField, Range(0, 1)]
    protected float _dashCost = 0.01f;
    [Header("돌진 기본 속도"), SerializeField, Range(float.Epsilon, byte.MaxValue)]
    protected float _dashSpeed = 1;
    [Header("후진 속도 비율"), SerializeField, Range(0, 1)]
    protected float _reverseRate = 0.5f;
    [Header("도약 기본 비용"), SerializeField, Range(0, 1)]
    protected float _jumpCost = 0.1f;
    [Header("도약 기본 속도"), SerializeField, Range(0, byte.MaxValue)]
    protected float _jumpSpeed = 4;
    [Header("도약 가능 거리"), SerializeField, Range(0, byte.MaxValue)]
    protected float _jumpDistance = 0.501f;

    private void OnEnable()
    {
        StartCoroutine(DoProcess());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    protected abstract IEnumerator DoProcess();
}