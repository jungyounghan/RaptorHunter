using UnityEngine;

/// <summary>
/// �� Ŭ����
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public sealed class Gun : MonoBehaviour
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

    private bool _hasAudioSource = false;

    private AudioSource _audioSource = null;

    private AudioSource getAudioSource
    {
        get
        {
            if (_hasAudioSource == false)
            {
                _hasAudioSource = TryGetComponent(out _audioSource);
            }
            return _audioSource;
        }
    }

    [Header("������ Ʈ������"), SerializeField]
    private Transform _gunMagazine = null;  //źâ

    [Header("������")]
    [SerializeField]
    private Transform _laserPoint = null;   //������ ������ ��
    [SerializeField]
    private LineRenderer _laserLine = null; //������ �� ������

    [Header("�ѱ�")]
    [SerializeField]
    private Transform _muzzlePoint = null;      //�ѱ� ��ġ
    [SerializeField]
    private LineRenderer _muzzleLine = null;    //�ѱ� �߻�ü ���� ������
    [SerializeField]
    private ParticleSystem _muzzleFlashEffect;   //�ѱ� ȭ�� ȿ��

    [Header("���̾� ����ũ"), SerializeField]
    private LayerMask _layerMask;

    [Header("���� �ӵ�(�ʴ� n��)"), SerializeField, Range(Stat.MinAttackSpeed, Stat.MaxAttackSpeed)]
    private float _shotSpeed = 10;
    private float _shotCoolTime = 0;

    private float _bulletCoolTime = 0f;

    private static readonly float FireDistance = 50f;
    private static readonly float BulletDuration = 0.1f;

    public class Handle
    {
        public Vector3 position;
        public Quaternion rotation;

        public Handle(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }

    private void ShowShotEffect(Vector3 position)
    {
        if (_muzzleLine != null)
        {
            _muzzleLine.positionCount = 2;
            _muzzleLine.SetPosition(0, _muzzleLine.transform.position);
            _muzzleLine.SetPosition(1, position);
            _muzzleLine.enabled = true;
        }
    }

    public void LookAt(Vector3 position)
    {
        getTransform.LookAt(position);
    }

    public void Set(float attackSpeed)
    {
        _shotSpeed = Mathf.Clamp(attackSpeed, Stat.MinAttackSpeed, Stat.MaxAttackSpeed);
    }

    public void Recharge()
    {
        float deltaTime = Time.deltaTime;
        if(_shotCoolTime > 0)
        {
            _shotCoolTime -= deltaTime;
            if(_shotCoolTime < 0)
            {
                _shotCoolTime = 0;
            }
        }
        if(_bulletCoolTime > 0)
        {
            _bulletCoolTime -= deltaTime;
            if (_bulletCoolTime < 0)
            {
                _bulletCoolTime = 0;
                if(_muzzleLine != null)
                {
                    _muzzleLine.enabled = false;
                }
            }
            else if(_muzzleLine != null)
            {
                _muzzleLine.SetPosition(0, _muzzleLine.transform.position);
            }
        }
        if (_laserPoint != null && _laserLine != null && Physics.Raycast(_laserPoint.position - _laserPoint.forward, _laserPoint.forward, out RaycastHit hit, Mathf.Infinity, _layerMask))
        {
            if (_laserLine.positionCount != 2)
            {
                _laserLine.positionCount = 2;
            }
            _laserLine.SetPosition(0, _laserPoint.position);
            _laserLine.SetPosition(1, hit.point);
        }
    }

    public void HideLaser()
    {
        if (_laserLine != null)
        {
            _laserLine.positionCount = 0;
        }
    }

    public void Shot(uint damage)
    {
        if (_muzzlePoint != null && _shotCoolTime == 0)
        {
            if (_muzzleFlashEffect != null)
            {
                _muzzleFlashEffect.Play();
            }
            // ź�� ���� ȿ�� ���
            //shellEjectEffect.Play();
            if (getAudioSource != null)
            {
                getAudioSource.Play();
            }
            Vector3 position = _muzzlePoint.position;
            Vector3 forward = _muzzlePoint.forward;
            if (Physics.Raycast(position - forward, forward, out RaycastHit hit, Mathf.Infinity, _layerMask))
            {
                ShowShotEffect(hit.point);
                Transform transform = hit.collider.transform;
                while(transform != null)
                {
                    IHittable hittable = transform.GetComponent<IHittable>();
                    if (hittable != null)
                    {
                        transform = getTransform;
                        while (transform != null)
                        {
                            transform = transform.parent;
                            if (hittable.transform == transform)
                            {
                                return;
                            }
                        }
                        hittable.Hit(hit.point, forward, damage);
                        break;
                    }
                    else
                    {
                        transform = transform.parent;
                    }
                }
            }
            else
            {
                ShowShotEffect(position + forward * FireDistance);
            }
            _shotCoolTime = 1 / _shotSpeed;
            _bulletCoolTime = BulletDuration;
        }
    }

    public Handle GetMagazineHandle()
    {
        if(_gunMagazine != null)
        {
            return new Handle(_gunMagazine.position, _gunMagazine.rotation);
        }
        return null;
    }
}