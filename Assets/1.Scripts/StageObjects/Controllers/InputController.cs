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

    protected override IEnumerator DoProcess()
    {
        while (true)
        {
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
                //마우스 구간
                Camera camera = Camera.main;
                if (camera != null)
                {
                    Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                    getCharacter.LookAt(ray.origin + ray.direction * AimDistance);
                    //if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layer))
                    //{
                    //    Vector3 point = hit.point;
                    //    //플레이어와 조준점의 방향 벡터가 서로 어긋난다면(플레이어 뒷방향에 조준점이 잡힌다면 음수를 반환함)
                    //    if (Vector3.Dot(forward, point - position) <= 0)
                    //    {
                    //        ray.direction.Normalize(); // 방향 벡터 정규화                    
                    //        float dot = -Vector3.Dot(forward, position); // 평면 방정식의 dot 값 계산                        
                    //        float denominator = Vector3.Dot(forward, ray.direction); // 평면과 광선의 방향 벡터가 이루는 내적 값
                    //        // 내적이 0이면 평면과 광선이 평행 => 교차점 없음
                    //        if (Mathf.Abs(denominator) >= Mathf.Epsilon)
                    //        {
                    //            float t = -(Vector3.Dot(forward, ray.origin) + dot) / denominator; // 평면과 광선이 교차하는 비율 t 계산
                    //            // t < 0 이면 평면 뒤쪽에서 교차 (광선이 평면 반대방향)
                    //            if (t >= 0)
                    //            {
                    //                point = ray.origin + ray.direction * t;
                    //                if(Physics.Raycast(point + forward, ray.direction, out hit, Mathf.Infinity, layer))
                    //                {
                    //                    point = hit.point;
                    //                }
                    //            }
                    //        }
                    //    }
                    //    getCharacter.LookAt(point);
                    //}
                }
                if (attack == true)
                {

                }
            }
            else
            {
                //땅에 떨어질 때 까지
            }
            yield return null;
        }
    }
}