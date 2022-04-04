using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviewsController : MonoBehaviour
{
    public static ReviewsController instance;
    
    [Header("Data")]
    [SerializeField] public TextAsset positiveReviewData;
    [SerializeField] public TextAsset neutralReviewData;
    [SerializeField] public TextAsset negativeReviewData;

    [Header("Settings")] 
    [SerializeField] private float timeBeforeFirstReviewInSeconds = 30f;
    [SerializeField] private float reviewFrequencyInSeconds = 30f;
    public float reviewVisibleDuration = 10f;

    public static event Action<int> OnReviewPosted;
    
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


    private void Start()
    {
        reviews.positive = JsonUtility.FromJson<ReviewList>(positiveReviewData.text).positive;
        reviews.neutral = JsonUtility.FromJson<ReviewList>(neutralReviewData.text).neutral;
        reviews.negative = JsonUtility.FromJson<ReviewList>(negativeReviewData.text).negative;

        InvokeRepeating(nameof(ReviewPosted), timeBeforeFirstReviewInSeconds, reviewFrequencyInSeconds);
    }

    private void ReviewPosted()
    {
        int feelingBehindReview = 0;
        if (GameController.instance._approvalRating * 10 / 100 < 0.33f)
        {
            feelingBehindReview = 2;
        } else if (GameController.instance._approvalRating * 10 / 100 > 0.66f)
        {
            feelingBehindReview = 0;
        }
        else
        {
            feelingBehindReview = 1;
        }
        OnReviewPosted?.Invoke(feelingBehindReview);
    }


    [Serializable]
    public class Review
    {
        public string name, username, message;
    }
    public ReviewList reviews = new ReviewList();

    [Serializable]
    public class ReviewList
    {
        public Review[] positive;
        public Review[] neutral;
        public Review[] negative;
    }
    
}
