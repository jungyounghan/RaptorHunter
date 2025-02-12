using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private static readonly string CenterX = "_CenterX";
    private static readonly string CenterY = "_CenterY";
    private static readonly string Degree = "_Degree";
    private static readonly string Angle = "_Angle";

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

    private List<Material> _list = new List<Material>();

    [Header("바라볼 대상의 트랜스폼"), SerializeField]
    private Transform _target;

    [SerializeField]
    private LayerMask _layerMask;

#if UNITY_EDITOR

    private Color _targetColor = Color.black;

    private Color _leftColor = Color.red;

    private Color _rightColor = Color.blue;

    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false && _target != null)
        {
            Vector3 center = getTransform.position;
            Vector3 target = _target.position;
            center.y = target.y;
            Vector3 scope = getTransform.right * (center - target).magnitude;
            Vector3 direction = (center - target);
            Debug.DrawRay(center, scope, _targetColor);
            Debug.DrawRay(center, -scope, _targetColor);
            Debug.DrawRay(target, direction - scope, _rightColor);
            Debug.DrawRay(target, direction + scope, _leftColor);
        }
    }
#endif

    private void Update()
    {
        //굳이 _target이 없어도 될거 같다.
        if (_target != null)
        {
            Vector3 center = getTransform.position;
            Vector3 target = _target.position;
            center.y = target.y;
            float magnitude = (center - target).magnitude;
            Vector3 scope = getTransform.right * magnitude;
            Vector3 direction = (center - target);
            Vector3 left = target + (direction + scope);
            Vector3 right = target + (direction - scope);
            float distance = Mathf.Sqrt((magnitude * magnitude) * 2);
            if (Physics.Raycast(target, direction - scope, out RaycastHit rightHit, distance, _layerMask))
            {
                right = rightHit.point;
            }
#if UNITY_EDITOR
            Debug.DrawLine(center, right, _targetColor);
            Debug.DrawLine(target, right, _rightColor);
#endif
            if (Physics.Raycast(target, direction + scope, out RaycastHit leftHit, distance, _layerMask))
            {
                left = leftHit.point;
            }
#if UNITY_EDITOR
            Debug.DrawLine(center, left, _targetColor);
            Debug.DrawLine(target, left, _leftColor);
#endif
            Vector2 o = new Vector2(center.x, center.z);
            Vector2 ao = new Vector2(left.x, left.z) - o;
            Vector2 bo = new Vector2(right.x, right.z) - o;
            Vector3 forward = ((left - center).normalized + (right - center).normalized) * 0.5f;
            float denom = ao.magnitude * bo.magnitude;
            float angle = (denom > 0f) ? Mathf.Acos(Mathf.Clamp(Vector2.Dot(ao, bo) / denom, -1f, 1f)) * Mathf.Rad2Deg : 0f;
            Debug.Log(angle);
            float degree = Mathf.Atan2(forward.z, forward.x) * Mathf.Rad2Deg;
            foreach (Material material in _list)
            {
                material.SetFloat(CenterX, center.x);
                material.SetFloat(CenterY, center.z);
                material.SetFloat(Degree, degree);
                material.SetFloat(Angle, angle);
            }
        }
        else
        {
            foreach (Material material in _list)
            {
                material.SetFloat(Angle, 360);
            }
        }
    }

    public void Show(IEnumerable<Material> materials)
    {
        if(materials != null)
        {
            foreach(Material material in materials)
            {
                if(_list.Contains(material) == false)
                {
                    _list.Add(material);
                }
            }
        }
    }

    public void Hide(IEnumerable<Material> materials)
    {
        if (materials != null)
        {
            foreach (Material material in materials)
            {
                if (_list.Contains(material) == true)
                {
                    material.SetFloat(Angle, 360);
                    _list.Remove(material);
                }
            }
        }
    }
}