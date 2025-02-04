using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 조종 가능한 헌터 캐릭터 클래스
/// </summary>
public class HunterCharacter : Character
{
    public override bool TryJump()
    {
        bool tryJump = base.TryJump();
        if(tryJump == true)
        {
            //애니메이션
        }
        return tryJump;
    }

    public override bool TryAttack()
    {
        return false;
    }
}
