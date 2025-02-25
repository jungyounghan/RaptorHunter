using UnityEngine;

/// <summary>
/// �⺻ �ɷ�ġ�� ������ ��ũ���ͺ� �߻� Ŭ����
/// </summary>
[CreateAssetMenu(menuName = nameof(Stat))]
public sealed class Stat : ScriptableObject
{
    //�ּ� ���¹̳� �ѵ�
    public const float MinFullStamina = float.Epsilon;
    //�ִ� ���¹̳� �ѵ�
    public const float MaxFullStamina = byte.MaxValue;

    //�ּ� ���¹̳� ȸ�� �ӵ�
    public const float MinRecoverStamina = float.Epsilon;
    //�ִ� ���¹̳� ȸ�� �ӵ�
    public const float MaxRecoverStamina = byte.MaxValue;

    //�ּ� ���� �ӵ�
    public const float MinWalkSpeed = 0;
    //�ִ� ���� �ӵ�
    public const float MaxWalkSpeed = byte.MaxValue;

    //�ּ� ���� �ӵ�
    public const float MinDashSpeed = float.Epsilon;
    //�ִ� ���� �ӵ�
    public const float MaxDashSpeed = byte.MaxValue;

    //�ּ� ���� �Ҹ� ���
    public const float MinDashCost = 0;
    //�ִ� ���� �Ҹ� ���
    public const float MaxDashCost = 1;

    //�ּ� ���� �ӵ� ����
    public const float MinReverseRate = 0;
    //�ִ� ���� �ӵ� ����
    public const float MaxReverseRate = 1;

    //�ּ� ���ߴ� �Ÿ�
    public const float MinStoppingDistance = 0;
    //�ִ� ���ߴ� �Ÿ�
    public const float MaxStoppingDistance = 20;

    //�ּ� ���� �ӵ�
    public const float MinAttackSpeed = 0.1f;
    //�ִ� ���� �ӵ�
    public const float MaxAttackSpeed = 10f;

    [Header("���¹̳� �ѵ�"), Range(MinFullStamina, MaxFullStamina)]
    public float fullStamina = 100;

    [Header("���¹̳� ȸ�� �ӵ�"), Range(MinRecoverStamina, MaxRecoverStamina)]
    public float recoverStamina = 10;

    [Header("���� �ӵ�"), Range(MinWalkSpeed, MaxWalkSpeed)]
    public float walkSpeed = 3.5f;

    [Header("���� �ӵ�"), Range(MinDashSpeed, MaxDashSpeed)]
    public float dashSpeed = 1;

    [Header("���� �Ҹ� ���"), Range(MinDashCost, MaxDashCost)]
    public float dashCost = 0.001f;

    [Header("���� �ӵ� ����"), Range(MinReverseRate, MaxReverseRate)]
    public float reverseRate = 0.5f;

    [Header("���ߴ� �Ÿ�"), Range(MinStoppingDistance, MaxStoppingDistance)]
    public float stoppingDistance = 1;

    [Header("���� �ӵ�"), Range(MinAttackSpeed, MaxAttackSpeed)]
    public float attackSpeed = 1;

    [Header("���ݷ�")]
    public uint attackDamage = 1;

    [Header("�ִ� ü��")]
    public uint fullLife = 10;
}