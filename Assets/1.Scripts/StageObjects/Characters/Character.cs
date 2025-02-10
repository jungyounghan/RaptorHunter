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

    public abstract void DoJumpAction();

    public abstract void DoLandAction();

    public abstract void DoStopAction();

    public abstract void DoMoveAction(Vector2 direction);
}