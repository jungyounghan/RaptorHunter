using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider))]
public sealed class Foot : MonoBehaviour
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

    private bool _hasBoxCollider = false;

    private BoxCollider _boxCollider = null;

    private BoxCollider getBoxCollider
    {
        get
        {
            if(_hasBoxCollider == false)
            {
                _hasBoxCollider = TryGetComponent(out _boxCollider);
            }
            return _boxCollider;
        }
    }

    private List<Collider> _colliders = new List<Collider>();

#if UNITY_EDITOR
    [SerializeField]
    private float _pivotSize = 0.1f;

    [SerializeField]
    private Color _pivotColor = Color.green;

    [SerializeField]
    private Color _contactColor = Color.red;

    private void OnDrawGizmos()
    {
        DrawPoint(getTransform.position, _pivotColor);
    }

    private void DrawPoint(Vector3 point, Color color)
    {
        Debug.DrawLine(point, point + getTransform.up * _pivotSize, color);
        Debug.DrawLine(point, point - getTransform.up * _pivotSize, color);
        Debug.DrawLine(point, point + getTransform.right * _pivotSize, color);
        Debug.DrawLine(point, point - getTransform.right * _pivotSize, color);
        Debug.DrawLine(point, point + getTransform.forward * _pivotSize, color);
        Debug.DrawLine(point, point - getTransform.forward * _pivotSize, color);
    }
#endif

    private void OnCollisionEnter(Collision collision)
    {
        if (_colliders.Contains(collision.collider) == false)
        {
            _colliders.Add(collision.collider);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        _colliders.Remove(collision.collider);
    }

    private void LateUpdate()
    {
        List<Vector3> points = new List<Vector3>();
        foreach(Collider collider in _colliders)
        {
            if (collider is MeshCollider meshCollider)
            {
                List<Vector3> contactPoints = new List<Vector3>();
                if (meshCollider.convex == true)
                {
                    // Convex가 true인 경우 ComputePenetration 활용
                    if (Physics.ComputePenetration(
                            getBoxCollider, getTransform.position, getTransform.rotation,
                            meshCollider, meshCollider.transform.position, meshCollider.transform.rotation,
                            out Vector3 direction, out float distance))
                    {
                        Vector3 contactPoint = meshCollider.ClosestPoint(getTransform.position - direction * distance);
                        contactPoints.Add(contactPoint);
                    }
                }
                else
                {
                    // Convex가 false인 경우 Raycast로 충돌 검사
                    Vector3 origin = getTransform.position;
                    Vector3 closestPoint = meshCollider.ClosestPoint(origin);

                    if (closestPoint != origin) // 내부에 완전히 포함되지 않았을 경우만 추가
                    {
                        contactPoints.Add(closestPoint);
                    }
                    else
                    {
                        // 내부에 포함된 경우 -> Raycast로 충돌 지점 찾기
                        RaycastHit hit;
                        Vector3 rayDirection = (meshCollider.bounds.center - origin).normalized;
                        if (meshCollider.Raycast(new Ray(origin, rayDirection), out hit, meshCollider.bounds.extents.magnitude * 2))
                        {
                            contactPoints.Add(hit.point);
                        }
                    }
                }
                foreach (Vector3 point in contactPoints)
                {
                    DrawPoint(point, _contactColor);
                }
            }
            else
            {
                Physics.ComputePenetration(getBoxCollider, getTransform.position, getTransform.rotation, collider, collider.transform.position, collider.transform.rotation, out Vector3 direction, out float distance);
                points.Add(collider.ClosestPoint(getTransform.position - direction * distance));
            }
        }
        foreach(Vector3 point in points)
        {
            DrawPoint(point, _contactColor);
        }
    }
}