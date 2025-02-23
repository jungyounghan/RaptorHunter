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
    private Spawner _enemySpawner;
    [SerializeField]
    private List<GameObject> _props = new List<GameObject>();

    private uint _killCount = 0;
    private ManualController _allyController;
    private List<AutomaticController> _enemyControllers = new List<AutomaticController>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        SetProps(false);
    }
#endif

    private void Awake()
    {
        SpawnAlly(Character.Raptor);
        SpawnEnemy(Character.Raptor);
        SetProps(true);
    }

    private void SpawnAlly(bool human)
    {
        if (_allySpawner != null)
        {
            Character character = human == true ? _allySpawner.Get(_hunterCharacter) : _allySpawner.Get(_raptorCharacter);
            if (character != null)
            {
                bool success = character.gameObject.TryGetComponent(out _allyController);
                if (success == false)
                {
                    _allyController = character.gameObject.AddComponent<ManualController>();
                }
                _allyController.Initialize(SetStamina, SetLife);
                //_allyController.Set();
                _allyController.Revive();
                if (_cinemachineVirtualCamera != null)
                {
                    Transform transform = character.transform;
                    _cinemachineVirtualCamera.Follow = transform;
                    _cinemachineVirtualCamera.LookAt = transform;
                }
            }
        }
    }

    private void SpawnEnemy(bool human)
    {
        if(_enemySpawner != null)
        {
            foreach(AutomaticController enemyController in _enemyControllers)
            {
                if(enemyController.gameObject.activeSelf == false && enemyController.IsHuman() == human)
                {
                    _enemySpawner.Get(enemyController.character);
                    enemyController.Revive();
                    return;
                }
            }
            Character character = human == true ? _enemySpawner.Get(_hunterCharacter) : _enemySpawner.Get(_raptorCharacter);
            if (character != null)
            {
                AutomaticController automaticController = character.gameObject.GetComponent<AutomaticController>();
                if(automaticController == null)
                {
                    automaticController = character.gameObject.AddComponent<AutomaticController>();
                }
                automaticController.Initialize(SetLife);
                //automaticController.Set();
                automaticController.Revive();
                _enemyControllers.Add(automaticController);
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
        _state?.SetStamina(current, max);
    }

    private void SetLife(uint current, uint max, Controller controller)
    {
        if (controller == _allyController)
        {
            _state?.SetLife(current, max);
        }
        else
        {
            if(controller.alive == false)
            {
                _state?.SetKill(++_killCount);
            }
        }
    }
}