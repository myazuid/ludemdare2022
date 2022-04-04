using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject[] reviewPanels;

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

    private void Awake()
    {
        instance = this;
        _rectTransform = bar.GetComponent<RectTransform>();
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

        hideTooltip();
    }

    private void ONTotalProcessedChanged(int obj)
    {
        totalProcessed.text = obj.ToString();
    }

    private void ONApprovalChanged(float obj)
    {
        float percentageRating = obj / 10f;
        float scaledRating = Mathf.Lerp(0, 148, obj / 10f);
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
}