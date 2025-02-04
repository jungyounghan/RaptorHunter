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

    [Header("������ ����� Ʈ������"), SerializeField]
    private Transform _target = null;
    [Header("������ ������ ���� ����"), SerializeField]
    private bool _orbit = false;
    [Header("������ ������ ����"), SerializeField]
    private Vector3 _offset = Vector3.zero;
    [Header("���� ���� �ӵ�"), SerializeField]
    private float _smoothingSpeed = 10f;

#if UNITY_EDITOR
    [Header("��ġ ǥ�� ����"), SerializeField]
    private Color _gizmoColor = Color.blue;

    private void OnDrawGizmos()
    {
        if (_target != null)
        {
            Vector3 start = _target.position;
            Vector3 direction = (start + _offset);
            Debug.DrawRay(start, direction, _gizmoColor);
        }
    }

    private void OnValidate()
    {
        if (_target != null)
        {
            getTransform.position = _target.position + _offset;
            if(_orbit == true)
            {
                Vector3 eulerAngles = _target.eulerAngles;
                eulerAngles.x = getTransform.rotation.eulerAngles.x;
                getTransform.rotation = Quaternion.Euler(eulerAngles);
            }
        }
    }
#endif 

    private void LateUpdate()
    {
        if(_target != null)
        {
            Vector3 start = getTransform.position;
            Vector3 end = _target.position + _offset;
            getTransform.position = Vector3.Lerp(start, end, _smoothingSpeed * Time.deltaTime);
            if (_orbit == true)
            {
                Vector3 eulerAngles = _target.eulerAngles;
                eulerAngles.x = getTransform.rotation.eulerAngles.x;
                getTransform.rotation = Quaternion.Euler(eulerAngles);
            }
        }
    }
}