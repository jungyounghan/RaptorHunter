using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class RaptorBehavior : MonoBehaviour
{
    private bool _hasTransform = false;

    private Transform _transform = null;

    private Transform getTransform {
        get
        {
            if(_hasTransform == false)
            {
                _transform = transform;
                _hasTransform = true;
            }
            return _transform;
        }
    }

    private bool _hasAnimator = false;

    private Animator _animator = null;

    private Animator getAnimator {
        get
        {
            if(_hasAnimator == false)
            {
                _animator = GetComponent<Animator>();
                _hasAnimator = true;
            }
            return _animator;
        }
    }

    [SerializeField, Range(0, 7)]
    private float _moveSpeed = 0f;


    private void Update()
    {
        Move(_moveSpeed * Time.deltaTime);
    }

    public void Move(float value)
    {
        getAnimator.SetFloat("Speed", value);
        getTransform.Translate(getTransform.forward * -value);
        getAnimator.speed = value > 0 ? value * 50 : 1;
    }
}