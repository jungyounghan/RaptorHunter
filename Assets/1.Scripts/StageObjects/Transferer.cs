using UnityEngine;

/// <summary>
/// 키보드나 마우스를 이용하여 원하는 방향으로 객체를 이동시켜주는 클래스
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

	[Header("마우스가 축을 따라 이동할 때 카메라 회전 속도"), SerializeField, Range(0, byte.MaxValue)]
	private float _turnSpeed = 4.0f;
	[Header("전방과 후방을 가고 있을 때 카메라 속도"), SerializeField, Range(0, byte.MaxValue)]
	private float _moveSpeed = 4.0f;
	[Header("시프트를 눌렀을 때 가속도 비율"), SerializeField, Range(0, byte.MaxValue)]
	private float _accelerationRate = 5;

	//Yaw: 물체의 Z축 기준 회전 반경
	private float _yaw = 0f;
	//Pitch: 물체의 Y축 기준 회전 반경
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