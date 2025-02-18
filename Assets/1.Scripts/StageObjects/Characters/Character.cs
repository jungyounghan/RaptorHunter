using UnityEngine;

/// <summary>
/// ���� ������ �߻� ĳ���� Ŭ����
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
    private Transform _target = null;

    protected const string AllyLayer = "Ally";
    protected const string EnemyLayer = "Enemy";
    protected const string DefaultLayer = "Default";

    public virtual void LookAt(Vector3 position)
    {
        if (_target != null)
        {
            _target.position = position;
        }
    }

    public abstract void DoJumpAction();

    public abstract void DoLandAction();

    public abstract void DoStopAction();

    public abstract void DoHitAction(bool dead);

    public abstract void DoMoveAction(Vector2 direction, float speed, bool dash);
}