using FIMSpace.FProceduralAnimation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Test : MonoBehaviour
{
    public Animator animator;
    private Rigidbody[] rigidbodies;
    private Dictionary<Transform, Coroutine> activeForces = new Dictionary<Transform, Coroutine>();

    void Start()
    {
        //animator = GetComponent<Animator>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();

        // Rigidbody 설정 조정 (부드러운 물리 반응을 위해)
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.isKinematic = false;  // 
        }

        // Animator 업데이트 모드를 물리 기반으로 변경
        animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
    }

    // 특정 부위에 힘 적용 (물리 반응)
    void ApplyImpact(Transform hitPart, Vector3 force)
    {
        Rigidbody rb = hitPart.GetComponent<Rigidbody>();
        if (rb == null) return;

        if (activeForces.ContainsKey(hitPart))
        {
            StopCoroutine(activeForces[hitPart]);
        }

        Coroutine impactRoutine = StartCoroutine(ImpactForceRoutine(rb, force));
        activeForces[hitPart] = impactRoutine;
    }

    // 점진적으로 힘을 줄여 자연스럽게 반응
    IEnumerator ImpactForceRoutine(Rigidbody rb, Vector3 force)
    {
        float duration = 0.5f;
        float time = 0;
        while (time < duration)
        {
            rb.AddForce(force * (1 - time / duration), ForceMode.Impulse);
            time += Time.deltaTime;
            yield return null;
        }
    }

    // 충돌 감지 후 힘을 적용
    void OnCollisionEnter(Collision collision)
    {
        Transform hitPart = collision.transform;
        Vector3 impactForce = collision.impulse * 0.5f;
        ApplyImpact(hitPart, impactForce);
    }

    // 충돌 부위의 애니메이션 영향 감소
    void OnAnimatorIK(int layerIndex)
    {
        foreach (var part in activeForces.Keys)
        {
            int boneIndex = GetBoneIndex(part);
            if (boneIndex != -1)
            {
                animator.SetIKPositionWeight((AvatarIKGoal)boneIndex, 0.3f); // 충돌 부위만 보정 감소
            }
        }
    }

    // Transform을 AvatarIKGoal로 변환하는 예제
    int GetBoneIndex(Transform bone)
    {
        if (bone.name.Contains("Hand")) return (int)AvatarIKGoal.RightHand;
        if (bone.name.Contains("Foot")) return (int)AvatarIKGoal.RightFoot;
        return -1;
    }
}