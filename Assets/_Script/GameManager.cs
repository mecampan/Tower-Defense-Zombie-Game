using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject panel; // Parent object containing the red-boxed UI elements
    [SerializeField] private GameObject startPlayModeButton; // The "Start Play Mode" button
    [SerializeField] private PlacementSystem placementSystem;
    [SerializeField] private CustomerManager customerManager;

    public void StartPlayMode()
    {
        // Hide all red-boxed UI elements
        if (panel != null)
            panel.SetActive(false);

        // Hide the "Start Play Mode" button
        if (startPlayModeButton != null)
            startPlayModeButton.SetActive(false);

        Debug.Log("Transitioning to Play Mode. UI elements have been hidden.");
        placementSystem.StopPlacement();

        BeginPlayMode();
    }

    private void BeginPlayMode()
    {
        // Start gameplay (e.g., spawn enemies, start timers, etc.)
        Debug.Log("Play Mode started! Gameplay logic goes here.");

        customerManager.BeginCustomerSpawner();
    }
}
