using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance = null;
    protected virtual bool dontDestroyOnLoad { get; } = false;

    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    public static TYPE InstanceAs<TYPE>() where TYPE : T { return instance as TYPE; }

    public static void IfInstanceNotNull(System.Action<T> onNotNull = null, System.Action onNull = null)
    {
        if (instance != null)
        {
            onNotNull?.Invoke(instance);
        }
        else
        {
            onNull?.Invoke();
        }
    }

    protected virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = (T)this;
            if (dontDestroyOnLoad) { DontDestroyOnLoad(this); }
        }
    }

    protected virtual void OnDestroy()
    {
        if (this == instance)
        {
            instance = null;
        }
    }
}
