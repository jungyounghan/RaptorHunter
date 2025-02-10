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
        getCharacter.Initialize();
        while (true)
        {
            bool dash = Input.GetButton(DashKey);
            bool jump = Input.GetButtonDown(JumpKey);
            bool attack = Input.GetButton(AttackKey);
            float walk = Input.GetAxis(VerticalKey);
            float turn = Input.GetAxis(HorizontalKey);
            float deltaTime = Time.deltaTime;
            Vector2 direction = getCharacter.GetMoveSpeed(new Vector2(turn * deltaTime, getNavMeshAgent.speed * walk * deltaTime), dash);
            Vector3 position = getTransform.position;
            getNavMeshAgent.Move(getTransform.forward * direction.y);
            if (jump == true && getCharacter.TryJump(getTransform.position) == true)
            {
                getNavMeshAgent.enabled = false;
                getCharacter.DoJumpAction();
                Debug.Log("����");
                yield return new WaitWhile(() => getCharacter.IsGrounded(getTransform.position));
                Debug.Log("����");
                getNavMeshAgent.enabled = true;
            }
            else if (getTransform.position != position)
            {
                getCharacter.DoMoveAction(direction, dash);
            }
            else
            {
                getCharacter.DoStopAction(true);
            }
            yield return null;
        }
    }
}