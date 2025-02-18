using System.Collections;
using UnityEngine;

/// <summary>
/// Ư�� �÷��̾� ��ü�� ������ �� �ִ� �Է� ��Ʈ�ѷ� Ŭ���� 
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
                //Ű���� ����
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
                //���콺 ����
                Camera camera = Camera.main;
                if (camera != null)
                {
                    Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                    getCharacter.LookAt(ray.origin + ray.direction * AimDistance);
                    //if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layer))
                    //{
                    //    Vector3 point = hit.point;
                    //    //�÷��̾�� �������� ���� ���Ͱ� ���� ��߳��ٸ�(�÷��̾� �޹��⿡ �������� �����ٸ� ������ ��ȯ��)
                    //    if (Vector3.Dot(forward, point - position) <= 0)
                    //    {
                    //        ray.direction.Normalize(); // ���� ���� ����ȭ                    
                    //        float dot = -Vector3.Dot(forward, position); // ��� �������� dot �� ���                        
                    //        float denominator = Vector3.Dot(forward, ray.direction); // ���� ������ ���� ���Ͱ� �̷�� ���� ��
                    //        // ������ 0�̸� ���� ������ ���� => ������ ����
                    //        if (Mathf.Abs(denominator) >= Mathf.Epsilon)
                    //        {
                    //            float t = -(Vector3.Dot(forward, ray.origin) + dot) / denominator; // ���� ������ �����ϴ� ���� t ���
                    //            // t < 0 �̸� ��� ���ʿ��� ���� (������ ��� �ݴ����)
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
                //���� ������ �� ����
            }
            yield return null;
        }
    }
}