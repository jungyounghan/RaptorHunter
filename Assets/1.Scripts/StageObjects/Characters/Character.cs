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

    public abstract void DoJumpAction();

    public abstract void DoLandAction();

    public abstract void DoStopAction();

    public abstract void DoMoveAction(Vector2 direction, float speed, bool dash);

    public abstract float GetForwardSize();
}