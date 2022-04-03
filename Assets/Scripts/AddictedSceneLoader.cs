using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AddictedSceneLoader : MonoBehaviour
{
    public String[] scenesToLoad;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < scenesToLoad.Length; i++)
        {
            SceneManager.LoadScene(scenesToLoad[i], LoadSceneMode.Additive);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
