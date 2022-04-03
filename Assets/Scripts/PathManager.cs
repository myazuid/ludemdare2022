using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public static PathManager instance;

    public List<Path> pathList = new List<Path>();

    [SerializeField] List<float> pathLevelSpeedMultiplier = new List<float>();

    Transform gatesParent;

    public static Action OnPathUpgraded;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        gatesParent = GameObject.Find("Gates").transform;

        GeneratePathList();
    }

    private void GeneratePathList()
    {
        for (int i = 0; i < gatesParent.childCount; i++)
        {
            for (int j = 0; j < gatesParent.childCount; j++)
            {
                var firstGate = gatesParent.GetChild(i).gameObject;
                var secondGate = gatesParent.GetChild(j).gameObject;

                if (firstGate == secondGate)
                {
                    continue;
                }

                // if the path isn't found in the list
                if (ReturnPathLevel(firstGate,secondGate) == -1)
                {
                    pathList.Add(new Path(firstGate, secondGate));
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
}

[System.Serializable]
public class Path
{
    public GameObject gate1; // one of the gates
    public GameObject gate2; // the second gate
    public int pathLevel; // the upgrade level (starts at 0)
    public float pathSpeed; // between 0 and 1

    public Path(GameObject _gate1, GameObject _gate2)
    {
        gate1 = _gate1;
        gate2 = _gate2;
        pathLevel = 0;
        pathSpeed = 1;
    }

    public void SetPathSpeed(float _speed)
    {
        var clampedSpeed = Mathf.Clamp(_speed, 0, 1);
        pathSpeed = clampedSpeed;
    }

    public void UpgradePath()
    {
        // to upgrade pathLevel
        PathManager.OnPathUpgraded?.Invoke();
    }
}
