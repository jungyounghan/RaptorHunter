using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������ ���� ĳ���� Ŭ����
/// </summary>
public class HunterCharacter : Character
{
    public override bool TryJump()
    {
        bool tryJump = base.TryJump();
        if(tryJump == true)
        {
            //�ִϸ��̼�
        }
        return tryJump;
    }

    public override bool TryAttack()
    {
        return false;
    }
}
