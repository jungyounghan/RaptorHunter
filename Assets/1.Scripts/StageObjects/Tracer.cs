using UnityEngine;

[DisallowMultipleComponent]
public class Tracer : MonoBehaviour
{
    private bool _hasTransform = false;

    private Transform _transform = null;

    private Transform getTransform {
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

    public Transform target {
        get
        {
            return _target;
        }
        set
        {
            _target = value;
        }
    }

    [Header("������ ����� Ʈ������"), SerializeField]
    private Transform _target = null;
    [Header("������ ������ ����"), SerializeField]
    private Vector3 _offset = Vector3.zero;
    [Header("���� ���� �ӵ�"), SerializeField]
    private float _smoothingSpeed = 10f;

    private enum Point: byte
    {
        First,
        Second,
        Third
    }

    [Header("������ ����� ����"), SerializeField]
    private Point _point = Point.Third;

#if UNITY_EDITOR
    [Header("��ġ ǥ�� ����"), SerializeField]
    private Color _gizmoColor = Color.blue;

    private void OnDrawGizmos()
    {
        if (_target != null)
        {
            Debug.DrawLine(getTransform.position, _target.position, _gizmoColor);
        }
    }

    private void OnValidate()
    {
        if (_target != null)
        {

        }
    }
#endif 

    private void LateUpdate()
    {
        if(_target != null)
        {
            Vector3 start = getTransform.position;
            Vector3 end = _target.position + _offset;
            switch(_point)
            {
                case Point.First:
                case Point.Second:
                case Point.Third:
                    getTransform.position = Vector3.Lerp(start, end, _smoothingSpeed * Time.deltaTime);
                    break;
            }
        }
    }
}