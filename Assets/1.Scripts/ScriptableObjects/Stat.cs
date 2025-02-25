using UnityEngine;

/// <summary>
/// 기본 능력치를 구성할 스크립터블 추상 클래스
/// </summary>
[CreateAssetMenu(menuName = nameof(Stat))]
public sealed class Stat : ScriptableObject
{
    //최소 스태미나 한도
    public const float MinFullStamina = float.Epsilon;
    //최대 스태미나 한도
    public const float MaxFullStamina = byte.MaxValue;

    //최소 스태미나 회복 속도
    public const float MinRecoverStamina = float.Epsilon;
    //최대 스태미나 회복 속도
    public const float MaxRecoverStamina = byte.MaxValue;

    //최소 걸음 속도
    public const float MinWalkSpeed = 0;
    //최대 걸음 속도
    public const float MaxWalkSpeed = byte.MaxValue;

    //최소 돌진 속도
    public const float MinDashSpeed = float.Epsilon;
    //최대 돌진 속도
    public const float MaxDashSpeed = byte.MaxValue;

    //최소 돌진 소모 비용
    public const float MinDashCost = 0;
    //최대 돌진 소모 비용
    public const float MaxDashCost = 1;

    //최소 후진 속도 비율
    public const float MinReverseRate = 0;
    //최대 후진 속도 비율
    public const float MaxReverseRate = 1;

    //최소 멈추는 거리
    public const float MinStoppingDistance = 0;
    //최대 멈추는 거리
    public const float MaxStoppingDistance = 20;

    //최소 공격 속도
    public const float MinAttackSpeed = 0.1f;
    //최대 공격 속도
    public const float MaxAttackSpeed = 10f;

    [Header("스태미나 한도"), Range(MinFullStamina, MaxFullStamina)]
    public float fullStamina = 100;

    [Header("스태미나 회복 속도"), Range(MinRecoverStamina, MaxRecoverStamina)]
    public float recoverStamina = 10;

    [Header("걸음 속도"), Range(MinWalkSpeed, MaxWalkSpeed)]
    public float walkSpeed = 3.5f;

    [Header("돌진 속도"), Range(MinDashSpeed, MaxDashSpeed)]
    public float dashSpeed = 1;

    [Header("돌진 소모 비용"), Range(MinDashCost, MaxDashCost)]
    public float dashCost = 0.001f;

    [Header("후진 속도 비율"), Range(MinReverseRate, MaxReverseRate)]
    public float reverseRate = 0.5f;

    [Header("멈추는 거리"), Range(MinStoppingDistance, MaxStoppingDistance)]
    public float stoppingDistance = 1;

    [Header("공격 속도"), Range(MinAttackSpeed, MaxAttackSpeed)]
    public float attackSpeed = 1;

    [Header("공격력")]
    public uint attackDamage = 1;

    [Header("최대 체력")]
    public uint fullLife = 10;
}