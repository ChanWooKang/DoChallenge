using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileManager
{
    #region [ Const Strings ]
    const string EditorPath = "/@Datas/Json";
    const string WindowPath = "/Json";
    const string JsonSuffix = ".json";
    #endregion [ Const Strings ]
    string JsonPath = string.Empty;

    public void Init()
    {
    #if UNITY_EDITOR
        JsonPath = Application.dataPath + EditorPath;
    #else
        JsonPath = Application.persistentDataPath + WindowPath;
    #endif
    }

    void CheckJsonPath()
    {
        if (Directory.Exists(JsonPath) == false)
            Directory.CreateDirectory(JsonPath);
    }

    public void SaveJsonFile<T>(T data, string name)
    {
        CheckJsonPath();

        string path = Path.Combine(JsonPath, name) + JsonSuffix;

        if (File.Exists(path))        
            File.Delete(path);
        
        FileStream fs = new FileStream(path, FileMode.Create);
        try
        {
            string Json = JsonUtility.ToJson(data, true);
            byte[] datas = Encoding.UTF8.GetBytes(Json);
            fs.Write(datas, 0, datas.Length);
            fs.Close();
        }
        catch
        {
            fs.Close();
        }
    }

    public string LoadJsonFile(string name)
    {
        CheckJsonPath();
        string path = Path.Combine(JsonPath, name) + JsonSuffix;

        if (File.Exists(path) == false)
            return null;

        FileStream fs = new FileStream(path, FileMode.Open);
        try
        {
            byte[] datas = new byte[fs.Length];
            fs.Read(datas, 0, datas.Length);
            fs.Close();
            string JsonData = Encoding.UTF8.GetString(datas);
            return JsonData;
        }
        catch
        {
            fs.Close();
            return null;
        }
    }
}
