using UnityEngine;
using FIMSpace.FProceduralAnimation;

/// <summary>
/// 조종 가능한 추상 캐릭터 클래스
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(RagdollAnimator2))]
[DisallowMultipleComponent]
public abstract class Character : MonoBehaviour
{
    private bool _hasAnimator = false;

    private Animator _animator = null;

    protected Animator getAnimator {
        get
        {
            if (_hasAnimator == false)
            {
                _hasAnimator = TryGetComponent(out _animator);
            }
            return _animator;
        }
    }

    [SerializeField]
    protected float _currentStamina = 0;
    [Header("스태미나 회복력"), SerializeField, Range(float.Epsilon, byte.MaxValue)]
    private float _recoverStamina = 10;
    [Header("스태미나 최대치"), SerializeField, Range(float.Epsilon, byte.MaxValue)]
    private float _maxStamina = 100;
    [Header("돌진 기본 비용"), SerializeField, Range(0, 1)]
    protected float _dashCost = 0.01f;
    [Header("돌진 기본 속도"), SerializeField, Range(float.Epsilon, byte.MaxValue)]
    protected float _dashSpeed = 1;
    [Header("후진 속도 비율"), SerializeField, Range(0, 1)]
    protected float _reverseRate = 0.5f;
    [Header("도약 기본 비용"), SerializeField, Range(0, 1)]
    private float _jumpCost = 0.1f;
    [Header("도약 기본 속도"), SerializeField, Range(0, byte.MaxValue)]
    private float _jumpSpeed = 4;
    [Header("도약 가능 거리"), SerializeField, Range(0, byte.MaxValue)]
    private float _jumpDistance = 0.501f;

    public void Initialize()
    {
        _currentStamina = _maxStamina;
    }

    public virtual void DoStopAction(bool alive)
    {
        switch (alive)
        {
            case true:
                if (_currentStamina < _maxStamina)
                {
                    _currentStamina += Time.deltaTime * _recoverStamina;
                    if (_currentStamina > _maxStamina)
                    {
                        _currentStamina = _maxStamina;
                    }
                }
                break;
            case false:
                if (_currentStamina > 0)
                {
                    _currentStamina = 0;
                }
                break;
        }
    }

    public virtual void DoMoveAction(Vector2 speed, bool dash)
    {
        if (dash == true)
        {
            _currentStamina -= _currentStamina * _dashCost;
        }
    }

    public virtual void DoJumpAction()
    {

        _currentStamina -= _currentStamina * _jumpCost;
    }

    public bool IsGrounded(Vector3 point)
    {
        return Physics.Raycast(point, Vector3.down, _jumpDistance);
    }

    public bool TryJump(Vector3 point)
    {
        return IsGrounded(point) == true && _jumpSpeed * (_currentStamina / _maxStamina) >= _jumpDistance;
    }

    public Vector2 GetMoveSpeed(Vector2 speed, bool dash)
    {
        if (dash == true)
        {
            speed += speed * _dashSpeed * (_currentStamina / _maxStamina);
        }
        return speed;
    }
}