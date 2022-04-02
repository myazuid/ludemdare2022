using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravellerController : MonoBehaviour
{
    [SerializeField] private GameObject startGate, endGate;
    float timeSpentQueuing;
    float queuingPatienceDuration = 30;
    [SerializeField] float travellerSpeed;
    [SerializeField] float thresholdDistanceToEnterQueue;
    private float frequencyToCheckProximityToEndGate = 1;
    private float nextTimeToCheckProximityToEndGate = 0;

    public enum UnhappinessLevelType { Happy,Unsatisfied,Angry,Leaving };

    private UnhappinessLevelType UnhappinessLevel
    {
        get
        {
            var happyThreshold = queuingPatienceDuration * 0.33f;
            var unsatisfiedThreshold = queuingPatienceDuration * 0.66f;

            if (timeSpentQueuing < happyThreshold)
            {
                return UnhappinessLevelType.Happy;
            }
            else if (timeSpentQueuing < unsatisfiedThreshold)
            {
                return UnhappinessLevelType.Unsatisfied;
            }
            else if (timeSpentQueuing < queuingPatienceDuration)
            {
                return UnhappinessLevelType.Angry;
            }
            else
            {
                return UnhappinessLevelType.Leaving;
            }
        }
    }

    public enum TravellerState { Travelling, Queuing};
    private TravellerState travellerState = TravellerState.Travelling;

    private float queueDistanceFromNextTraveller = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (travellerState == TravellerState.Travelling)
        {
            MoveTowardsEndGate(endGate);
        }
        else if (travellerState == TravellerState.Queuing)
        {
            timeSpentQueuing += Time.deltaTime;
        }
    }

    private void MoveTowardsEndGate(GameObject _endGate)
    {
        var dir = _endGate.transform.position - transform.position;
        transform.Translate(dir.normalized * Time.deltaTime);

        CheckForProximityToEndGate(_endGate);
    }

    private void CheckForProximityToEndGate(GameObject _endGate)
    {
        if (Time.time > nextTimeToCheckProximityToEndGate)
        {
            var dist = Vector2.Distance(transform.position,
            _endGate.transform.position);

            print("Distance to my end gate is " + dist);

            if (dist < thresholdDistanceToEnterQueue)
            {
                EnterQueue(_endGate);
            }

            nextTimeToCheckProximityToEndGate = Time.time +
                frequencyToCheckProximityToEndGate;
        }
    }

    private void EnterQueue(GameObject _endGate)
    {
        print("Entering the queue of " + _endGate.name);

        travellerState = TravellerState.Queuing;

        var gateController = endGate.GetComponent<GateController>();
        gateController.outboundTravellerQueue.Add(this.gameObject);

        var queueStart = endGate.transform.Find("QueueStart").position;
        var xPos = queueStart.x + (queueDistanceFromNextTraveller *
            gateController.outboundTravellerQueue.Count);

        transform.position = new Vector2(xPos, queueStart.y);

    }
}
