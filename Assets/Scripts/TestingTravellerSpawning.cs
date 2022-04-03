using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingTravellerSpawning : MonoBehaviour
{
    [SerializeField] GameObject travellerPrefab;

    float nextSpawnTime = 0;
    float spawnFrequency = 0.5f;

    Transform gatesParent;

    // Start is called before the first frame update
    void Start()
    {
        gatesParent = GameObject.Find("Gates").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextSpawnTime)
        {
            SpawnTraveller();
            nextSpawnTime = Time.time + spawnFrequency;
        }
    }

    private void SpawnTraveller()
    {
        int startGate = Random.Range(0, gatesParent.childCount);
        int endGate;
        do
        {
            endGate = Random.Range(0, gatesParent.childCount);
        } while (endGate == startGate);

        var traveller =  Instantiate(travellerPrefab,
            gatesParent.GetChild(startGate).transform.position,
            Quaternion.identity);

        traveller.GetComponent<TravellerController>().startGate =
            gatesParent.GetChild(startGate).gameObject; 
        traveller.GetComponent<TravellerController>().endGate =
            gatesParent.GetChild(endGate).gameObject;
    }
}
