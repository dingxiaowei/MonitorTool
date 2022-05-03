using System;
using UnityEngine;
public abstract class Singleton<T> where T : class, new()
{
    public class Options
    {

    }

    protected static T p_instance;

    public static T GetInstance()
    {
        if (p_instance != null)
        {
            return p_instance;
        }
        p_instance = Activator.CreateInstance<T>();
        return p_instance;
    }
    public virtual void Initialize(Options options = null) { }
    public virtual void Dispose()
    {
        if (p_instance != null)
        {
            p_instance = null;
        }
    }
}

public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
{
    public class Options
    {

    }

    protected static T p_instance;

    public static T GetInstance(string objName, GameObject obj = null)
    {
        if (p_instance != null) return p_instance;
        if (obj == null)
        {
            obj = new GameObject("[" + objName + "]");
        }

        p_instance = (T)obj.AddComponent(typeof(T));
        return p_instance;
    }

    public virtual void Initialize(Options options = null)
    {
    }

    public virtual void Dispose()
    {
        if (p_instance != null)
        {
            p_instance = null;
        }

        UnityEngine.Object.Destroy(gameObject);
    }
}