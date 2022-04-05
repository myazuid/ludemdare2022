using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject[] reviewPanels;
    [SerializeField] private GameObject tutorialContainer;
    [SerializeField] private Sprite[] bossSpriteSources;
    [SerializeField] private Image tutorialBossSprite;
    [SerializeField] private Text tutorialTitleText, tutorialBodyText;
    [SerializeField] private float tutorialNotificationDurationInSeconds = 8f;
    [SerializeField] private float timeSpentInTutorial;

    private bool _gameStartTutorial, _firstTravellerSpawnTutorial, _negativeReviewTutorial, _firstAngryAlienTutorial;
    public bool _firstGateArrivalTutorial, _firstUpgradeTutorial, _firstPathTutorial, _upgradedGatesOnce;

    public Text balanceChanged;
    public Text totalProcessed;
    public Image bar;
    private RectTransform _rectTransform;

    public Color goodColor;
    public Color badColor;

    public Image satisfactionImage;
    public Sprite iconSad, iconNeutral, iconHappy;

    public GameObject tooltipContainer;
    public Text textTitle, textDescription;
    public GameObject defeatContainer;
    
    public static UIController instance;
    private float startXSize;
    

    private void Awake()
    {
        instance = this;
        _rectTransform = bar.GetComponent<RectTransform>();
        startXSize = _rectTransform.sizeDelta.x;
    }

    public void Start()
    {
        if (GameController.instance == null)
        {
            return;
        }

        GameController.onBalanceChanged += OnBalanceChanged;
        GameController.onApprovalChanged += ONApprovalChanged;
        GameController.onTotalProcessedChanged += ONTotalProcessedChanged;
        ReviewsController.OnReviewPosted += OnReviewPosted;
        ONApprovalChanged(GameController.instance._approvalRating);
        hideTooltip();

        ShowTutorial(this);
    }

    private void ShowTutorial(UIController uiController)
    {
        if (!_gameStartTutorial)
        {
            tutorialContainer.SetActive(true);
            tutorialBossSprite.sprite = bossSpriteSources[0];
            tutorialTitleText.text = "Welcome!";
            tutorialBodyText.text = "Welcome to the Stonehenge galactic transport hub team, new hire." +
                                    "\n\nIt’s a pretty simple job really. " +
                                    "I don’t know why the previous hires couldn’t handle it! " +
                                    "We are sure you will do great. If not, we will know.";
            ChangeTimescale();
            _gameStartTutorial = true;
        }
        else
        {
            tutorialContainer.SetActive(true);
            tutorialBossSprite.sprite = bossSpriteSources[0];
            tutorialTitleText.text = "Your first traveller.";
            tutorialBodyText.text = "Here is your first passenger! Isn’t he cute? He looks very happy too! " +
                                    "Make sure to keep him that way!! Let’s see where he plans to go…";
            ChangeTimescale();
        }
    }
    
    public void ShowAngryTutorial(GameController gameController)
    {
        tutorialContainer.SetActive(true);
        tutorialBossSprite.sprite = bossSpriteSources[2];
        tutorialTitleText.text = "Angry Customer!";
        tutorialBodyText.text = "That poor passenger has been waiting too long and is now angry with you! " +
                                "Frankly, i feel the same way. " +
                                "Send them on their way before they leave " +
                                "and write a negative review against our company.";
        ChangeTimescale();
    }
    
    public void ShowTutorial(GameController gameController)
    {
        tutorialContainer.SetActive(true);
        tutorialBossSprite.sprite = bossSpriteSources[2];
        tutorialTitleText.text = "ONE JOB!";
        tutorialBodyText.text = "I gave you one job to do and you failed. " +
                                "An angry passenger just left a negative review " +
                                "and forever tarnished the great name of this company. " +
                                "If your approval rating drops below 0 you will be fired!";
        ChangeTimescale();
    }
    
    public void ShowTutorial(GateController gateController)
    {
        tutorialContainer.SetActive(true);
        tutorialBossSprite.sprite = bossSpriteSources[1];
        tutorialTitleText.text = "Send The Passenger On His Way.";
        tutorialBodyText.text = "Looks like he is queued up at the gate. " +
                                "He needs your help getting through the security check. " +
                                "Click on the button above the gate to send him through. " +
                                "And be quick about it, he has an important job to do " +
                                "and he is waiting on you to DO.. YOUR.. JOB!!";
        ChangeTimescale();
    }
    
    public void ShowGateUpgradeTutorial(GateController gateController)
    {
        tutorialContainer.SetActive(true);
        tutorialBossSprite.sprite = bossSpriteSources[1];
        tutorialTitleText.text = "Automatically Processing Travellers.";
        tutorialBodyText.text = "If you had read the manual it clearly specifies " +
                                "that you can upgrade gates to automatically send passengers through. " +
                                "Try it now.";
        ChangeTimescale();
    }
    
    public void ShowTutorial(PathController pathController)
    {
        tutorialContainer.SetActive(true);
        tutorialBossSprite.sprite = bossSpriteSources[2];
        tutorialTitleText.text = "Move Faster!";
        tutorialBodyText.text = "Have you not noticed how slowly these passengers are moving? " +
                                "It’s because you haven’t created any paths for them! What are you thinking? " +
                                "You should really be building and upgrading paths so that they can move faster. " +
                                "Do your job so they can get to theirs.";
        ChangeTimescale();
    }

    private void ONTotalProcessedChanged(int obj)
    {
        totalProcessed.text = obj.ToString();
    }

    private void ONApprovalChanged(float obj)
    {
        float percentageRating = obj / 10f;
        float scaledRating = Mathf.Lerp(0, startXSize, obj / 10f);
        _rectTransform.sizeDelta = new Vector2(scaledRating, 24);
        bar.color = Color.Lerp(badColor, goodColor, percentageRating);

        if (percentageRating < .3f)
        {
            satisfactionImage.sprite = iconSad;
        }
        else if (percentageRating < .6f)
        {
            satisfactionImage.sprite = iconNeutral;
        }
        else
        {
            satisfactionImage.sprite = iconHappy;
        }
    }

    private void OnBalanceChanged(int amount)
    {
        balanceChanged.text = amount.ToString();
    }

    private void OnReviewPosted(int type)
    {
        //0 positive, 1 neutral, 2 negative.
        switch (type)
        {
            case 0:
                reviewPanels[type].transform.GetChild(0).GetComponent<Text>().text = ReviewsController.instance.reviews
                    .positive[Random.Range(0, ReviewsController.instance.reviews.positive.Length)].name;

                reviewPanels[type].transform.GetChild(1).GetComponent<Text>().text = ReviewsController.instance.reviews
                    .positive[Random.Range(0, ReviewsController.instance.reviews.positive.Length)].username;

                reviewPanels[type].transform.GetChild(2).GetComponent<Text>().text = ReviewsController.instance.reviews
                    .positive[Random.Range(0, ReviewsController.instance.reviews.positive.Length)].message;
                StartCoroutine(ShowReview(reviewPanels[type]));
                break;

            case 1:
                reviewPanels[type].transform.GetChild(0).GetComponent<Text>().text = ReviewsController.instance.reviews
                    .neutral[Random.Range(0, ReviewsController.instance.reviews.neutral.Length)].name;

                reviewPanels[type].transform.GetChild(1).GetComponent<Text>().text = ReviewsController.instance.reviews
                    .neutral[Random.Range(0, ReviewsController.instance.reviews.neutral.Length)].username;

                reviewPanels[type].transform.GetChild(2).GetComponent<Text>().text = ReviewsController.instance.reviews
                    .neutral[Random.Range(0, ReviewsController.instance.reviews.neutral.Length)].message;
                StartCoroutine(ShowReview(reviewPanels[type]));
                break;

            case 2:
                reviewPanels[type].transform.GetChild(0).GetComponent<Text>().text = ReviewsController.instance.reviews
                    .negative[Random.Range(0, ReviewsController.instance.reviews.negative.Length)].name;

                reviewPanels[type].transform.GetChild(1).GetComponent<Text>().text = ReviewsController.instance.reviews
                    .negative[Random.Range(0, ReviewsController.instance.reviews.negative.Length)].username;

                reviewPanels[type].transform.GetChild(2).GetComponent<Text>().text = ReviewsController.instance.reviews
                    .negative[Random.Range(0, ReviewsController.instance.reviews.negative.Length)].message;
                StartCoroutine(ShowReview(reviewPanels[type]));
                break;
        }
    }

    public void showTooltip(GateController gateController) // for upgrades
    {
        tooltipContainer.SetActive(true);
        textTitle.text = "Gateway";
        if (gateController.gateLevel == gateController.MaxGateLevel)
        {
            textDescription.text = "Level " + gateController.gateLevel + " / " + (GameController.instance.gateLevels.Count - 1) + "\n" +
                               "Automatically processes " + gateController.gateLevel + " travellers per second\n" +
                               "\n" +
                               "Max Level.";
        }
        else
        {
            textDescription.text = "Level " + gateController.gateLevel + " / " + (GameController.instance.gateLevels.Count - 1) + "\n" +
                   "Upgrade Cost: " + GameController.instance.gateLevels[gateController.gateLevel].upgradeCost + "\n" +
                   "Automatically processes " + gateController.gateLevel + " travellers per second\n" +
                    "Click to upgrade and increase traveller processing.";
        }
    }
    
    public void showTooltip(PathController pathController)
    {
        tooltipContainer.SetActive(true);
        textTitle.text = "Path";
        if (pathController.pathLevel == pathController.MaxPathLevel)
        {
            textDescription.text = "Level " + pathController.pathLevel + " / " + (GameController.instance.pathLevels.Count - 1) + "\n" +
                "\n" +
                "Max Level.";
        }
        else
        {
            textDescription.text = "Level " + pathController.pathLevel + " / " + (GameController.instance.pathLevels.Count - 1) + "\n" +
                               "Upgrade Cost: " + GameController.instance.pathLevels[pathController.pathLevel].upgradeCost + "\n" +
                               "Click to upgrade and increase speed of travellers";
        }
        
    }

    public void showTooltip(GateProcessOutboundButton gateProcessOutboundButton)
    {
        tooltipContainer.SetActive(true);
        textTitle.text = "Gateway";
        textDescription.text = "Click to send travellers through";
    }

    public void hideTooltip()
    {
        tooltipContainer.SetActive(false);
    }

    public void HideTutorial()
    {
        tutorialContainer.SetActive(false);
    }

    public void showDefeatScreen()
    {
        if (defeatContainer != null)
        {
            defeatContainer.SetActive(true);
        }
    }

    IEnumerator ShowReview(GameObject reviewPanelToShow)
    {
        reviewPanelToShow.SetActive(true);
        yield return new WaitForSeconds(ReviewsController.instance.reviewVisibleDuration);
        reviewPanelToShow.SetActive(false);
    }

    private void ChangeTimescale()
    {
        Time.timeScale = Time.timeScale == 0 ? 1f : 0f;

        if (_gameStartTutorial && !_firstTravellerSpawnTutorial)
        {
            _firstTravellerSpawnTutorial = true;
            StartCoroutine(FirstTravellerSpawned());
        }
    }

    IEnumerator FirstTravellerSpawned()
    {
        yield return new WaitForSeconds(2.5f);
        ShowTutorial(this);
    }

    public void loadMenu()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (Time.timeScale == 0)
        {
            timeSpentInTutorial += Time.unscaledDeltaTime;
        }

        if (timeSpentInTutorial >= tutorialNotificationDurationInSeconds)
        {
            HideTutorial();
            ChangeTimescale();
            timeSpentInTutorial = 0f;
        }
    }
}