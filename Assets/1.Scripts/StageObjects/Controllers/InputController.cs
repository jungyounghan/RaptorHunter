using System.Collections;
using UnityEngine;

/// <summary>
/// 특정 플레이어 객체를 조종할 수 있는 입력 컨트롤러 클래스 
/// </summary>
public sealed class InputController : Controller
{
    private static readonly string VerticalKey = "Vertical";
    private static readonly string HorizontalKey = "Horizontal";
    private static readonly string JumpKey = "Jump";
    private static readonly string DashKey = "Dash";
    private static readonly string AttackKey = "Fire1";

    protected override IEnumerator DoProcess()
    {
        while (true)
        {
            float move = Input.GetAxis(VerticalKey);
            float turn = Input.GetAxis(HorizontalKey);
            bool jump = Input.GetButton(JumpKey);
            bool dash = Input.GetButton(DashKey);
            bool attack = Input.GetButton(AttackKey);
            getCharacter.Move(move);
            getCharacter.Turn(turn);
            getCharacter.dash = dash;
            yield return null;
        }
    }
}