using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-101)]
public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    [SerializedDictionary("Type", "Data")]
    [SerializeField] private SerializedDictionary<Turret_Type, TurretDataCollection> TurretDatas;

    [SerializedDictionary("Type", "Data")]
    [SerializeField] private SerializedDictionary<Enemy_Type, EnemyDataCollection> EnemyDatas;

    [SerializedDictionary("Type", "Data")]
    public SerializedDictionary<Player_Card_Type, CardDataCollection> CardDatas;

    public int GameLevel;

    [SerializeField] private List<InGameMapData> MapDatas;
    private readonly List<InGameMapData> candidates = new List<InGameMapData>();



    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Instance.InitMapData();
            Destroy(gameObject);
        }
    }

    //제작 데이터 반환
    public CraftingItemDataBase GetCraftingData(Item_Type type)
    {
        return null; // 아이템 추가시 수정
    }
    public CraftingItemDataBase GetCraftingData(Turret_Type type)
    {
        return TurretDatas[type].MakeData;
    }

    //터렛 데이터 반환
    public TurretData GetTurretData(Turret_Type type)
    {
        return TurretDatas[type].Data;
    }

    public bool IsTurretData(Turret_Type type)
    {
        return TurretDatas.ContainsKey(type);
    }

    //적 데이터 반환
    public EnemyDataCollection GetEnemyData(Enemy_Type type)
    {
        return EnemyDatas[type];
    }

    public bool IsEnemyData(Enemy_Type type)
    {
        return true;
    }

    //맵 데이터 선정 및 반환
    public void InitMapData()
    {
        foreach (InGameMapData mapData in MapDatas)
        {
            mapData.IsUsed = false;
        }
    }
    public void SetMapUsed(MapData targetMapData)
    {
        for (int i = 0; i < MapDatas.Count; i++)
        {
            InGameMapData inGameMapData = MapDatas[i];

            if (inGameMapData.MapData == null)
            {
                continue;
            }

            if (inGameMapData.MapData == targetMapData)
            {
                inGameMapData.IsUsed = true;
                return;
            }
        }
    }

    public MapData GetMap(int CurrentIndex)
    {
        if (MapDatas == null || MapDatas.Count == 0)
        {
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
            return null;
        }

        return selectedMapData.MapData;
    }

    private InGameMapData GetUnusedRandomMapByDifficultyWithFallback(int startDifficulty)
    {
        int maxDifficulty = 5;

        startDifficulty = Mathf.Clamp(startDifficulty, 1, 5);

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

    public int GetRandomRoomCount()
    {
        switch (GameLevel)
        {
            case 0:
                return 0;

            case 1:
                return UnityEngine.Random.Range(2, 4); // 3 ~ 4

            case 2:
                return UnityEngine.Random.Range(2, 5); // 3 ~ 5

            case 3:
                return UnityEngine.Random.Range(4, 6); // 5 ~ 6

            default:
                return 2;
        }
    }

    //플레이어 카드
    public PlayerCardBase GetCardData(Player_Card_Type type)
    {
        return CardDatas[type].Card;
    }

    public PlayerCardBase GetRandomCardCollection()
    {
        Array cardTypes = Enum.GetValues(typeof(Player_Card_Type));

        for (int i = 0; i < cardTypes.Length; i++)
        {
            Player_Card_Type type = (Player_Card_Type)cardTypes.GetValue(i);

            if ((int)type < 3)
            {
                continue;
            }

            if (!CardDatas.TryGetValue(type, out CardDataCollection collection))
            {
                continue;
            }

            if (collection == null || collection.Data == null)
            {
                continue;
            }

            if (!GameManager.Instance.Inventory.IsCanGetCard(type))
            {
                continue;
            }

            float randomValue = UnityEngine.Random.Range(0f, 100f);

            if (randomValue <= collection.Data.ShowPer)
            {
                collection.Card.Init(GameManager.Instance.Player);
                return collection.Card;
            }
        }

        return GetGuaranteedRandomCardCollection();
    }

    private PlayerCardBase GetGuaranteedRandomCardCollection()
    {
        for (int tryCount = 0; tryCount < 10; tryCount++)
        {
            int randomIndex = UnityEngine.Random.Range(0, 3);
            Player_Card_Type type = (Player_Card_Type)randomIndex;

            if (!CardDatas.TryGetValue(type, out CardDataCollection collection))
            {
                continue;
            }

            if (collection == null)
            {
                continue;
            }

            collection.Card.Init(GameManager.Instance.Player);
            return collection.Card;
        }

        return null;
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

[Serializable]
public class EnemyDataCollection
{
    public MobWeaponData Data;
    public Sprite Icon;
    public string Name;
}

[Serializable]
public class CardDataCollection
{
    public PlayerCardBase Card;
    public PlayerCardScriptableData Data;

    public void ApplyCardEffect()
    {
        Card.Apply();
    }
}