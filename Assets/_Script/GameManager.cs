using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject panel; // Parent object containing the red-boxed UI elements
    [SerializeField] private GameObject startPlayModeButton; // The "Start Play Mode" button
    [SerializeField] private PlacementSystem placementSystem;
    [SerializeField] private CustomerManager customerManager;
    [SerializeField] private ZombieManager zombieManager;
    [SerializeField] private UIManager uIManager;

    public void StartPlayMode()
    {
        uIManager.ShowDialog(
            () =>
            {
                Debug.Log("Yes clicked: Transitioning to Play Mode");

                // Ensure popup is destroyed before hiding other UI
                if (panel != null)
                {
                    Debug.Log("Hiding panel...");
                    panel.SetActive(false);
                }

                if (startPlayModeButton != null)
                {
                    Debug.Log("Hiding StartPlayMode button...");
                    startPlayModeButton.SetActive(false);
                }

                placementSystem.StopPlacement();
                BeginPlayMode();
            },
            () =>
            {
                Debug.Log("No clicked: Canceled Play Mode");
            }
        );
    }

    private void BeginPlayMode()
    {
        // Start gameplay (e.g., spawn enemies, start timers, etc.)
        Debug.Log("Play Mode started! Gameplay logic goes here.");

        customerManager.BeginCustomerSpawner();
        zombieManager.BeginZombieSpawner();
    }
}
