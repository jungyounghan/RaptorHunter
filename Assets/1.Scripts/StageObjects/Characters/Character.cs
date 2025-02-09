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


    public void PlayMoveAction(Vector2 speed, bool dash)
    {
        //코스트는 여기서 소모됨
    }

    public Vector2 GetMoveSpeed(Vector2 speed, bool dash)
    {
        return speed;
    }
}
