using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
public class Obstacle : MonoBehaviour, IHittable
{
    private bool _hasRigidbody = false;

    private Rigidbody _rigidbody = null;

    protected Rigidbody getRigidbody {
        get
        {
            if (_hasRigidbody == false)
            {
                _hasRigidbody = TryGetComponent(out _rigidbody);
            }
            return _rigidbody;
        }
    }

    private bool _hasAudioSource = false;

    private AudioSource _audioSource = null;

    private AudioSource getAudioSource {
        get
        {
            if (_hasAudioSource == false)
            {
                _hasAudioSource = TryGetComponent(out _audioSource);
            }
            return _audioSource;
        }
    }

    [SerializeField]
    private List<AudioClip> _audioClips = new List<AudioClip>();

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(getRigidbody.velocity.magnitude);
        //PlaySound();
    }

    private void PlaySound()
    {
        int count = _audioClips.Count;
        if (count > 0)
        {
            int index = Random.Range(0, count);
            getAudioSource.clip = _audioClips[index];
            getAudioSource.Play();
        }
    }

    public void Hit(Vector3 origin, Vector3 direction, uint force)
    {
        //공격력 100정도 되야 소리가 찰짐
        getRigidbody.AddForce(direction.normalized * force, ForceMode.Impulse);
        PlaySound();
    }
}