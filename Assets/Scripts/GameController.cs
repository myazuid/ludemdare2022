using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using UnityEngine.U2D;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [SerializeField]
    private GameObject travellerPrefab;
    [SerializeField] 
    private GameObject gatesParent;

    public GameObject deactivatedGatePrefab;
    [SerializeField] 
    private int fareCost;

    public GameObject beamTravellerSpawn;
    public List<PathLevel> pathLevels = new List<PathLevel>();
    public List<GateLevel> gateLevels = new List<GateLevel>();

    private float _travellerSpawnRateInSeconds;
    private float _increaseDifficultyFrequencyInSeconds;
    private float _timeSinceLastSpawn;
    public float _approvalRating;
    private float _approvalRatingChangeSmall, _approvalRatingChangeBig, _approvalRatingRageExit;
    [NonSerialized]
    public int _currentBalance;

    public int _totalProcessed;

    private float surgeDelay = 20f;
    private int surgeCount;
    
    
    private List<GameObject> _travellers;

    private float timeTillNextGate = 30f;

    public static Action<int> onBalanceChanged;
    public static Action<float> onApprovalChanged;
    public static Action<int> onTotalProcessedChanged;
    
    private void Awake()
    {
        Time.timeScale = 1f;

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        for (int i = 0; i < gatesParent.transform.childCount; i++)
        {
            if (!gatesParent.transform.GetChild(i).gameObject.activeSelf)
            {
                GameObject go = Instantiate(deactivatedGatePrefab, gatesParent.transform.GetChild(i).position, Quaternion.identity);
                gatesParent.transform.GetChild(i).GetComponent<GateController>().deactivatedGate = go;
            }

        }
        
        _approvalRating = 6f;
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
        _approvalRatingChangeSmall = 0.1f;  // used to be 0.1f
        _approvalRatingRageExit = 1f;
        _approvalRatingChangeBig = 0.5f;
        _travellerSpawnRateInSeconds = 2f;
        _increaseDifficultyFrequencyInSeconds = 15f;

        InvokeRepeating(nameof(UpdateSpawnRate), _increaseDifficultyFrequencyInSeconds, _increaseDifficultyFrequencyInSeconds);
    }

    private void Update()
    {
        _timeSinceLastSpawn += Time.deltaTime;
        timeTillNextGate -= Time.deltaTime;

        if (timeTillNextGate <= 0)
        {
            timeTillNextGate = 45;
            for (int i = 0; i < gatesParent.transform.childCount; i++)
            {
                if (!gatesParent.transform.GetChild(i).gameObject.activeSelf)
                {
                    gatesParent.transform.GetChild(i).gameObject.SetActive(true);
                    gatesParent.transform.GetChild(i).gameObject.GetComponent<GateController>().deactivatedGate.SetActive(false);
                    break;
                }
            }
        }

        if (_timeSinceLastSpawn >= _travellerSpawnRateInSeconds)
        {
            SpawnTraveller();
            _timeSinceLastSpawn = Random.Range(0, -_travellerSpawnRateInSeconds/2f);
        }

        surgeDelay -= Time.deltaTime;
        if (surgeDelay <= 0)
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


            int spawnAmount = Random.Range(3 + surgeCount / 2, 3 + surgeCount);
            spawnAmount = Mathf.Min(spawnAmount, 20);
            StartCoroutine(SurgeTraveller(startGate, endGate, spawnAmount));
            surgeCount+=2;
            surgeDelay = Random.Range(15f, 30f);
        }
    }
    
    IEnumerator SurgeTraveller(int start, int end, int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnTraveller(start, end);
            yield return new WaitForSeconds(.3f);
        }
    }
    

    private void UpdateSpawnRate()
    {
        _travellerSpawnRateInSeconds *= .75f;
        _travellerSpawnRateInSeconds = Mathf.Round(_travellerSpawnRateInSeconds * 100f) / 100f;
        _travellerSpawnRateInSeconds = Mathf.Clamp(_travellerSpawnRateInSeconds, .025f, 10f);
;    }

    private void SpawnTraveller(int startGateA = -1, int startGateB = -1)
    {
        int startGate = startGateA;
        if (startGateA == -1)
        {
            do
            {
                startGate = Random.Range(0, gatesParent.transform.childCount);
            } while (!gatesParent.transform.GetChild(startGate).gameObject.activeSelf);
        }

        int endGate = startGateB;
        if (startGateB == -1)
        {
            do
            {
                endGate = Random.Range(0, gatesParent.transform.childCount);
            } while (endGate == startGate || !gatesParent.transform.GetChild(endGate).gameObject.activeSelf);
        }

        var traveller = Instantiate(travellerPrefab, gatesParent.transform.GetChild(startGate).position, quaternion.identity);
        
        traveller.GetComponent<TravellerController>().startGate =
            gatesParent.transform.GetChild(startGate).gameObject; 
        traveller.GetComponent<TravellerController>().endGate =
            gatesParent.transform.GetChild(endGate).gameObject;

        Instantiate(beamTravellerSpawn, traveller.transform.position, Quaternion.identity);

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
            onBalanceChanged.Invoke(_currentBalance);

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
        onApprovalChanged?.Invoke(_approvalRating);
    }

    private void LowerApprovalRating(bool bigApprovalRatingLoss)
    {
        if (bigApprovalRatingLoss)
        {
            _approvalRating -= _approvalRatingChangeBig * 2;
        }
        else _approvalRating -= _approvalRatingRageExit;
        
        _approvalRating = Mathf.Round(_approvalRating * 10f) / 10f;
        onApprovalChanged?.Invoke(_approvalRating);
        
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
        UIController.instance.showDefeatScreen();
        Time.timeScale = 0;
    }
}

[Serializable]
public class PathLevel
{
    public SpriteShape sprite;
    public int upgradeCost;
}

[Serializable]
public class GateLevel
{
    public Sprite sprite;
    public int upgradeCost;
}