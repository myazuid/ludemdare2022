using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
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
    private int _currentBalance;
    
    
    private List<GameObject> _travellers;
    
    //temp UI stuff
    [Header("TEMP UI STUFF")]
    [SerializeField] private TextMeshProUGUI _currentBalanceText;
    [SerializeField] private TextMeshProUGUI _currentApprovalRatingText;

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
        
        //temp UI stuff
        _currentBalanceText.text = "$" + _currentBalance;
        _currentApprovalRatingText.text = "Approval Rating: " + _approvalRating;
    }

    private void Update()
    {
        _timeSinceLastSpawn += Time.deltaTime;

        if (_timeSinceLastSpawn >= _travellerSpawnRateInSeconds)
        {
            SpawnTraveller();
            _timeSinceLastSpawn = 0f;
        }
    }

    private void UpdateSpawnRate()
    {
        _travellerSpawnRateInSeconds -= 0.01f;
        _travellerSpawnRateInSeconds = Mathf.Round(_travellerSpawnRateInSeconds * 100f) / 100f;
    }

    private void SpawnTraveller()
    {
        int startGate = Random.Range(0, gatesParent.transform.childCount);
        int endGate;
        do
        {
            endGate = Random.Range(0, gatesParent.transform.childCount);
        } while (endGate == startGate);

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
        UpdateUI();
    }

    public void SpendFromBalance(int purchaseCost)
    {
        if (_currentBalance - purchaseCost > 0)
        {
            _currentBalance -= purchaseCost;
        }
    }
    
    private void UpdateUI()
    {
        //Temp UI stuff
        _currentBalanceText.text = "$" + _currentBalance;
        _currentApprovalRatingText.text = "Approval Rating: " + _approvalRating;
        
    }

    private void FinishTravellerProcessing(GameObject travellerObject, bool transitedSuccessfully)
    {
        RemoveTravellerFromTotalCount(travellerObject);
        if (transitedSuccessfully)
        {
            AddToBalance();
            RaiseApprovalRating(false);
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
        UpdateUI();
    }

    private void LowerApprovalRating(bool bigApprovalRatingLoss)
    {
        if (bigApprovalRatingLoss)
        {
            _approvalRating -= _approvalRatingChangeBig;
        }
        else _approvalRating -= _approvalRatingChangeSmall;
        
        _approvalRating = Mathf.Round(_approvalRating * 10f) / 10f;
        UpdateUI();
        
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
