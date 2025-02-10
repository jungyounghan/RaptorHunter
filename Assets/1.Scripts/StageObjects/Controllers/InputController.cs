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
            bool dash = Input.GetButton(DashKey);
            bool jump = Input.GetButtonDown(JumpKey);
            bool attack = Input.GetButton(AttackKey);
            float walk = Input.GetAxis(VerticalKey);
            float turn = Input.GetAxis(HorizontalKey);
            if(jump == true && TryJump() == true)
            {
                getNavMeshAgent.enabled = false;
                getCharacter.DoJumpAction();
                yield return new WaitWhile(() => IsGrounded());
                yield return new WaitUntil(() => IsGrounded());
                getCharacter.DoLandAction();
                getNavMeshAgent.enabled = true;
            }
            else
            {
                float deltaTime = Time.deltaTime;
                Vector2 direction = new Vector2(turn, walk);
                float speed = GetMoveSpeed(direction.y, dash);
                Vector3 position = getTransform.position;
                getNavMeshAgent.Move(getTransform.forward.normalized * direction.y * deltaTime * speed);
                //회전(turn)도 필요함 direction.x
                if(position != getTransform.position)
                {
                    getCharacter.DoMoveAction(direction);
                }
                else
                {
                    getCharacter.DoStopAction();
                }
            }
            yield return null;
        }
    }
}