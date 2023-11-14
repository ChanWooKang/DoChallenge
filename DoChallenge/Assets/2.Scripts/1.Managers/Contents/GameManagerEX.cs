using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerEX : MonoBehaviour
{
    #region [ Singleton ]
    static GameManagerEX _uniqueInstance;
    public static GameManagerEX Game { get { return _uniqueInstance; } }
    #endregion [ Singleton ]

    public PlayerCtrl player;
}
