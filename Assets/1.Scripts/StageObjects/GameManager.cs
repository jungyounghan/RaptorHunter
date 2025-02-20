using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/// <summary>
/// 게임 진행을 총괄하는 클래스
/// </summary>
public sealed class GameManager : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField]
    private InputController _hunterInputController;
    [SerializeField]
    private InputController _raptorInputController;
    [SerializeField]
    private AutoController _hunterAutoController;
    [SerializeField]
    private AutoController _raptorAutoController;

    [Header("프로퍼티")]
    [SerializeField]
    private CinemachineVirtualCamera _cinemachineVirtualCamera;
    [SerializeField]
    private State _state;
    [SerializeField]
    private List<GameObject> _props = new List<GameObject>();
    [SerializeField]
    private Spawner<InputController> _allySpawner = new Spawner<InputController>();
    [SerializeField]
    private Spawner<AutoController> _enemySpawner = new Spawner<AutoController>();

    [SerializeField]
    private InputController _allyController;
    [SerializeField]
    private List<AutoController> _enemyController = new List<AutoController>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        SetProps(false);
    }
#endif

    private void Awake()
    {
        _allyController = _allySpawner.Get();
        if (_allyController != null)
        {
            _allyController.Initialize(SetStamina, SetLife);
            _allyController.Revive();
            if(_cinemachineVirtualCamera != null)
            {
                _cinemachineVirtualCamera.Follow = _allyController.getTransform;
                _cinemachineVirtualCamera.LookAt = _allyController.getTransform;
            }
        }
        SetProps(true);
    }

    private void Update()
    {
        _enemySpawner.Update();
    }

    private void SetProps(bool active)
    {
        for (int i = 0; i < _props.Count; i++)
        {
            if (_props[i] != null)
            {
                _props[i].SetActive(active);
            }
        }
    }

    public void SetLife(uint current, uint max, Controller controller)
    {
        if (controller == _allyController)
        {
            _state.SetLife(current, max);
        }
        else
        {

        }
    }

    public void SetStamina(float current, float max)
    {
        _state.SetStamina(current, max);
    }
}