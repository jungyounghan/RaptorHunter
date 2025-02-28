using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// 게임 진행을 총괄하는 클래스
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(StatBundle))]
public sealed class GameManager : MonoBehaviour
{
    private bool _hasStatBundle = false;

    private StatBundle _statBundle = null;

    private StatBundle getStatBundle
    {
        get
        {
            if(_hasStatBundle == false)
            {
                _hasStatBundle = TryGetComponent(out _statBundle);
            }
            return _statBundle;
        }
    }

    [Header("프리팹")]
    [SerializeField]
    private HunterCharacter _hunterCharacter;
    [SerializeField]
    private RaptorCharacter _raptorCharacter;
    [SerializeField]
    private List<Item> _itemPrefabs = new List<Item>();

    [Header("프로퍼티")]
    [SerializeField]
    private BoxRevealer _boxRevealer = null;
    [SerializeField]
    private CinemachineVirtualCamera _cinemachineVirtualCamera;
    [SerializeField]
    private State _state;
    [SerializeField]
    private Spawner _allySpawner;
    [SerializeField]
    private Spawner _enemySpawner;
    [SerializeField]
    private List<Transform> _itemSpawner = new List<Transform>();
    [SerializeField]
    private List<GameObject> _props = new List<GameObject>();

    private uint _waveCount = 0;
    private uint _killCount = 0;
    private uint _spawnCount = 0;
    private float _spawnTimer = 0;
    private ManualController _allyController;
    private List<AutomaticController> _enemyControllers = new List<AutomaticController>();
    private Dictionary<Item, Item> _itemDictionary = new Dictionary<Item, Item>();

    private static readonly int EnemyMaxCount = 50;
    private static readonly float StartSpawnTime = 11;
    private static readonly float PrepareSpawnTime = 6f;
    private static readonly float SpawnWaitingTime = 2f;
    private static readonly float SpawnRestingTime = 31;
    private static readonly float PlayEndTime = 2;
    private static readonly float ShowPopupTime = 0.5f;

#if UNITY_EDITOR
    private void OnValidate()
    {
        SetProps(false);
    }
#endif

    private void Awake()
    {
        SetProps(true);
        Obstacle[] obstacles = GetComponentsInChildren<Obstacle>();
        foreach(Obstacle obstacle in obstacles)
        {
            obstacle.Initialize(SpawnItem);
        }
        SpawnAlly(GameData.ally);
        _spawnTimer = StartSpawnTime;
        _state?.SetNotice("<color=white>전투를 준비하세요.\n 중앙에서 적들이 내려옵니다.</color>");
    }

    private void OnEnable()
    {
        StartCoroutine(DoSpawnEnemy());
        IEnumerator DoSpawnEnemy()
        {
            while(true)
            {
                if (_spawnCount > 0)
                {
                    switch(GameData.enemy)
                    {
                        case GameData.Enemy.Hunter:
                            SpawnEnemy(Character.Hunter);
                            break;
                        case GameData.Enemy.Raptor:
                            SpawnEnemy(Character.Raptor);
                            break;
                        case GameData.Enemy.Mix:
                            SpawnEnemy(Random.Range(0, 2) == 0 ? Character.Hunter: Character.Raptor);
                            break;
                    }
                    _spawnCount--;
                    yield return new WaitForSeconds(SpawnWaitingTime);
                }
                else
                {
                    yield return new WaitUntil(() => _spawnCount > 0);
                }
            }
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        if (_allyController != null && _allyController.alive == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _state?.ShowPause(true);
            }
            else
            {
                if (_spawnTimer > 0)
                {
                    _spawnTimer -= Time.deltaTime;
                    if (_spawnTimer <= 0)
                    {
                        _state?.SetNotice();
                        _spawnCount += getStatBundle.GetEnemyCount(_waveCount);
                        _waveCount++;
                        _state?.SetWave(_waveCount);
                        _spawnTimer = SpawnRestingTime;
                        int count = _itemPrefabs.Count;
                        if (count > 0)
                        {
                            foreach (Transform transform in _itemSpawner)
                            {
                                if (transform != null)
                                {
                                    SpawnItem(_itemPrefabs[Random.Range(0, count)], transform.position);
                                }
                            }
                        }
                    }
                    else if (_spawnTimer < PrepareSpawnTime)
                    {
                        _state?.SetNotice("<color=red>" + Mathf.Floor(_spawnTimer).ToString() + "</color>");
                    }
                }
                _state?.SetTimer(_spawnTimer);
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

    private void SetLife(uint current, uint max, Controller controller)
    {
        if (controller == _allyController)
        {
            _state?.SetLife(current, max);
            if(controller.alive == false)
            {
                StartCoroutine(DoStopGame());
                IEnumerator DoStopGame()
                {
                    float survivalTime = _waveCount > 0 ? StartSpawnTime + ((_waveCount - 1) * SpawnRestingTime) + (SpawnRestingTime - _spawnTimer): StartSpawnTime - _spawnTimer;
                    bool record = GameData.Save(GameData.ally, GameData.enemy, _killCount, survivalTime);
                    yield return new WaitForSeconds(PlayEndTime);
                    if (record == false)
                    {
                        _state?.SetNotice("<color=red>패배</color>");
                    }
                    else
                    {
                        _state?.SetNotice("<color=red>기록 달성!</color>");
                    }
                    yield return new WaitForSeconds(ShowPopupTime);
                    _state?.ShowPause(false);
                }
            }
        }
        else if (controller.alive == false)
        {
            _state?.SetKill(++_killCount);
        }
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
                _allyController.Initialize((current, max) => { _state?.SetStamina(current, max); }, SetLife, (value) => { _state?.SetAttackDamage(value); }, (value) => { _state?.SetAttackSpeed(value); });
                _allyController.Set(getStatBundle.GetAllyStat(human));
                _allyController.Revive();
                Transform transform = character.transform;
                if (_cinemachineVirtualCamera != null)
                {
                    _cinemachineVirtualCamera.Follow = transform;
                    _cinemachineVirtualCamera.LookAt = transform;                    
                }
                if(_boxRevealer != null)
                {
                    _boxRevealer.target = transform;
                }
            }
        }
    }

    private void SpawnEnemy(bool human)
    {
        if (_enemySpawner != null)
        {
            int count = _enemyControllers.Count;
            for (int i = 0; i < count; i++)
            {
                AutomaticController enemyController = _enemyControllers[i];
                if (enemyController.gameObject.activeSelf == false && enemyController.IsHuman() == human)
                {
                    _enemySpawner.Get(enemyController.character);
                    enemyController.Set(getStatBundle.GetEnemyStat(human, _waveCount));
                    enemyController.Revive();
                    return;
                }
            }
            if (count < EnemyMaxCount)
            {
                Character character = human == true ? _enemySpawner.Get(_hunterCharacter) : _enemySpawner.Get(_raptorCharacter);
                if (character != null)
                {
                    AutomaticController automaticController = character.gameObject.GetComponent<AutomaticController>();
                    if (automaticController == null)
                    {
                        automaticController = character.gameObject.AddComponent<AutomaticController>();
                    }
                    automaticController.Initialize(SetLife, _allyController);
                    automaticController.Set(getStatBundle.GetEnemyStat(human, _waveCount));
                    automaticController.Revive();
                    _enemyControllers.Add(automaticController);
                }
            }
        }
    }

    private void SpawnItem(Item item, Vector3 position)
    {
        if(item != null)
        {
            foreach (KeyValuePair<Item, Item> kvp in _itemDictionary)
            {
                if (position == kvp.Key.position && kvp.Key.gameObject.activeInHierarchy == true)
                {
                    return;
                }
            }
            foreach (KeyValuePair<Item, Item> kvp in _itemDictionary)
            {
                if (kvp.Value == item && kvp.Key.gameObject.activeInHierarchy == false)
                {
                    kvp.Key.transform.position = position;
                    kvp.Key.gameObject.SetActive(true);
                    return;
                }
            }
            _itemDictionary.Add(Instantiate(item, position, Quaternion.identity), item);
        }
    }
}