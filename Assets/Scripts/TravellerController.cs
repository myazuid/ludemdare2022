using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravellerController : MonoBehaviour
{
    public GameObject startGate, endGate;
    private GateController endGateController;
    Vector2 destination;

    // MOVEMENT
    [SerializeField] float travellerSpeed;
    [SerializeField] float thresholdDistanceToEnterQueue;
    private float frequencyToCheckProximityToEndGate = 1;
    private float nextTimeToCheckProximityToEndGate = 0;

    public enum TravellerState { Travelling, EnteringQueue , Queuing };
    private TravellerState travellerState = TravellerState.Travelling;

    // QUEUING STUFF
    float timeSpentQueuing;
    float minQueuingPatienceDuration = 10, maxQueuingPatienceDuration = 40;
    float queuingPatienceDuration;
    private float queueDistanceFromNextTraveller = 0.3f;

    public enum UnhappinessLevel { Happy, Unsatisfied, Angry };
    public UnhappinessLevel unhappinessLevel;

    private void Start()
    {
        queuingPatienceDuration = Random.Range(minQueuingPatienceDuration,
            maxQueuingPatienceDuration);

        destination = endGate.transform.Find("DestinationPoint").position;

        endGateController = endGate.GetComponent<GateController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (travellerState == TravellerState.Travelling)
        {
            MoveTowardsDestination(destination);
        }
        else if (travellerState == TravellerState.EnteringQueue)
        {
            MoveTowardsQueuePosition(destination);
        }
        else if (travellerState == TravellerState.Queuing)
        {
            CheckTravellerQueuingPatience();
        }
    }

    private void MoveTowardsDestination(Vector2 _destination)
    {
        var dir = _destination - (Vector2)transform.position;
        transform.Translate(dir.normalized * Time.deltaTime);

        CheckForProximityToEndGate(_destination, endGate);
    }

    private void CheckForProximityToEndGate(Vector2 _destination,
        GameObject _endGate)
    {
        if (Time.time > nextTimeToCheckProximityToEndGate)
        {
            var dist = Vector2.Distance(transform.position, _destination);

            if (dist < thresholdDistanceToEnterQueue)
            {
                StartEnteringQueue(_endGate);
            }

            nextTimeToCheckProximityToEndGate = Time.time +
                frequencyToCheckProximityToEndGate;
        }
    }

    private void StartEnteringQueue(GameObject _endGate)
    {
        travellerState = TravellerState.EnteringQueue;

        endGateController.outboundTravellerQueue.Add(this.gameObject);

        var queueStart = endGate.transform.Find("QueueStart").position;
        var xPos = queueStart.x + (queueDistanceFromNextTraveller *
            endGateController.outboundTravellerQueue.Count);

        destination = new Vector2(xPos, queueStart.y);
    }

    private void MoveTowardsQueuePosition(Vector2 _destination)
    {
        var dir = _destination - (Vector2)transform.position;
        transform.Translate(dir.normalized * Time.deltaTime);

        if (Time.time > nextTimeToCheckProximityToEndGate)
        {
            var dist = Vector2.Distance(transform.position, _destination);

            if (dist < 0.001f)
            {
                EnterQueue(endGate);
            }
            else
            {
                nextTimeToCheckProximityToEndGate = Time.time +
                frequencyToCheckProximityToEndGate;
            }
        }
    }

    private void EnterQueue(GameObject _endGate)
    {
        travellerState = TravellerState.Queuing;
    }

    private void CheckTravellerQueuingPatience()
    {
        timeSpentQueuing += Time.deltaTime;

        var happyThreshold = queuingPatienceDuration * 0.33f;
        var unsatisfiedThreshold = queuingPatienceDuration * 0.66f;

        if (timeSpentQueuing < happyThreshold)
        {
            if (unhappinessLevel != UnhappinessLevel.Happy)
            {
                unhappinessLevel = UnhappinessLevel.Happy;
            }
        }
        else if (timeSpentQueuing < unsatisfiedThreshold)
        {
            if (unhappinessLevel != UnhappinessLevel.Unsatisfied)
            {
                unhappinessLevel = UnhappinessLevel.Unsatisfied;
                print(this.gameObject.name + " is unsatisfied.");
            }

        }
        else if (timeSpentQueuing < queuingPatienceDuration)
        {
            if (unhappinessLevel != UnhappinessLevel.Angry)
            {
                unhappinessLevel = UnhappinessLevel.Angry;
                print(this.gameObject.name + " is angry!");
            }
        }
        else
        {
            ExitQueueAndLeave(endGate);
        }
    }

    private void ExitQueueAndLeave(GameObject _endGate)
    {
        print(this.gameObject.name + "says - Fuck this, I'm leaving!");

        // tell game controller

        var gateController = endGate.GetComponent<GateController>();
        gateController.RemoveTravellerFromQueueAndShiftDownQueue(this.gameObject);

        Destroy(this.gameObject);
    }
}