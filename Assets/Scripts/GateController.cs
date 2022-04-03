using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    public List<GameObject> outboundTravellerQueue =
        new List<GameObject>();

    private SpriteRenderer spriteRenderer;

    public static Action<GameObject, bool> OnTravellerProcessed;

    [SerializeField] private GameObject beamEffect;

    private int gateLevel = 0;
    private float timeOfNextGateProcessing = 0;
    private float gateProcessingFrequency = 1;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (gateLevel > 0) // if it is automated at all
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
        if (outboundTravellerQueue.Count > 0)
        {
            ProcessOutboundTraveller();
        }
    }

    private void OnMouseEnter()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r,
            spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);
    }

    private void OnMouseExit()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r,
            spriteRenderer.color.g, spriteRenderer.color.b, 1f);
    }

    private void ProcessOutboundTraveller()
    {
        var processedTraveller = outboundTravellerQueue[0];

        outboundTravellerQueue.RemoveAt(0);

        Instantiate(beamEffect, processedTraveller.transform.position, Quaternion.identity);

        Destroy(processedTraveller);

        // shift the remaining ones
        foreach (var traveller in outboundTravellerQueue)
        {
            var travellerController = traveller.GetComponent<TravellerController>();
            travellerController.queuingPosition =
                new Vector2(travellerController.queuingPosition.x -
                travellerController.queueDistanceFromNextTraveller,
                travellerController.queuingPosition.y);
        }

        //fire event to GameController to handle payments/remove traveller from global list
        OnTravellerProcessed?.Invoke(processedTraveller, true);
    }

    public void RemoveTravellerFromQueueAndShiftDownQueue(GameObject _traveller)
    {
        var index = outboundTravellerQueue.IndexOf(_traveller);

        outboundTravellerQueue.Remove(_traveller);

        for (int i = index; i < outboundTravellerQueue.Count; i++)
        {
            var traveller = outboundTravellerQueue[i].GetComponent<TravellerController>();
            traveller.queuingPosition = new Vector2(traveller.queuingPosition.x -
                traveller.queueDistanceFromNextTraveller, traveller.queuingPosition.y);
        }
    }

    public void UpgradeGate()
    {
        gateLevel++;
    }
}