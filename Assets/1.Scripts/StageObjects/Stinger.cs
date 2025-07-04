using System;
using UnityEngine;

/// <summary>
/// �̻�, ���� Ŭ����
/// </summary>
[RequireComponent(typeof(Collider))]
public sealed class Stinger : MonoBehaviour
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

    private Action _action;

    public uint damage {
        set;
        private get;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(damage > 0)
        {
            Transform transform = collision.collider.transform;
            while (transform != null)
            {
                IHittable hittable = transform.GetComponent<IHittable>();
                if (hittable != null)
                {
                    transform = getTransform;
                    while(transform != null)
                    {
                        transform = transform.parent;
                        if(hittable.transform == transform)
                        {
                            return;
                        }
                    }
                    hittable.Hit(getTransform.position, getTransform.forward, damage);
                    _action?.Invoke();
                    break;
                }
                else
                {
                    transform = transform.parent;
                }
            }
        }
    }

    public void Initialize(Action action)
    {
        _action = action;
    }
}