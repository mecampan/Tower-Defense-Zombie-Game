using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shopper : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed of movement
    public Grid shopperGrid; // Reference to the Grid object
    private Vector3Int currentGridPos; // Current position on the grid
    private Queue<Vector3Int> pathQueue = new Queue<Vector3Int>(); // Queue to hold the path

    public void Initialize(Vector3Int startPosition, Grid grid)
    {
        currentGridPos = startPosition;
        shopperGrid = grid;

        // Set the shopper's world position to the starting grid position
        transform.position = shopperGrid.CellToWorld(currentGridPos);

        // Start moving the shopper
        StartCoroutine(MoveOnGrid());
    }

    private IEnumerator MoveOnGrid()
    {
        while (true)
        {
            // If no path, generate a new random destination
            if (pathQueue.Count == 0)
            {
                // Snap to grid to ensure accuracy
                SnapToGrid();

                // Set a new random destination
                Vector3Int randomDestination = GetRandomGridPosition();
                SetPathTo(randomDestination);
            }

            // If we have a path, move to the next position
            if (pathQueue.Count > 0)
            {
                Vector3Int nextGridPos = pathQueue.Dequeue();
                Vector3 targetPosition = shopperGrid.CellToWorld(nextGridPos);

                // Move to the next grid position
                yield return StartCoroutine(MoveToPosition(targetPosition));

                // Update the current grid position
                currentGridPos = nextGridPos;
            }

            // Wait briefly before recalculating
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Snap to the exact target position
        transform.position = targetPosition;
    }

    private void SetPathTo(Vector3Int destination)
    {
        // Use the navigation system to calculate the shortest path
        //GridData dummyGridData = new GridData(); // Replace with actual grid data if available
        //List<Vector3Int> calculatedPath = Navagation.FindShortestPath(ref dummyGridData, destination, currentGridPos, shopperGrid);

        // Convert the calculated path to a queue
        //pathQueue = new Queue<Vector3Int>(calculatedPath);

        //Debug.Log($"Path set for shopper to {destination}: {string.Join(" -> ", calculatedPath)}");
    }

    private Vector3Int GetRandomGridPosition()
    {
        // Generate a random position within the grid bounds
        int randomX = Random.Range(-5, 5); // Replace with grid size
        int randomZ = Random.Range(-5, 5); // Replace with grid size
        return new Vector3Int(randomX, 0, randomZ);
    }

    private void SnapToGrid()
    {
        // Snap the shopper to its nearest grid position
        currentGridPos = shopperGrid.WorldToCell(transform.position);
        transform.position = shopperGrid.CellToWorld(currentGridPos);
    }
}
