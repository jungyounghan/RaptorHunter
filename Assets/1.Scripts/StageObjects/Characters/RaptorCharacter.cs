using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 조종 가능한 랩터 캐릭터 클래스
/// </summary>
public class RaptorCharacter : Character
{
    public override void DoJumpAction()
    {
    }

    public override void DoLandAction()
    {
    }
    public override void DoStopAction()
    {
    }

    public override void DoMoveAction(Vector2 direction, float speed, bool dash)
    {

    }

    public override float GetForwardSize()
    {
        return 1.5f;
    }
}