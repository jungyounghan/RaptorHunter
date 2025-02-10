using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 조종 가능한 사냥꾼 캐릭터 클래스
/// </summary>
public class HunterCharacter : Character
{
    private readonly int _moveHashIndex = Animator.StringToHash("Move");
    private readonly int _turnHashIndex = Animator.StringToHash("Turn");

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

    public override void DoMoveAction(Vector2 direction)
    {
        getAnimator.SetFloat(_turnHashIndex, direction.x);
        getAnimator.SetFloat(_moveHashIndex, direction.y);
    }
}