using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// ���� ���࿡ �ʿ��� �������� ��� �ִ� Ŭ����
/// </summary>
public class GameData
{
    public static bool ally = Character.Hunter;

    public enum Enemy: byte
    {
        Hunter,
        Raptor,
        Mix
    }

    public static Enemy enemy = Enemy.Raptor;

    private readonly static string FilePath = Path.Combine(Application.persistentDataPath, "ranking.json");

    //��ŷ ����
    public static void Save(bool character, Enemy enemy, uint killCount, float survivalTime)
    {
        Ranking.Info[] rankings = Load();
        List<Ranking.Info> list = new List<Ranking.Info>();
        Ranking.Info ranking = new Ranking.Info(character, enemy, killCount, survivalTime);
        int length = rankings != null ? rankings.Length : 0;
        bool done = false;
        for (int i = 0; i < length; i++)
        {
            if (rankings[i].killCount < ranking.killCount)
            {
                list.Add(ranking);
                done = true;
            }
            list.Add(rankings[i]);
        }
        if(done == false)
        {
            list.Add(ranking);
        }
        string json = JsonUtility.ToJson(new Ranking.InfoList(list.ToArray()), true); // true = ���� ���� ����
        File.WriteAllText(FilePath, json); // ���Ͽ� ����
    }

    public static Ranking.Info[] Load()
    {
        // ������ �����ϴ��� Ȯ��
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath); // ���Ͽ��� JSON �б�
            try
            {
                Ranking.InfoList infoList = JsonUtility.FromJson<Ranking.InfoList>(json); // JSON�� ��ü�� ��ȯ
                return infoList.infos;
            }
            catch
            {
                return null; // ���� �߻� �� null ��ȯ
            }
        }
        return null;
    }
}