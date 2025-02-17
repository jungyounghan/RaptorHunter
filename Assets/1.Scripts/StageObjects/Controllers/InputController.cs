using System.Collections;
using UnityEngine;

/// <summary>
/// Ư�� �÷��̾� ��ü�� ������ �� �ִ� �Է� ��Ʈ�ѷ� Ŭ���� 
/// </summary>
public sealed class InputController : Controller
{
    private const string DefaultLayer = "Default";
    private const string AllyLayer = "Ally";
    private const string EnemyLayer = "Enemy";

    private static readonly string VerticalKey = "Vertical";
    private static readonly string HorizontalKey = "Horizontal";
    private static readonly string DashKey = "Dash";
    private static readonly string AttackKey = "Fire1";

    protected override IEnumerator DoProcess()
    {
        while (true)
        {
            bool dash = Input.GetButton(DashKey);
            bool attack = Input.GetButton(AttackKey);
            float walk = Input.GetAxis(VerticalKey);
            float turn = Input.GetAxis(HorizontalKey);
            float deltaTime = Time.deltaTime;;
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
            int layer = LayerMask.GetMask(DefaultLayer);
            switch(LayerMask.LayerToName(gameObject.layer))
            {
                case AllyLayer:
                    layer |= LayerMask.GetMask(EnemyLayer);
                    break;
                case EnemyLayer:
                    layer |= LayerMask.GetMask(AllyLayer);
                    break;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layer))
            {
                //Vector3.Distance(position, point)> 3�̻��ΰ��

                Vector3 point = hit.point;
                if (Vector3.Dot(forward, point - position) >= 1)
                {
                    getCharacter.LookAt(point);
                }
                else
                {
                    Vector3 adjustedPoint = position + forward * 1f; // forward �������� 1��ŭ ��
                    adjustedPoint.x = point.x; // ���콺�� X�� ����
                    adjustedPoint.z = point.z;

                    // 2. ������ ��ǥ�� �ٶ󺸵��� ����
                    getCharacter.LookAt(adjustedPoint);
                }
            }
            yield return null;
        }
    }
}