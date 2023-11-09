using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;
using DataContents;

public class DataManager
{
    #region [ Contents Data ]
    public Dictionary<int, DataByLevel> Dict_Stat { get; private set; } = new Dictionary<int, DataByLevel>();
    public Dictionary<eMonster, DataByMonster> Dict_Monster { get; private set; } = new Dictionary<eMonster, DataByMonster>();
    public PlayerData Data_Player = new PlayerData();
    public Inventorydata Data_Inven = new Inventorydata();
    public KillData Data_Kill = new KillData();
    #endregion [ Contents Data ]

    #region [ Const Strings ]
    const string JsonPathFormat = "Json/{0}";
    const string DBL = "DataByLevel";
    const string DBM = "DataByMonster";
    const string PLAYER = "PlayerData";
    const string INVEN = "InventoryData";
    const string KILL = "KillData";
    #endregion [ Const Strings ]

    #region [ Load ]
    public void Init() { LoadDictionaryData(); }    

    Loader LoadJsonToDictionary<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        string text = Managers.File.LoadJsonFile(path);
        if (string.IsNullOrEmpty(text) == false)
            return JsonUtility.FromJson<Loader>(text);
        else
        {
            TextAsset data = Managers.Resource.Load<TextAsset>(string.Format(JsonPathFormat, path));
            Loader datas = JsonUtility.FromJson<Loader>(data.ToString());
            Managers.File.SaveJsonFile(datas, path);
            return datas;
        }
    }
    void LoadDictionaryData()
    {
        Dict_Stat = LoadJsonToDictionary<StatData, int, DataByLevel>(DBL).Make();
        Dict_Monster = LoadJsonToDictionary<MonsterData, eMonster, DataByMonster>(DBM).Make();
    }

    public void LoadInGameData()
    {
        string data = Managers.File.LoadJsonFile(PLAYER);
        if (string.IsNullOrEmpty(data) == false)
            Data_Player = JsonUtility.FromJson<PlayerData>(data);
        data = Managers.File.LoadJsonFile(INVEN);
        if (string.IsNullOrEmpty(data) == false)
            Data_Inven = JsonUtility.FromJson<Inventorydata>(data);
        data = Managers.File.LoadJsonFile(KILL);
        if (string.IsNullOrEmpty(data) == false)
            Data_Kill = JsonUtility.FromJson<KillData>(data);
    }

    #endregion [ Load ]

    #region [ Save ]

    void Save()
    {
        Managers.File.SaveJsonFile(Data_Player, PLAYER);
        Managers.File.SaveJsonFile(Data_Inven, INVEN);
        Managers.File.SaveJsonFile(Data_Kill, KILL);
    }

    public void SaveInGameData()
    {
        Save();
    }

    public void ResetInGameData()
    {
        Data_Player = new PlayerData();
        Data_Inven = new Inventorydata();
        Data_Kill = new KillData();

        Save();
    }

    #endregion [ Save ]
}
