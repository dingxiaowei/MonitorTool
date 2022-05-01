using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DefaultNamespace;
using UnityEngine;
using XLua;

public class XLuaManager : Singleton<XLuaManager>
{
     public LuaEnv luaenv;
     public string luaPath;
     
    public override void Initialize(Options options = null)
    {
        luaenv = new LuaEnv();
        if (string.IsNullOrEmpty(luaPath))
        {
            luaPath = string.Format("{0}/{1}/",Application.dataPath,"Lua");
            Debug.Log($"load lua root path:{luaPath}");
        }

        luaenv.AddLoader(LuaLoader);
        base.Initialize(options);
    }
    
    public void Start(string luaStart, Action act = null)
    {
        luaenv.DoString($"require '{luaStart}'");
        act?.Invoke();
    }

    /// <summary>
    /// Lua Loader
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private byte[] LuaLoader(ref string fileName)
    {
        string fullPath = string.Format(luaPath + fileName + ".lua");

        if (File.Exists(fullPath))
        {
            return File.ReadAllBytes(fullPath);
        }
        else
        {
            Debug.Log("LuaLoader重定向失败，文件名为" + fileName);
        }
        return null;
    }
}
