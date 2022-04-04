using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [SerializeField]
    private GameObject travellerPrefab;
    [SerializeField] 
    private GameObject gatesParent;
    [SerializeField] 
    private int fareCost;
    [SerializeField]
    public List<int> pathUpgradeCosts = new List<int>();
    [SerializeField]
    public List<int> gateUpgradeCosts = new List<int>();


    private float _travellerSpawnRateInSeconds;
    private float _increaseDifficultyFrequencyInSeconds;
    private float _timeSinceLastSpawn;
    private float _approvalRating;
    private float _approvalRatingChangeSmall, _approvalRatingChangeBig;
    [NonSerialized]
    public int _currentBalance;

    public int _totalProcessed;
    
    
    private List<GameObject> _travellers;

    private float timeTillNextGate = 30f;

    public static Action<int> onBalanceChanged;
    public static Action<float> onApprovalChanged;
    public static Action<int> onTotalProcessedChanged;
    
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

    private void OnEnable()
    {
        GateController.OnTravellerProcessed += FinishTravellerProcessing;
    }

    private void OnDisable()
    {
        GateController.OnTravellerProcessed -= FinishTravellerProcessing;
    }

    private void Start()
    {
        _travellers = new List<GameObject>();
        _approvalRating = 10f;
        _approvalRatingChangeSmall = 0.1f; 
        _approvalRatingChangeBig = 0.5f;
        _travellerSpawnRateInSeconds = 0.5f;
        _increaseDifficultyFrequencyInSeconds = 30f;

        InvokeRepeating(nameof(UpdateSpawnRate), _increaseDifficultyFrequencyInSeconds, _increaseDifficultyFrequencyInSeconds);
    }

    private void Update()
    {
        _timeSinceLastSpawn += Time.deltaTime;
        timeTillNextGate -= Time.deltaTime;

        if (timeTillNextGate <= 0)
        {
            timeTillNextGate = 30;
            for (int i = 0; i < gatesParent.transform.childCount; i++)
            {
                if (!gatesParent.transform.GetChild(i).gameObject.activeSelf)
                {
                    gatesParent.transform.GetChild(i).gameObject.SetActive(true);
                    break;
                }
            }
        }

        if (_timeSinceLastSpawn >= _travellerSpawnRateInSeconds)
        {
            SpawnTraveller();
            _timeSinceLastSpawn = 0f;
        }
    }

    private void UpdateSpawnRate()
    {
        _travellerSpawnRateInSeconds *= .75f;
        _travellerSpawnRateInSeconds = Mathf.Round(_travellerSpawnRateInSeconds * 100f) / 100f;
        _travellerSpawnRateInSeconds = Mathf.Clamp(_travellerSpawnRateInSeconds, .025f, 10f);
;    }

    private void SpawnTraveller()
    {
        int startGate;
        do
        {
            startGate = Random.Range(0, gatesParent.transform.childCount);
        } while (!gatesParent.transform.GetChild(startGate).gameObject.activeSelf);

        int endGate;
        do
        {
            endGate = Random.Range(0, gatesParent.transform.childCount);
        } while (endGate == startGate || !gatesParent.transform.GetChild(endGate).gameObject.activeSelf);

        var traveller = Instantiate(travellerPrefab, gatesParent.transform.GetChild(startGate).position, quaternion.identity);
        
        traveller.GetComponent<TravellerController>().startGate =
            gatesParent.transform.GetChild(startGate).gameObject; 
        traveller.GetComponent<TravellerController>().endGate =
            gatesParent.transform.GetChild(endGate).gameObject;

        AddTravellerToTotalCount(traveller);
    }

    public void AddToBalance()
    {
        _currentBalance += fareCost;
        onBalanceChanged.Invoke(_currentBalance);
    }

    public bool SpendFromBalance(int purchaseCost)
    {
        if (_currentBalance - purchaseCost >= 0)
        {
            _currentBalance -= purchaseCost;
            onBalanceChanged.Invoke(purchaseCost);

            return true;
        }
        else
        {
            return false;
        }
    }

    private void FinishTravellerProcessing(GameObject travellerObject, bool transitedSuccessfully)
    {
        RemoveTravellerFromTotalCount(travellerObject);
        if (transitedSuccessfully)
        {
            AddToBalance();
            RaiseApprovalRating(false);
            _totalProcessed++;
            onTotalProcessedChanged.Invoke(_totalProcessed);
        }
        else LowerApprovalRating(false);
    }

    
    private void RaiseApprovalRating(bool bigApprovalRatingGain)
    {
        if (bigApprovalRatingGain)
        {
            _approvalRating += _approvalRatingChangeBig;
        }
        else _approvalRating += _approvalRatingChangeSmall;

        if (_approvalRating >= 10f)
        {
            _approvalRating = 10f;
        }
        
        _approvalRating = Mathf.Round(_approvalRating * 10f) / 10f;
        onApprovalChanged.Invoke(_approvalRating);
    }

    private void LowerApprovalRating(bool bigApprovalRatingLoss)
    {
        if (bigApprovalRatingLoss)
        {
            _approvalRating -= _approvalRatingChangeBig;
        }
        else _approvalRating -= _approvalRatingChangeSmall;
        
        _approvalRating = Mathf.Round(_approvalRating * 10f) / 10f;
        onApprovalChanged.Invoke(_approvalRating);
        
        if (_approvalRating <= 0f)
        {
            EndGame();
        }
    }

    //Add to total active travellers, if needed for UI.
    private void AddTravellerToTotalCount(GameObject traveller)
    {
        _travellers.Add(traveller);
    }

    private void RemoveTravellerFromTotalCount(GameObject travellerObject)
    {
        if (_travellers.Contains(travellerObject))
        {
            _travellers.Remove(travellerObject);
        }
        else
        {
            Debug.LogWarning("Traveller exists outside of global List of travellers");
        }
    }

    private void EndGame()
    {
        Debug.LogWarning("You lose. Good day sir.");
    }
}
