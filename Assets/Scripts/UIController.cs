using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text balanceChanged;
    public Text totalProcessed;
    public Image bar;
    private RectTransform _rectTransform;

    public Color goodColor;
    public Color badColor;

    public Image satisfactionImage;
    public Sprite iconSad, iconNeutral, iconHappy;

    private void Awake()
    {
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
}