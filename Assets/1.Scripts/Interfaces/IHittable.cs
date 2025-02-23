using UnityEngine;

/// <summary>
/// 타격을 받는 객체가 상속 받는 인터페이스
/// </summary>
public interface IHittable
{
    public Transform transform {
        get;
    }

    /// <summary>
    /// 타격을 받는 함수(매개변수:위치, 방향, 세기)
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="force"></param>
    public void Hit(Vector3 origin, Vector3 direction, uint force);
}