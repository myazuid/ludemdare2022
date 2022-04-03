using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCharacterDisplaySpawner : MonoBehaviour
{

    public GameObject prefab;
    // Start is called before the first frame update
    void Start()
    {
        // for (int y = 0; y < 10; y++)
        // {
        //     for (int x = 0; x < 10; x++)
        //     {
        //         Instantiate(prefab, new Vector3(x - 5f, y - 5f), Quaternion.identity);
        //     }
        // }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            point.z = 0;
            Instantiate(prefab, point, Quaternion.identity);
        }
    }
}
