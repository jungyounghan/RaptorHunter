using UnityEngine;

/// <summary>
/// 조종 가능한 추상 캐릭터 클래스
/// </summary>
[RequireComponent(typeof(Animator))]
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
    protected Transform _target = null;

    private static readonly int ExitHashIndex = Animator.StringToHash("Exit");
    private static readonly float LinearInterpolation = 10;
    //protected const string AllyLayer = "Ally";
    //protected const string EnemyLayer = "Enemy";
    //protected const string DefaultLayer = "Default";


    public virtual void LookAt(Vector3 position)
    {
        if (_target != null)
        {
            _target.position = Vector3.Lerp(_target.position, position, Time.deltaTime * LinearInterpolation);
        }
    }
    public virtual void DoReviveAction()
    {
        getAnimator.SetTrigger(ExitHashIndex);
    }

    public abstract void DoJumpAction();

    public abstract void DoLandAction();

    public abstract void DoStopAction();

    public abstract void DoHitAction(bool dead);

    public abstract void DoMoveAction(Vector2 direction, float speed, bool dash);
}