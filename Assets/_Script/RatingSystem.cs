using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ratingsystem : MonoBehaviour
{
    public int rating = 5;
    public int minRating = 0;
    private int totalCustomers = 0;
    private int totalStars = 0;

    public Image[] starImages;
    public Sprite filledStar;
    public Sprite emptyStar;

    [SerializeField] private Image starPrefab;
    [SerializeField] private RectTransform UI;
    [SerializeField] private Vector2 startPosition;
    [SerializeField] private Vector2 targetPosition; 
    [SerializeField] private TextMeshProUGUI customerCount; 

    // Start is called before the first frame update
    void Start()
    {
        UpdateRatingUI();
        SetCustomerCountVisibility(false);
    }

    private void OnEnable()
    {
        EventManager.OnRating += ChangeRating;
    }

    private void OnDisable()
    {
        EventManager.OnRating -= ChangeRating;
    }

    // This method updates the UI based on the current rating
    [ContextMenu("Update Rating UI")]
    private void UpdateRatingUI()
    {
        for (int i = 0; i < starImages.Length; i++)
        {
            if (i < rating)
            {
                starImages[i].sprite = filledStar;
            }
            else
            {
                starImages[i].sprite = emptyStar;
            }
        }
    }

    public void SetCustomerCountVisibility(bool isVisible)
    {
        if (customerCount != null)
        {
            Color color = customerCount.color;
            color.a = isVisible ? 1f : 0f; // Set alpha to 1 for visible, 0 for invisible
            customerCount.color = color;
        }
    }

    // Call this method when a customer is damaged
    public void CustomerDamged()
    {
        ChangeRating(-1);  // Decrease rating by 1
    }

    // Call this method when a customer is killed
    public void CustomerKilled()
    {
        ChangeRating(-2);  // Decrease rating by 2
    }

    // Change rating based on the amount
    private void ChangeRating(int amount)
    {
        totalCustomers++;
        totalStars += amount;
        if(customerCount != null)
        {
            customerCount.text = $"Customers {totalCustomers}/ 30";
        }

        // Calculate the average rating (rounded up)
        float average = totalStars / (float)totalCustomers;
        rating = Mathf.CeilToInt(average);

        // Ensure rating stays between bounds
        rating = Mathf.Clamp(rating, minRating, starImages.Length);

        //Debug.Log($"Average Rating: {rating} (Total Stars: {totalStars}, Customers: {totalCustomers})");
        RatingAnimationEffect(amount);
        UpdateRatingUI();
    }

    private void RatingAnimationEffect(int stars)
    {
        List<RectTransform> starRects = new List<RectTransform>();

        for (int i = 0; i < stars; i++)
        {
            Vector2 spawnPosition = startPosition + new Vector2(50 * i, 0);

            Image newStar = Instantiate(starPrefab, UI);
            RectTransform starRect = newStar.GetComponent<RectTransform>();

            starRect.anchoredPosition = spawnPosition;

            starRects.Add(starRect);
        }

        StartCoroutine(MoveStarsSequentially(starRects));
    }

    private IEnumerator MoveStarsSequentially(List<RectTransform> starRects)
    {
        float initialDelay = 1f; // Time before any stars start moving
        float delayBetweenMoves = 0.2f; // Time between each star's movement

        // Wait for the initial delay
        yield return new WaitForSeconds(initialDelay);

        for (int i = 0; i < starRects.Count; i++)
        {
            StartCoroutine(MoveStarToTarget(starRects[i], starRects[i].anchoredPosition, targetPosition, 1f));

            yield return new WaitForSeconds(delayBetweenMoves);
        }
    }


    private IEnumerator MoveStarToTarget(RectTransform star, Vector2 startPosition, Vector2 targetPosition, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            star.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / duration);
            yield return null;
        }

        star.anchoredPosition = targetPosition;

        Destroy(star.gameObject);
    }

    // Trigger the game over state
    private void GameOver()
    {
        Debug.Log("Game Over! Rating has reached zero");
    }

    private void OnValidate()
    {
        if (Application.isPlaying && starImages != null && filledStar != null && emptyStar != null)
        {
            UpdateRatingUI();
        }
    }

}
