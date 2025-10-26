using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        throw null;
                    }
#endif
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                }

                if (_instance)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        throw null;
                    }
#endif
                    _instance.OnMonoSingleToonInit();
                    DontDestroyOnLoad(_instance.gameObject);
                }
            }

            return _instance;
        }
    }

    protected virtual void OnMonoSingleToonInit()
    {
    }
}