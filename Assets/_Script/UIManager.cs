using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI turretCounterText, furnitureCounterText;

    public void UpdateTurretCounter(int count)
    {
        turretCounterText.text = $"{count}";
    }

    public void UpdateFurnitureCounter(int count)
    {
        furnitureCounterText.text = $"{count}";
    }
}