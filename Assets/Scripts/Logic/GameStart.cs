using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    private XLuaManager _mLuaManager;
    public string startLuaFileName;

    private void Awake()
    {
        _mLuaManager = XLuaManager.GetInstance();
        _mLuaManager.Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {
        _mLuaManager.Start(startLuaFileName, () => {
            Debug.Log("Start Lua Finished!");
        });
    }

    private void OnDisable()
    {
        _mLuaManager.Dispose();
    }
}