using System.Collections.Generic;
using UnityEngine;

public class StatBundle : MonoBehaviour
{
    [Header("조종하는 캐릭터의 능력치")]
    [Header("사냥꾼 능력치"), SerializeField]
    private Stat _allyHunterStat;
    [Header("랩터 능력치"), SerializeField]
    private Stat _allyRaptorStat;

    [Header("스폰되는 캐릭터의 능력치")]
    [Header("사냥꾼 능력치"), SerializeField]
    private List<Stat> _enemyHunterStat = new List<Stat>();
    [Header("랩터 능력치"), SerializeField]
    private List<Stat> _enemyRaptorStat = new List<Stat>();

    [Header("스폰되는 캐릭터의 수"), SerializeField]
    private List<uint> _enemyWaveCount = new List<uint>();

    public Stat GetAllyStat(bool human)
    {
        return human == true ? _allyHunterStat : _allyRaptorStat;
    }

    public Stat GetEnemyStat(bool human, uint wave)
    {
        switch(human)
        {
            case Character.Hunter:
                if (_enemyHunterStat.Count > 0)
                {
                    if (wave >= _enemyHunterStat.Count)
                    {
                        return _enemyHunterStat[_enemyHunterStat.Count - 1];
                    }
                    else
                    {
                        return _enemyHunterStat[(int)wave];
                    }
                }
                break;
            case Character.Raptor:
                if(_enemyRaptorStat.Count > 0)
                {
                    if(wave >= _enemyRaptorStat.Count)
                    {
                        return _enemyRaptorStat[_enemyRaptorStat.Count - 1];
                    }
                    else
                    {
                        return _enemyRaptorStat[(int)wave];
                    }
                }
                break;
        }
        return null;
    }


    public uint GetEnemyCount(uint wave)
    {
        if(_enemyWaveCount.Count > 0)
        {
            if (wave >= _enemyWaveCount.Count)
            {
                return _enemyWaveCount[_enemyWaveCount.Count - 1];
            }
            else
            {
                return _enemyWaveCount[(int)wave];
            }
        }
        return 0;
    }
}