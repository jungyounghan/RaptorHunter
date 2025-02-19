using System.Collections;
using UnityEngine;

/// <summary>
/// 특정 플레이어 객체를 조종할 수 있는 입력 컨트롤러 클래스 
/// </summary>
public sealed class InputController : Controller
{
    private static readonly string VerticalKey = "Vertical";
    private static readonly string HorizontalKey = "Horizontal";
    private static readonly string DashKey = "Dash";
    private static readonly string AttackKey = "Fire1";

    private static readonly float AimDistance = 5;

    protected override void Update()
    {
        base.Update();
        if (alive == true)
        {
            Camera camera = Camera.main;
            if (camera != null)
            {
                Vector3 position = getTransform.position;
                Vector3 forward = getTransform.forward;
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                Vector3 point = ray.origin + ray.direction * AimDistance;
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity) && Vector3.Dot(forward, hit.point - position) > 0)
                {
                    point = hit.point;
                }
                getCharacter.LookAt(point);
            }
        }
    }

    protected override IEnumerator DoProcess()
    {
        while (true)
        {
            if (alive == true)
            {
                getNavMeshAgent.enabled = IsGrounded();
                if (getNavMeshAgent.enabled == true)
                {
                    //키보드 구간
                    bool dash = Input.GetButton(DashKey);
                    bool attack = Input.GetButton(AttackKey);
                    float walk = Input.GetAxis(VerticalKey);
                    float turn = Input.GetAxis(HorizontalKey);
                    float deltaTime = Time.deltaTime; ;
                    Vector2 direction = new Vector2(turn, walk);
                    float speed = GetMoveSpeed(direction.y, dash);
                    Vector3 position = getTransform.position;
                    Vector3 forward = getTransform.forward;
                    Quaternion rotation = getTransform.rotation;
                    getTransform.rotation = Quaternion.Slerp(rotation, Quaternion.Euler(0, turn, 0) * rotation, deltaTime * getNavMeshAgent.angularSpeed);
                    getNavMeshAgent.Move(forward.normalized * direction.y * deltaTime * speed);
                    if (position != getTransform.position || turn != 0)
                    {
                        getCharacter.DoMoveAction(direction, speed, speed > getNavMeshAgent.speed || (walk < 0 && dash));
                    }
                    else
                    {
                        getCharacter.DoStopAction();
                    }
                    
                    yield return null;
                }
                else
                {
                    getCharacter.DoJumpAction();
                    yield return new WaitUntil(() => IsGrounded());
                    getCharacter.DoLandAction();
                }
            }
        }
    }
}