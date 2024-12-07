using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turretCounterNum, furnitureCounterNum;
    [SerializeField] private Canvas UI; // Reference to your Canvas
    [SerializeField] private GameObject textPrefab; // Prefab for dynamically creating the warning text
    [SerializeField] private GameObject popupPrefab; // Prefab for the dialog box

    public float displayDuration = 1f;

    private GameObject currentWarningObject; // To track the created text object
    private GameObject currentPopupObject; // To track the created popup menu

    public void UpdateTurretCounter(int count)
    {
        turretCounterNum.text = $"{count}";
    }

    public void UpdateFurnitureCounter(int count)
    {
        furnitureCounterNum.text = $"{count}";
    }

    void OnEnable()
    {
        EventManager.OnWarning += CreateAndShowText;
    }

    void OnDisable()
    {
        EventManager.OnWarning -= CreateAndShowText;
    }

    private void CreateAndShowText(string message)
    {
        // If a warning text is already active, destroy it
        if (currentWarningObject != null)
        {
            Destroy(currentWarningObject);
        }

        // Create a new text object
        currentWarningObject = Instantiate(textPrefab, UI.transform);
        var textComponent = currentWarningObject.GetComponent<TextMeshProUGUI>();

        if (textComponent != null)
        {
            textComponent.text = message;
        }

        // Start coroutine to destroy the object after duration
        StartCoroutine(DestroyTextAfterDuration());
    }

    private IEnumerator DestroyTextAfterDuration()
    {
        yield return new WaitForSeconds(displayDuration);

        if (currentWarningObject != null)
        {
            Destroy(currentWarningObject);
        }
    }


    public void ShowDialog(System.Action onYes, System.Action onNo)
    {
        GameObject popUpInstance = Instantiate(popupPrefab, UI.transform);

        // Configure the Yes button
        Button yesButton = popUpInstance.transform.Find("YesButton").GetComponent<Button>();
        yesButton.onClick.AddListener(() =>
        {
            onYes?.Invoke(); // Execute Yes action
            Destroy(popUpInstance); // Close dialog
        });

        // Configure the No button
        Button noButton = popUpInstance.transform.Find("NoButton").GetComponent<Button>();
        noButton.onClick.AddListener(() =>
        {
            onNo?.Invoke(); // Execute No action
            Destroy(popUpInstance); // Close dialog
        });
    }
}
