using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class Singleton<T>:MonoBehaviour where T : MonoBehaviour {

    private static T instance = null; 

    public static T Instance {
        get
        {
            CreateInstance();
            return instance; 
        }
    }

	virtual protected void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Tienes mas de una copia del singleton"); 
        }
        CreateInstance(); 
    }

    static void CreateInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<T>();
            Scene actualScene = SceneManager.GetActiveScene();
            SceneManager.sceneUnloaded += (scene) =>
            {
                if (scene.name == actualScene.name)
                    instance = null;
            };

        }
  
    }

}
