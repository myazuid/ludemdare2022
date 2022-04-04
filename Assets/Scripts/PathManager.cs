using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public static PathManager instance;

    public List<PathController> pathList = new List<PathController>();

    public List<float> pathLevelSpeedMultiplier = new List<float>();

    Transform gatesParent;
    GameObject pathsParent;

    public static Action<PathController> OnPathUpgraded;

    [SerializeField] private GameObject pathPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            gatesParent = GameObject.Find("Gates").transform;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        if (pathPrefab == null)
            Debug.LogError("pathPrefab is missing a prefab");


        GeneratePaths();
    }

    public void GeneratePaths()
    {
        pathsParent = new GameObject();
        pathsParent.name = "Paths";

        for (int i = 0; i < gatesParent.childCount; i++)
        {
            for (int j = 0; j < gatesParent.childCount; j++)
            {
                var firstGate = gatesParent.GetChild(i).gameObject;
                var secondGate = gatesParent.GetChild(j).gameObject;

                if (firstGate == secondGate || !firstGate.activeSelf || !secondGate.activeSelf)
                {
                    continue;
                }

                // if the path isn't found in the list
                if (ReturnPathLevel(firstGate,secondGate) == -1)
                {                    
                    var newPath = Instantiate(pathPrefab, Vector2.zero,
                        Quaternion.identity);
                    newPath.transform.SetParent(pathsParent.transform);

                    var newPathController = newPath.GetComponent<PathController>();

                    newPathController.gate1 = firstGate;
                    newPathController.gate2 = secondGate;
                    newPathController.SetSpriteShapePathPoints();

                    pathList.Add(newPathController);
                }
            }
        }
    }

    public int ReturnPathLevel(
        GameObject _gateA, GameObject _gateB)
    {
        foreach (var path in pathList)
        {
            if (path.gate1 == _gateA && path.gate2 == _gateB ||
                path.gate1 == _gateB && path.gate2 == _gateA)
            {
                return path.pathLevel;
            }
        }

        return -1; // path not found
    }

    public PathController ReturnPathController(GameObject _gateA,
        GameObject _gateB)
    {
        foreach (var path in pathList)
        {
            if (path.gate1 == _gateA && path.gate2 == _gateB ||
                path.gate1 == _gateB && path.gate2 == _gateA)
            {
                return path;
            }
        }

        return null; // path not found
    }
}
