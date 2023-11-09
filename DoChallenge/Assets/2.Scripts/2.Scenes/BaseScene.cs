using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public abstract class BaseScene : MonoBehaviour
{
    #region [ Field Data ]
    public List<CursorUnit> list_Cursor;
    #endregion [ Field Data ]

    #region [ Property ]
    public eScene PrevScene { get; protected set; } = eScene.Unknown;
    public eScene CurrScene { get; protected set; } = eScene.Unknown;
    #endregion [ Property ]

    void Awake() { Init(); }
    
    protected virtual void Init()
    {
        Managers.Scene.AddCursor();
        Managers.Scene.CurrentScene = this;
    }

    public void SceneLoad(eScene scene)
    {
        StartCoroutine(Managers.Scene.LoadCoroutine(scene));
    }

    public virtual void Clear()
    {
        PoolingManager.Pool.Clear();
        PrevScene = CurrScene;
    }

    //public abstract void NewGame();
    //public abstract void ContinueGame();

    //public virtual void Quit() { }
}
