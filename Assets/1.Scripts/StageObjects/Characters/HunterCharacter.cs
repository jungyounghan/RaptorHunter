using UnityEngine;

/// <summary>
/// 조종 가능한 사냥꾼 캐릭터 클래스
/// </summary>
public sealed class HunterCharacter : Character
{
    private readonly int _moveHashIndex = Animator.StringToHash("Move");
    private readonly int _turnHashIndex = Animator.StringToHash("Turn");

    private void OnAnimatorIK(int layerIndex)
    {
        
    }

    public override void DoJumpAction()
    {

    }

    public override void DoLandAction()
    {

    }

    public override void DoStopAction()
    {
        getAnimator.SetFloat(_turnHashIndex, 0);
        getAnimator.SetFloat(_moveHashIndex, 0);
    }

    public override void DoMoveAction(Vector2 direction, float speed, bool dash)
    {
        if(dash == true)
        {
            direction *= 2;
        }
        getAnimator.SetFloat(_turnHashIndex, direction.x);
        getAnimator.SetFloat(_moveHashIndex, direction.y);
    }
}