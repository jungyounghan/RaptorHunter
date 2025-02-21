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
    private HunterCharacter _hunterCharacter;
    [SerializeField]
    private RaptorCharacter _raptorCharacter;

    [Header("프로퍼티")]
    [SerializeField]
    private CinemachineVirtualCamera _cinemachineVirtualCamera;
    [SerializeField]
    private State _state;
    [SerializeField]
    private Spawner _allySpawner;
    [SerializeField]
    private List<GameObject> _props = new List<GameObject>();

    [SerializeField]
    private ManualController _allyController;
    [SerializeField]
    private List<AutomaticController> _enemyController = new List<AutomaticController>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        SetProps(false);
    }
#endif

    private void Awake()
    {
        SpawnAlly(Character.Raptor);
        SetProps(true);
    }

    private void SpawnAlly(bool human)
    {
        Character character = human == true ? _allySpawner?.Get(_hunterCharacter): _allySpawner?.Get(_raptorCharacter);
        if(character != null)
        {
            _allyController = character.gameObject.AddComponent<ManualController>();
            if(_allyController != null)
            {
                _allyController.Initialize(SetStamina, SetLife);
                _allyController.Revive();
            }
            if(_cinemachineVirtualCamera != null)
            {
                Transform transform = character.transform;
                _cinemachineVirtualCamera.Follow = transform;
                _cinemachineVirtualCamera.LookAt = transform;
            }
        }
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
    private void SetStamina(float current, float max)
    {
        _state.SetStamina(current, max);
    }

    private void SetLife(uint current, uint max, Controller controller)
    {
        if (controller == _allyController)
        {
            _state.SetLife(current, max);
        }
        else
        {

        }
    }
}