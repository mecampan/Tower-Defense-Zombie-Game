using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ratingsystem : MonoBehaviour
{
    public int rating = 5;
    public int minRating = 0;

    public Image[] starImages;
    public Sprite filledStar;
    public Sprite emptyStar;

    // Start is called before the first frame update
    void Start()
    {
        UpdateRatingUI();
    }

    // This method updates the UI based on the current rating
    [ContextMenu("Update Rating UI")]
    void UpdateRatingUI()
    {
        for (int i = 0; i < starImages.Length; i++)
        {
            if (i < rating)
            {
                starImages[i].sprite = filledStar;  // Assign filled star sprite
                //Debug.Log($"Star {i}: Filled");
            }
            else
            {
                starImages[i].sprite = emptyStar;  // Assign empty star sprite
                //Debug.Log($"Star {i}: Empty");
            }
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
    void ChangeRating(int amount)
    {
        // Clamp rating between minRating (0) and max rating (5)
        rating = Mathf.Clamp(rating + amount, minRating, 5);
        UpdateRatingUI();  // Update the UI to reflect the new rating

        // Check if rating reaches zero (game over)
        if (rating == minRating)
        {
            GameOver();
        }
    }

    // Trigger the game over state
    void GameOver()
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
