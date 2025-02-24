using System.Collections.Generic;
using UnityEngine;

public class StatBundle : MonoBehaviour
{
    [Header("�����ϴ� ĳ������ �ɷ�ġ")]
    [Header("��ɲ� �ɷ�ġ"), SerializeField]
    private Stat _allyHunterStat;
    [Header("���� �ɷ�ġ"), SerializeField]
    private Stat _allyRaptorStat;

    [Header("�����Ǵ� ĳ������ �ɷ�ġ")]
    [Header("��ɲ� �ɷ�ġ"), SerializeField]
    private List<Stat> _enemyHunterStat = new List<Stat>();
    [Header("���� �ɷ�ġ"), SerializeField]
    private List<Stat> _enemyRaptorStat = new List<Stat>();

    [Header("�����Ǵ� ĳ������ ��"), SerializeField]
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