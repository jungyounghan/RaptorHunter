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
            Vector2 direction = new Vector2(turn, walk);
            if (jump == true)
            {
                yield return StartCoroutine(DoJump(direction));
            }
            else
            {
                float deltaTime = Time.deltaTime;
                float speed = GetMoveSpeed(direction.y, dash);
                Vector3 position = getTransform.position;
                Quaternion rotation = getTransform.rotation;
                getTransform.rotation = Quaternion.Slerp(rotation, Quaternion.Euler(0, turn, 0) * rotation, deltaTime * getNavMeshAgent.angularSpeed);
                getNavMeshAgent.Move(getTransform.forward.normalized * direction.y * deltaTime * speed);
                //getNavMeshAgent.SetDestination(Vector3.zero); 

                //getRigidbody.velocity = getTransform.forward * direction.y * speed;
                if (position != getTransform.position || turn != 0)
                {
                    //Debug.Log(getRigidbody.velocity);
                    getCharacter.DoMoveAction(direction, speed, speed > getNavMeshAgent.speed || (walk < 0 && dash));
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