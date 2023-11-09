using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Define;

public class SceneManagerEx
{
    #region [ Field Data ]
    BaseScene _currScene;
    public Dictionary<eCursor, Texture2D> dict_Cursor;
    #endregion [ Field Data ]

    #region [ Property ]
    public BaseScene CurrentScene { get { InitCurrentScene(); return _currScene; } set { _currScene = value; } }
    #endregion [ Property ]
    
    public void Init()
    {
        dict_Cursor = new Dictionary<eCursor, Texture2D>();
    }

    void InitCurrentScene()
    {
        if (_currScene == null)
        {
            _currScene = GameObject.FindWithTag(Utility.ConvertEnum(eTag.Scene)).GetComponent<BaseScene>();
        }
    }

    public IEnumerator LoadCoroutine(eScene scene)
    {
        Managers.Clear();
        string sceneName = Utility.ConvertEnum(scene);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            yield return null;
        }
    }

    public void AddCursor()
    {
        if(CurrentScene.list_Cursor.Count > 0)
        {
            for(int i = 0; i < CurrentScene.list_Cursor.Count; i++)
            {
                CursorUnit unit = CurrentScene.list_Cursor[i];
                if (dict_Cursor.ContainsKey(unit.type) == false)
                    dict_Cursor.Add(unit.type, unit.tex);
                else
                    dict_Cursor[unit.type] = unit.tex;
            }
        }
    }

    public void Clear()
    {
        CurrentScene.Clear();
        dict_Cursor.Clear();
    }    
}
