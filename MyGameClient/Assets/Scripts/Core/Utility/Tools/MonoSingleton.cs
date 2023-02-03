using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance 
    {
        get
        {
            if (instance == null)
            {
                if(FindObjectOfType<T>() != null)
                {
                    instance = FindObjectOfType<T>();
                }
                else
                {
                    GameObject gameObject = new GameObject(typeof(T).Name);
                    gameObject.transform.localPosition = Vector3.zero;
                    gameObject.transform.localRotation = Quaternion.identity;
                    gameObject.transform.localScale = Vector3.one;
                    instance = gameObject.AddComponent<T>();
                }
            }
            return instance; 
        }
    }

    protected virtual void Awake()
    {
        //  DontDestroyOnLoad(this.gameObject);

        if (default(T) == instance)
        {
            instance = this as T;
        }
        else if (instance != this)
        {
            Debug.LogError("The class already has an instance. ");
            DestroyImmediate(this);
        }
    }
}
