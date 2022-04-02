using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingTravellerSpawning : MonoBehaviour
{
    [SerializeField] GameObject travellerPrefab;

    [SerializeField] List<GameObject> gates = new List<GameObject>();

    float nextSpawnTime = 0;
    float spawnFrequency = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
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
        int startGate = Random.Range(0, gates.Count);
        int endGate;
        do
        {
            endGate = Random.Range(0, gates.Count);
        } while (endGate == startGate);

        var traveller =  Instantiate(travellerPrefab,
            gates[startGate].transform.position,
            Quaternion.identity);

        traveller.GetComponent<TravellerController>().startGate = gates[startGate];
        traveller.GetComponent<TravellerController>().endGate = gates[endGate];
    }
}
