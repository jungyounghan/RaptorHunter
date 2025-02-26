using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 게임 진행에 필요한 정보들을 담고 있는 클래스
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

    //랭킹 저장
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
        string json = JsonUtility.ToJson(new Ranking.InfoList(list.ToArray()), true); // true = 보기 좋게 포맷
        File.WriteAllText(FilePath, json); // 파일에 저장
    }

    public static Ranking.Info[] Load()
    {
        // 파일이 존재하는지 확인
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath); // 파일에서 JSON 읽기
            try
            {
                Ranking.InfoList infoList = JsonUtility.FromJson<Ranking.InfoList>(json); // JSON을 객체로 변환
                return infoList.infos;
            }
            catch
            {
                return null; // 예외 발생 시 null 반환
            }
        }
        return null;
    }
}