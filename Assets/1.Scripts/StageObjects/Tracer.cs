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

    [Header("추적할 대상의 트랜스폼"), SerializeField]
    private Transform _target = null;
    [Header("추적할 대상과의 공전 여부"), SerializeField]
    private bool _orbit = false;
    [Header("추적할 대상과의 간격"), SerializeField]
    private Vector3 _offset = Vector3.zero;
    [Header("선형 보간 속도"), SerializeField]
    private float _smoothingSpeed = 10f;

#if UNITY_EDITOR
    [Header("위치 표시 색깔"), SerializeField]
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