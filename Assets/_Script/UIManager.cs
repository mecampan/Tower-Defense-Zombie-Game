using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turretCounterNum, furnitureCounterNum;
    [SerializeField] private Canvas UI;
    [SerializeField] private GameObject textPrefab;     
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private GameObject toolTip;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private RectTransform canvasRectTransform;

    private float displayDuration = 2f;

    private GameObject currentWarningObject; // To track the created text object
    private GameObject currentPopupObject; // To track the created popup menu

    void OnEnable()
    {
        EventManager.OnWarning += CreateAndShowText;
        EventManager.OnTooltip += HandleTooltip;
    }

    void OnDisable()
    {
        EventManager.OnWarning -= CreateAndShowText;
        EventManager.OnTooltip -= HandleTooltip;
    }

    private void Start()
    {
        toolTip.SetActive(false);
    }

    private void Update()
    {
        if (toolTip.activeSelf)
        {
            FollowMouse();
        }
    }

    public void UpdateTurretCounter(int count)
    {
        turretCounterNum.text = $"{count}";
    }

    public void UpdateFurnitureCounter(int count)
    {
        furnitureCounterNum.text = $"{count}";
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
        if (currentPopupObject != null) return;

        currentPopupObject = Instantiate(popupPrefab, UI.transform);

        Button yesButton = currentPopupObject.transform.Find("YesButton").GetComponent<Button>();
        yesButton.onClick.AddListener(() =>
        {
            onYes?.Invoke();
            DestroyPopup();
        });

        Button noButton = currentPopupObject.transform.Find("NoButton").GetComponent<Button>();
        noButton.onClick.AddListener(() =>
        {
            onNo?.Invoke();
            DestroyPopup();
        });
    }

    private void DestroyPopup()
    {
        if (currentPopupObject != null)
        {
            Destroy(currentPopupObject);
            currentPopupObject = null;
        }
    }

    private void HandleTooltip(string message, bool isVisible)
    {
        toolTip.SetActive(isVisible); // Show or hide the tooltip
        if (isVisible)
        {
            tooltipText.text = message; // Update tooltip text
        }
    }


    private void FollowMouse()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            Input.mousePosition,
            null, // Camera is null for Screen Space - Overlay
            out localPoint);

        // Update tooltip position with an offset
        RectTransform toolTipRect = toolTip.GetComponent<RectTransform>();
        toolTipRect.anchoredPosition = localPoint + new Vector2(-100f, 100f);
    }
}
