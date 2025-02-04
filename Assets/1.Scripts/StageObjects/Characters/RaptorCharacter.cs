using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 조종 가능한 랩터 캐릭터 클래스
/// </summary>
public class RaptorCharacter : Character
{
    public override bool TryAttack()
    {
        return false;
    }
}
