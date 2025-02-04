using UnityEngine;

/// <summary>
/// Ű���峪 ���콺�� �̿��Ͽ� ���ϴ� �������� ��ü�� �̵������ִ� Ŭ����
/// </summary>
[DisallowMultipleComponent]
public class Transferer : MonoBehaviour
{
	private bool _hasTransform = false;

	private Transform _transform = null;

	private Transform getTransform {
		get
        {
			if(_hasTransform == false)
            {
				_hasTransform = true;
				_transform = transform;
            }
			return _transform;
        }
    }

	[Header("���콺�� ���� ���� �̵��� �� ī�޶� ȸ�� �ӵ�"), SerializeField, Range(0, byte.MaxValue)]
	private float _turnSpeed = 4.0f;
	[Header("����� �Ĺ��� ���� ���� �� ī�޶� �ӵ�"), SerializeField, Range(0, byte.MaxValue)]
	private float _moveSpeed = 4.0f;
	[Header("����Ʈ�� ������ �� ���ӵ� ����"), SerializeField, Range(0, byte.MaxValue)]
	private float _accelerationRate = 5;

	//Yaw: ��ü�� Z�� ���� ȸ�� �ݰ�
	private float _yaw = 0f;
	//Pitch: ��ü�� Y�� ���� ȸ�� �ݰ�
	private float _pitch = 0f;

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		Vector3 move = Vector3.zero;
		if (Input.GetKey(KeyCode.W))
		{
			move += _moveSpeed * deltaTime * Vector3.forward;
		}
		if (Input.GetKey(KeyCode.S))
		{
			move += _moveSpeed * deltaTime * Vector3.back;
		}
		if (Input.GetKey(KeyCode.A))
		{
			move += _moveSpeed * deltaTime * Vector3.left;
		}
		if (Input.GetKey(KeyCode.D))
		{
			move += _moveSpeed * deltaTime * Vector3.right;
		}
		if (Input.GetKey(KeyCode.Q))
		{
			move += _moveSpeed * deltaTime * Vector3.down;
		}
		if (Input.GetKey(KeyCode.E))
		{
			move += _moveSpeed * deltaTime * Vector3.up;
		}
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
		{
			move *= _accelerationRate;
		}
		if (Mathf.Abs(move.sqrMagnitude) > Mathf.Epsilon)
		{
			getTransform.Translate(move, Space.Self);
		}
		if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
		{
			_yaw += Input.GetAxis("Mouse X");
			_pitch -= Input.GetAxis("Mouse Y");
			getTransform.eulerAngles = new Vector3(_turnSpeed * _pitch, _turnSpeed * _yaw, 0.0f);
		}
	}
}