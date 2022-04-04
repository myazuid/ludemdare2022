using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GateController : MonoBehaviour
{
    public List<GameObject> outboundTravellerQueue =
        new List<GameObject>();

    private SpriteRenderer spriteRenderer;

    public static Action<GameObject, bool> OnTravellerProcessed;

    [SerializeField] private GameObject beamEffect;

    [HideInInspector]
    public int gateLevel = 0;
    private float timeOfNextGateProcessing = 0;
    private float gateProcessingFrequency = 1;
    public int MaxGateLevel
    {
        get
        {
            return GameController.instance.gateLevels.Count - 1;
        }
    }

    [SerializeField] private TextMeshProUGUI gateNameText;

    float gapBetweenQueuingTravellers = 0.3f;
    float gapBetweenQueuingCircles = 0.3f;
    float baseQueueRadius = 2;

    public GameObject beamPrefab;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        gateNameText.text = GateSpawner.instance.ReturnUnusedWorldName();
        PathManager.instance.GeneratePaths();
        Instantiate(beamPrefab, transform.position, Quaternion.identity);
    }

    private void Update()
    {
        if (gateLevel > 0 && outboundTravellerQueue.Count > 0) // if it is automated at all
        {
            if (Time.time > timeOfNextGateProcessing)
            {
                for (int i = 0; i < gateLevel; i++)
                {
                    ProcessOutboundTraveller();
                }

                timeOfNextGateProcessing = Time.time + gateProcessingFrequency;
            }
        }
    }

    private void OnMouseDown()
    {
        UpgradeGate();

        UIController.instance.showTooltip(this); // to refresh it
    }

    private void OnMouseEnter()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r,
            spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);
        
        UIController.instance.showTooltip(this);

    }

    private void OnMouseExit()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r,
            spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        UIController.instance.hideTooltip();
    }

    public void ProcessOutboundTraveller()
    {
        if (outboundTravellerQueue.Count == 0)
        {
            return;
        }
        
        var processedTraveller = outboundTravellerQueue[0];

        outboundTravellerQueue.RemoveAt(0);

        Instantiate(beamEffect, processedTraveller.transform.position, Quaternion.identity);

        Destroy(processedTraveller);

        for (int i = 0; i < outboundTravellerQueue.Count; i++)
        {
            var traveller = outboundTravellerQueue[i].GetComponent<TravellerController>();
            traveller.queuingPosition = FindQueuePositionAtIndex(i);
        }

        //fire event to GameController to handle payments/remove traveller from global list
        OnTravellerProcessed?.Invoke(processedTraveller, true);
    }

    public void RemoveTravellerFromQueueAndShiftDownQueue(GameObject _traveller)
    {
        var index = outboundTravellerQueue.IndexOf(_traveller);

        outboundTravellerQueue.Remove(_traveller);

        for (int i = 0; i < outboundTravellerQueue.Count; i++)
        {
            var traveller = outboundTravellerQueue[i].GetComponent<TravellerController>();
            traveller.queuingPosition = FindQueuePositionAtIndex(i);
        }
    }

    public void UpgradeGate()
    {
        if (gateLevel < MaxGateLevel)
        {
            var success = GameController.instance.SpendFromBalance(
            GameController.instance.gateLevels[gateLevel].upgradeCost);
            if (success)
            {
                gateLevel++;
                //spriteRenderer.sprite = gateSprites[Mathf.Min(gateLevel, gateSprites.Length - 1)];
                spriteRenderer.sprite = GameController.instance.gateLevels[gateLevel].sprite;
            }
        }   
    }

    public Vector2 FindNextAvailableQueuePosition()
    {
        float circleRadius = baseQueueRadius;
        bool positionFound;
        int numberOfPlacesInPreviousCircles = 0;
        float positionRoundCircle = -1;
        int circleNumber = 0;

        for (int i = 0; i < outboundTravellerQueue.Count + 1; i++)
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

    // COPY OF ABOVE DO NOT EDIT!!!
    public Vector2 FindQueuePositionAtIndex(int _index)
    {
        float circleRadius = baseQueueRadius;
        bool positionFound;
        int numberOfPlacesInPreviousCircles = 0;
        float positionRoundCircle = -1;
        int circleNumber = 0;

        for (int i = 0; i < _index + 1; i++)
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