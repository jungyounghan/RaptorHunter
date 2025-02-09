using System.Collections;
using UnityEngine;

/// <summary>
/// Ư�� �÷��̾� ��ü�� ������ �� �ִ� �Է� ��Ʈ�ѷ� Ŭ���� 
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
            bool dash = Input.GetButton(DashKey);
            bool jump = Input.GetButton(JumpKey);
            bool attack = Input.GetButton(AttackKey);
            float walk = Input.GetAxis(VerticalKey);
            float turn = Input.GetAxis(HorizontalKey);
            float deltaTime = Time.deltaTime;
            Vector2 direction = getCharacter.GetMoveSpeed(new Vector2(turn * deltaTime, getNavMeshAgent.speed * walk * deltaTime), dash);
            Vector3 position = getTransform.position;
            getNavMeshAgent.Move(getTransform.forward * direction.y);
            if(getTransform.position != position)
            {
                getCharacter.PlayMoveAction(direction, dash);
            }
            yield return null;
        }
    }
}