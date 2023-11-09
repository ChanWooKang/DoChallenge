using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerEx : MonoBehaviour
{
    #region [ Singleton ]
    static GameManagerEx _uniqueInstance;
    public static GameManagerEx Game { get { return _uniqueInstance; } }
    #endregion [ Singleton]

    public PlayerCtrl player;
}
