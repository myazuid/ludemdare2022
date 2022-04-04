using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TravellerController : MonoBehaviour
{
    public GameObject startGate, endGate;
    private GateController endGateController;
    Vector2 destination;

    // MOVEMENT
    [SerializeField] float baseTravellerSpeed;
    [SerializeField] float actualtravellerSpeed;
    [SerializeField] float thresholdDistanceToEnterQueue;
    private float frequencyToCheckProximityToEndGate = 1;
    private float nextTimeToCheckProximityToEndGate = 0;

    public enum TravellerState { Travelling, Queuing };
    private TravellerState travellerState = TravellerState.Travelling;

    // QUEUING STUFF
    float timeSpentQueuing = 0;
    float minQueuingPatienceDuration = 10, maxQueuingPatienceDuration = 30;
    float queuingPatienceDuration;
    public float queueDistanceFromNextTraveller = 0.3f;
    public Vector2 queuingPosition;

    public enum UnhappinessLevel { Happy, Unsatisfied, Angry };
    public UnhappinessLevel unhappinessLevel;

    // UPGRADE STUFF
    private PathController pathController;

    private CharacterVisuals charVisuals;

    private void Start()
    {
        queuingPatienceDuration = Random.Range(minQueuingPatienceDuration,
            maxQueuingPatienceDuration);

        destination = endGate.transform.Find("DestinationPoint").position;

        endGateController = endGate.GetComponent<GateController>();

        pathController = PathManager.instance.ReturnPathController(startGate, endGate);
        SetSpeedBasedOnPathLevel(pathController.pathLevel);

        charVisuals = transform.Find("Character").GetComponent<CharacterVisuals>();
    }

    private void OnEnable()
    {
        PathManager.OnPathUpgraded += CheckForPathUpdate;
    }

    private void OnDisable()
    {
        PathManager.OnPathUpgraded -= CheckForPathUpdate;
    }

    // Update is called once per frame
    void Update()
    {
        CheckTravellerQueuingPatience();

        if (travellerState == TravellerState.Travelling)
        {
            MoveTowardsDestination(destination);
        }
        else if (travellerState == TravellerState.Queuing)
        {
            MoveTowardsQueuePosition(queuingPosition);
            //CheckTravellerQueuingPatience();
        }
    }

    private void CheckForPathUpdate(PathController _pathController)
    {
        if (pathController == _pathController)
        {
            SetSpeedBasedOnPathLevel(pathController.pathLevel);
        }
    }

    private void SetSpeedBasedOnPathLevel(int _pathLevel)
    {
        var speedMultiplier = PathManager.
                instance.pathLevelSpeedMultiplier[_pathLevel];
        actualtravellerSpeed = baseTravellerSpeed * speedMultiplier;
    }

    private void ResetSpeedToDefault()
    {
        actualtravellerSpeed = baseTravellerSpeed;
    }

    private void MoveTowardsDestination(Vector2 _destination)
    {
        var dir = _destination - (Vector2)transform.position;
        transform.Translate(dir.normalized * actualtravellerSpeed * Time.deltaTime);

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
        endGateController.outboundTravellerQueue.Add(this.gameObject);

        queuingPosition = endGateController.FindNextAvailableQueuePosition();

        ResetSpeedToDefault();

        travellerState = TravellerState.Queuing;
    }

    private void MoveTowardsQueuePosition(Vector2 _destination)
    {
        var dir = _destination - (Vector2)transform.position;
        transform.Translate(dir.normalized * actualtravellerSpeed * Time.deltaTime);

        if (Time.time > nextTimeToCheckProximityToEndGate)
        {
            var dist = Vector2.Distance(transform.position, _destination);

            if (dist < 0.001f)
            {
                return;
            }
            else
            {
                nextTimeToCheckProximityToEndGate = Time.time +
                frequencyToCheckProximityToEndGate;
            }
        }
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
                charVisuals.setMoodlet(CharacterVisuals.MOODLET_STATE.SAD);
            }

        }
        else if (timeSpentQueuing < queuingPatienceDuration)
        {
            if (unhappinessLevel != UnhappinessLevel.Angry)
            {
                unhappinessLevel = UnhappinessLevel.Angry;
                charVisuals.setMoodlet(CharacterVisuals.MOODLET_STATE.ANGRY);
            }
        }
        else
        {
            if (travellerState == TravellerState.Queuing)
            {
                ExitQueueAndLeave(endGate);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void ExitQueueAndLeave(GameObject _endGate)
    {
        GateController.OnTravellerProcessed?.Invoke(gameObject, false);

        var gateController = endGate.GetComponent<GateController>();
        gateController.RemoveTravellerFromQueueAndShiftDownQueue(this.gameObject);

        Destroy(this.gameObject);
    }
}