using Unity.VisualScripting;
using UnityEngine;

public class Test : MonoBehaviour
{
    private bool _hasTransform = false;

    private Transform _transform = null;

    private Transform getTransform
    {
        get
        {
            if (_hasTransform == false)
            {
                _hasTransform = true;
                _transform = transform;
            }
            return _transform;
        }
    }

    [Header("�ٶ� ����� Ʈ������"), SerializeField]
    private Transform _target;

    [SerializeField]
    private LayerMask layerMask;

#if UNITY_EDITOR

    private Color _targetColor = Color.black;

    private Color _leftColor = Color.red;

    private Color _rightColor = Color.blue;

    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false && _target != null)
        {
            Vector3 start = getTransform.position;
            Vector3 forward = getTransform.forward;
            Vector3 end = _target.position;
            Vector3 direction = end - start;
            Vector3 cross = Vector3.Cross(direction, Vector3.up);
            Debug.DrawLine(start, end, _targetColor);
            Debug.DrawLine(start, end - cross, _targetColor);
            Debug.DrawLine(start, end + cross, _targetColor);
            Debug.DrawRay(end, -cross, _leftColor);
            Debug.DrawRay(end, cross, _rightColor);
        }
    }
#endif

    private void Update()
    {
        if (_target != null)
        {
            Vector3 start = getTransform.position;   //������: O
            Vector3 end = _target.position;
            Vector3 direction = end - start;
            Vector3 cross = Vector3.Cross(direction, Vector3.up);
            Vector2 o = new Vector2(start.x, start.z);
            Vector2 a = new Vector2(end.x + cross.x, end.z + cross.z);
            Vector2 b = new Vector2(end.x - cross.x, end.z - cross.z);
            //if (Physics.Raycast(center, right, out RaycastHit rightHit) == true)
            //{
            //    Debug.DrawLine(center, rightHit.point, Color.blue);
            //}
            //else
            //{
            //    Debug.DrawLine(center, center + right, Color.blue);
            //}
            //if (Physics.Raycast(center, -right, out RaycastHit leftHit) == true)
            //{
            //    Debug.DrawLine(center, leftHit.point, Color.red);
            //}
            //else
            //{
            //    Debug.DrawLine(center, center - right, Color.red);
            //}
            Debug.DrawLine(new Vector3(o.x, start.y, o.y), new Vector3(a.x, start.y, a.y), _targetColor);
            Debug.DrawLine(new Vector3(o.x, start.y, o.y), new Vector3(b.x, start.y, b.y), _targetColor);
        }
    }

    public float GetAngle(Vector2 A, Vector2 B, Vector2 C)
    {
        // ���� ���
        Vector2 BA = A - B;
        Vector2 BC = C - B;
        // ���� ���
        float dotProduct = Vector2.Dot(BA, BC);
        // ũ�� ��� (����ȭ X)
        float magnitudeBA = BA.magnitude;
        float magnitudeBC = BC.magnitude;
        // �ڻ��� ��Ÿ ���
        float cosTheta = dotProduct / (magnitudeBA * magnitudeBC);
        // ���� -> �� ��ȯ
        float angle = Mathf.Acos(cosTheta) * Mathf.Rad2Deg;
        return angle;
    }

    //���� ���� x�� z�� ���� radian float���� ��ȯ�����ִ� �Լ�
    public static float GetForwardAngleXZ(Transform transform)
    {
        Vector3 forwardXZ = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        float angleRadians = Mathf.Atan2(forwardXZ.x, forwardXZ.z);
        return angleRadians;
    }
}