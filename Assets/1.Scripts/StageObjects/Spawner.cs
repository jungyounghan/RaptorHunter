using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 캐릭터들을 소환해주는 클래스
/// </summary>
public sealed class Spawner: MonoBehaviour
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

    [Header("랜덤 회전"), SerializeField]
    private Vector3 _randomAngle;

#if UNITY_EDITOR
    [Header("기즈모 색상"), SerializeField]
    private Color _gizmoColor = Color.green;


    private void OnDrawGizmos()
    {
        Gizmos.color = _gizmoColor;
        Vector3 pointSize = getTransform.lossyScale * 0.5f;
        pointSize.x = Mathf.Abs(pointSize.x);
        pointSize.y = Mathf.Abs(pointSize.y);
        pointSize.z = Mathf.Abs(pointSize.z);
        Vector3 point1 = getTransform.TransformPoint(-pointSize);
        Vector3 point2 = getTransform.TransformPoint(new Vector3(pointSize.x, -pointSize.y, -pointSize.z));
        Vector3 point3 = getTransform.TransformPoint(new Vector3(pointSize.x, -pointSize.y, pointSize.z));
        Vector3 point4 = getTransform.TransformPoint(new Vector3(-pointSize.x, -pointSize.y, pointSize.z));
        Vector3 point5 = getTransform.TransformPoint(new Vector3(-pointSize.x, pointSize.y, -pointSize.z));
        Vector3 point6 = getTransform.TransformPoint(new Vector3(pointSize.x, pointSize.y, -pointSize.z));
        Vector3 point7 = getTransform.TransformPoint(pointSize);
        Vector3 point8 = getTransform.TransformPoint(new Vector3(-pointSize.x, pointSize.y, pointSize.z));
        Gizmos.DrawLine(point1, point2);
        Gizmos.DrawLine(point2, point3);
        Gizmos.DrawLine(point3, point4);
        Gizmos.DrawLine(point4, point1);
        Gizmos.DrawLine(point5, point6);
        Gizmos.DrawLine(point6, point7);
        Gizmos.DrawLine(point7, point8);
        Gizmos.DrawLine(point8, point5);
        Gizmos.DrawLine(point1, point5);
        Gizmos.DrawLine(point2, point6);
        Gizmos.DrawLine(point3, point7);
        Gizmos.DrawLine(point4, point8);
    }

#endif

    public Character Get(Character character)
    {
        if(character != null)
        {
            Vector3 pointSize = getTransform.lossyScale * 0.5f;
            pointSize.x = Mathf.Abs(pointSize.x);
            pointSize.y = Mathf.Abs(pointSize.y);
            pointSize.z = Mathf.Abs(pointSize.z);
            Vector3 random = new Vector3(Random.Range(-pointSize.x, pointSize.x), Random.Range(-pointSize.y, pointSize.y), Random.Range(-pointSize.z, pointSize.z));
            // 로컬 -> 월드 변환 (회전 및 위치 적용)
            Vector3 position = getTransform.TransformPoint(random);
            Vector3 angleSize = _randomAngle;
            angleSize.x = Mathf.Abs(angleSize.x);
            angleSize.y = Mathf.Abs(angleSize.y);
            angleSize.z = Mathf.Abs(angleSize.z);
            random = new Vector3(Random.Range(-angleSize.x, angleSize.x), Random.Range(-angleSize.y, angleSize.y), Random.Range(-angleSize.z, angleSize.z));
            Quaternion rotation = Quaternion.Euler(getTransform.rotation.eulerAngles + random);
            if (character.gameObject.scene == SceneManager.GetActiveScene())
            {
                character.transform.position = position;
                character.transform.rotation = rotation;
                character.gameObject.SetActive(true);
            }
            else
            {
                character = Instantiate(character, position, rotation);
            }
        }
        return character;
    }


}