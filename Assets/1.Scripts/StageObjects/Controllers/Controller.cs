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
    [Header("���� �⺻ ���"), SerializeField, Range(0, 1)]
    private float _jumpCost = 0.1f;
    [Header("���� �⺻ �ӵ�"), SerializeField, Range(0, byte.MaxValue)]
    private float _jumpSpeed = 5;
    [Header("���� ���� �Ÿ�"), SerializeField, Range(0, 1)]
    private float _jumpDistance = 0.1f;

    private static readonly float AddJumpDistance = 0.2f;

#if UNITY_EDITOR
    [Header("���� �Ÿ� ����"), SerializeField]
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

    protected IEnumerator DoJump(Vector2 direction)
    {
        if (IsGrounded() == true)
        {
            float value = _jumpSpeed * staminaRate;
            _currentStamina -= _currentStamina * _jumpCost;
            if (value >= _jumpDistance)
            {
                getNavMeshAgent.isStopped = true;
                getNavMeshAgent.enabled = false;
                getCharacter.DoJumpAction();
                Vector3 startPos = getTransform.position;
                Vector3 targetPos = startPos + getTransform.forward.normalized * direction.y; // �������� ����
                float elapsedTime = 0;
                bool jump = true;
                while ((jump == true && IsGrounded() == true) || (jump == false && IsGrounded() == false))
                {
                    elapsedTime += Time.deltaTime;
                    float t = elapsedTime / _jumpDistance;
                    float height = Mathf.Sin(t * Mathf.PI) * value; // ������ �
                    getTransform.position = Vector3.Lerp(startPos, targetPos, t) + Vector3.up * height;
                    if(jump == true && IsGrounded() == false)
                    {
                        jump = false;
                    }
                    yield return null;
                }
                getCharacter.DoLandAction();
                getNavMeshAgent.enabled = true;   // NavMeshAgent �ٽ� Ȱ��ȭ
                getNavMeshAgent.isStopped = false;
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