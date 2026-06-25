using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;
using static CreftingUI;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    [SerializedDictionary("Type", "Data")]
    [SerializeField] private SerializedDictionary<Turret_Type, TurretDataCollection> TurretDatas;

    public int GameLevel;

    [SerializeField] private List<InGameMapData> MapDatas;
    private readonly List<InGameMapData> candidates = new List<InGameMapData>();



    private void Awake()
    {
        Instance = this;
        MapDatas = new List<InGameMapData>();
        //if(Instance == null)
        //{
        //    Instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}
    }

    public CraftingItemDataBase GetCraftingData(Item_Type type)
    {
        return null; // 아이템 추가시 수정
    }
    public CraftingItemDataBase GetCraftingData(Turret_Type type)
    {
        return TurretDatas[type].MakeData;
    }

    public TurretData GetData(Turret_Type type)
    {
        return TurretDatas[type].Data;
    }

    public bool IsData(Turret_Type type)
    {
        return TurretDatas.ContainsKey(type);
    }

    public MapData GetMap(int CurrentIndex)
    {
        if (MapDatas == null || MapDatas.Count == 0)
        {
            Debug.LogWarning("MapDatas가 비어있습니다.");
            return null;
        }

        if (CurrentIndex < 0)
        {
            CurrentIndex = 0;
        }

        int targetDifficulty;

        if (GameLevel == 0)
        {
            // 게임 난이도 0은 무조건 난이도 1
            targetDifficulty = 1;
        }
        else
        {
            GetDifficultyRange(CurrentIndex, out int minDifficulty, out int maxDifficulty);

            if (CurrentIndex == 0)
            {
                // 첫 맵은 무조건 min 난이도
                targetDifficulty = minDifficulty;
            }
            else
            {
                // 이후 맵은 min ~ max 중 랜덤
                targetDifficulty = UnityEngine.Random.Range(minDifficulty, maxDifficulty + 1);
            }
        }

        InGameMapData selectedMapData = GetUnusedRandomMapByDifficultyWithFallback(targetDifficulty);

        if (selectedMapData == null)
        {
            Debug.LogWarning($"사용 가능한 맵을 찾지 못했습니다. GameLevel: {GameLevel}, CurrentIndex: {CurrentIndex}");
            return null;
        }

        selectedMapData.IsUsed = true;
        return selectedMapData.MapData;
    }

    private InGameMapData GetUnusedRandomMapByDifficultyWithFallback(int startDifficulty)
    {
        int maxDifficulty = GetMaxMapDifficulty();

        for (int difficulty = startDifficulty; difficulty <= maxDifficulty; difficulty++)
        {
            candidates.Clear();

            for (int i = 0; i < MapDatas.Count; i++)
            {
                InGameMapData inGameMapData = MapDatas[i];

                if (inGameMapData == null)
                {
                    continue;
                }

                if (inGameMapData.IsUsed)
                {
                    continue;
                }

                if (inGameMapData.MapData == null)
                {
                    continue;
                }

                if (inGameMapData.MapData.Difficult != difficulty)
                {
                    continue;
                }

                candidates.Add(inGameMapData);
            }

            if (candidates.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, candidates.Count);
                return candidates[randomIndex];
            }
        }

        return null;
    }

    private void GetDifficultyRange(int CurrentIndex, out int minDifficulty, out int maxDifficulty)
    {
        if (CurrentIndex < 0)
        {
            CurrentIndex = 0;
        }

        int roomNumber = CurrentIndex + 1;

        minDifficulty = GameLevel + roomNumber / 4 + roomNumber / 6;
        maxDifficulty = GameLevel + 1 + roomNumber / 3 + roomNumber / 5;

        minDifficulty = Mathf.Clamp(minDifficulty, 1, 5);
        maxDifficulty = Mathf.Clamp(maxDifficulty, 1, 5);
    }

    private int GetMaxMapDifficulty()
    {
        int maxDifficulty = 0;

        for (int i = 0; i < MapDatas.Count; i++)
        {
            InGameMapData inGameMapData = MapDatas[i];

            if (inGameMapData == null)
            {
                continue;
            }

            if (inGameMapData.MapData == null)
            {
                continue;
            }

            if (inGameMapData.MapData.Difficult > maxDifficulty)
            {
                maxDifficulty = inGameMapData.MapData.Difficult;
            }
        }

        return maxDifficulty;
    }

    public int GetRandomRoomCount()
    {
        switch (GameLevel)
        {
            case 0:
                return 2;

            case 1:
                return UnityEngine.Random.Range(3, 5); // 3 ~ 4

            case 2:
                return UnityEngine.Random.Range(3, 6); // 3 ~ 5

            case 3:
                return UnityEngine.Random.Range(5, 7); // 5 ~ 6

            default:
                Debug.LogWarning($"정의되지 않은 GameLevel입니다: {GameLevel}");
                return 2;
        }
    }
}

[Serializable]
public class TurretDataCollection
{
    public TurretData Data;

    public TurretCrafingData MakeData;
}

[Serializable]
public class InGameMapData
{
    public MapData MapData;
    public bool IsUsed;
}