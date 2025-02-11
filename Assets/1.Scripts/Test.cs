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

        // Rigidbody ���� ���� (�ε巯�� ���� ������ ����)
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.isKinematic = false;  // 
        }

        // Animator ������Ʈ ��带 ���� ������� ����
        animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
    }

    // Ư�� ������ �� ���� (���� ����)
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

    // ���������� ���� �ٿ� �ڿ������� ����
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

    // �浹 ���� �� ���� ����
    void OnCollisionEnter(Collision collision)
    {
        Transform hitPart = collision.transform;
        Vector3 impactForce = collision.impulse * 0.5f;
        ApplyImpact(hitPart, impactForce);
    }

    // �浹 ������ �ִϸ��̼� ���� ����
    void OnAnimatorIK(int layerIndex)
    {
        foreach (var part in activeForces.Keys)
        {
            int boneIndex = GetBoneIndex(part);
            if (boneIndex != -1)
            {
                animator.SetIKPositionWeight((AvatarIKGoal)boneIndex, 0.3f); // �浹 ������ ���� ����
            }
        }
    }

    // Transform�� AvatarIKGoal�� ��ȯ�ϴ� ����
    int GetBoneIndex(Transform bone)
    {
        if (bone.name.Contains("Hand")) return (int)AvatarIKGoal.RightHand;
        if (bone.name.Contains("Foot")) return (int)AvatarIKGoal.RightFoot;
        return -1;
    }
}