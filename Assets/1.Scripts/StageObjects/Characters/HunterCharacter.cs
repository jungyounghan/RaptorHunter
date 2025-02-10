using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������ ��ɲ� ĳ���� Ŭ����
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