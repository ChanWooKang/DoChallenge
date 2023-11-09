using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    #region [ SingleTon ]

    static Managers _uniqueInstance;
    static Managers Instance { get { return _uniqueInstance; } }

    #endregion [ SingleTon ]

    #region [ Core ]
    DataManager _data = new DataManager();
    FileManager _file = new FileManager();
    InputManager _input = new InputManager();
    ResourceManager _resource = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();

    public static DataManager Data { get { return Instance._data; } }
    public static FileManager File { get { return Instance._file; } }
    public static InputManager Input { get { return Instance._input; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }

    #endregion [ Core ]

    void Awake()
    {
        _uniqueInstance = this;
        DontDestroyOnLoad(gameObject);

        _uniqueInstance._file.Init();
        _uniqueInstance._data.Init();
        _uniqueInstance._scene.Init();
    }

    void Update()
    {
        _input.OnUpdate();
    }

    public static void Clear()
    {
        Input.Clear();
        Scene.Clear();
    }
}
