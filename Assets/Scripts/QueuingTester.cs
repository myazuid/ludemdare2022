using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueuingTester : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] float gapBetweenQueuingTravellers;
    [SerializeField] float gapBetweenQueuingCircles;
    [SerializeField] float baseQueueRadius;

    List<GameObject> enemies = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

        for (int i = 0; i < 100;i++)
        {
            print("Running the " + i + "traveller spawn");
            AddTravellerToTheQueue(i);
        }
    }

    public void AddTravellerToTheQueue(int index)
    {
        var spawnPos = FindNextAvailableQueuePosition();
        var enemy = Instantiate(prefab, spawnPos, Quaternion.identity) as GameObject;
        enemy.name = "Traveller " + index;
        enemies.Add(enemy);
    }

    public Vector2 FindNextAvailableQueuePosition()
    {
        float circleRadius = baseQueueRadius;
        bool positionFound;
        int numberOfPlacesInPreviousCircles = 0;
        float positionRoundCircle = -1;
        int circleNumber = 0;

        for (int i = 0; i < enemies.Count + 1; i++)
        {
            positionFound = false;

            while (!positionFound)
            {
                // circumference of the circle
                var circumference = Mathf.PI * circleRadius;

                // the position in this circle that we're checking
                var placeInThisCircle = i - numberOfPlacesInPreviousCircles;

                // the position around this circle for this one
                float gap;
                if (circleNumber > 0)
                {
                    gap = gapBetweenQueuingTravellers - (0.1f * circleNumber);
                }
                else
                {
                    gap = gapBetweenQueuingTravellers;
                }
                positionRoundCircle = circumference - (gap * placeInThisCircle);

                if (positionRoundCircle < 0)
                {
                    circleNumber++;
                    numberOfPlacesInPreviousCircles = i;
                    circleRadius += gapBetweenQueuingCircles;
                }
                else
                {
                    positionFound = true;
                }
            }
        }

        /* Get the vector direction */
        var vertical = Mathf.Sin(positionRoundCircle);
        var horizontal = Mathf.Cos(positionRoundCircle);

        var spawnDir = new Vector2(horizontal, vertical);
        if (circleNumber > 0)
        {
            spawnDir += (spawnDir.normalized * gapBetweenQueuingCircles * circleNumber);
        }
        var spawnPos = (Vector2)transform.position + spawnDir;

        return (spawnPos);
    }
}
