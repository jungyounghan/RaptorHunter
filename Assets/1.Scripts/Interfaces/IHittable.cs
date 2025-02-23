using UnityEngine;

/// <summary>
/// Ÿ���� �޴� ��ü�� ��� �޴� �������̽�
/// </summary>
public interface IHittable
{
    public Transform transform {
        get;
    }

    /// <summary>
    /// Ÿ���� �޴� �Լ�(�Ű�����:��ġ, ����, ����)
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="force"></param>
    public void Hit(Vector3 origin, Vector3 direction, uint force);
}