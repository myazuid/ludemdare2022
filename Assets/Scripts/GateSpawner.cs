using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GateSpawner : MonoBehaviour
{
    public static GateSpawner instance;

    List<string> worldsList = new List<string>();
    [SerializeField] private TextAsset worldsCsv;

    // Start is called before the first frame update
    void Awake()
    {
        SetupWorldsList(worldsCsv);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void SetupWorldsList(TextAsset _worldsCsv)
    {
        char delim = '\n';
        worldsList = _worldsCsv.text.Split(delim).ToList();
    }

    public string ReturnUnusedWorldName()
    {
        int index = Random.Range(0, worldsList.Count);
        string world = worldsList[index];
        worldsList.RemoveAt(index); // so it can't be used again
        return world;
    }
}
