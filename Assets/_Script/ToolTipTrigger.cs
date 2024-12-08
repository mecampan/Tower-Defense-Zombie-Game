using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea] public string tooltipMessage; // Text specific to this button

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventManager.ShowTooltip(tooltipMessage); // Trigger showing the tooltip
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventManager.HideTooltip(); // Trigger hiding the tooltip
    }
}
