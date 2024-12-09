using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

struct ShoppingItem
{
    public int id;
    public Sprite image;
    public ShoppingItem(int ID, Sprite IMAGE)
    {
        id = ID;
        image = IMAGE;
    }
}

struct ShoppingListItem
{
    public int id;
    public Vector3Int pos;
    public ShoppingListItem(int ID, Vector3Int POS)
    {
        id = ID;
        pos = POS;
    }
}

public class Customer : Entity
{
    private Image image;

    private List<ShoppingItem> ShoppingItems = new List<ShoppingItem>();

    [SerializeField]
    [Header("Customer Testing")]
    private List<ShoppingListItem> shoppingList = new List<ShoppingListItem>();

    [SerializeField]
    [Header("Customer Testing")]
    private Vector3Int exitPoint;
    private int currentTargetIndex = 0;
    private int health = 5;
    private bool canTakeDamage = true;

    
    public void Setup(Vector3Int spawnPosition, List<Sprite> Sprites, Vector3Int exit, float speed, float wait, ref Grid InputGrid, ref PlacementSystem inputPlacementSystem)
    {
        for (int i = 0; i < Sprites.Count; i++) {
            ShoppingItems.Add(new ShoppingItem(i + 1, Sprites[i]));
        }
        image = GetComponentInChildren<Image>();
        grid = InputGrid;
        placementSystem = inputPlacementSystem;
        pos = spawnPosition;


        GridData floorData = placementSystem.floorData;
        Dictionary<Vector3Int, PlacementData> placedObjects = floorData.GetAllPlacedObjects();


        Dictionary<int, List<ShoppingListItem>> storedTiles = new Dictionary<int, List<ShoppingListItem>>();
        foreach (var entry in placedObjects)
        {
            Vector3Int position = entry.Key;
            PlacementData data = entry.Value;

            HashSet<int> ids = data.IDs;

            foreach(int id in ids){
                if(storedTiles.ContainsKey(id)){
                    List<ShoppingListItem> outList;
                    storedTiles.TryGetValue(id, out outList);
                    if(outList != null){
                        outList.Add(new ShoppingListItem(id, position));
                    }
                    else{
                        outList = new List<ShoppingListItem>() { new ShoppingListItem(id, position) };
                    }
                }
                else {
                    storedTiles.Add(id, new List<ShoppingListItem>() { new ShoppingListItem(id, position ) });
                }
            }

            //string ids = string.Join(", ", data.IDs); // Convert IDs to a comma-separated string

            //Debug.Log($"Position: {position}, IDs: [{ids}], PlacedObjectIndex: {data.PlacedObjectIndex}");
        }

        List<int> itemList = new List<int>();

        int numItems = UnityEngine.Random.Range(1, 5);

        for(int i = 0; i < numItems; i++){
            itemList.Add(UnityEngine.Random.Range(1, 6));
        }

        shoppingList = new List<ShoppingListItem>();
    
        foreach(int item in itemList){
            List<ShoppingListItem> tmpTiles;
            storedTiles.TryGetValue(item, out tmpTiles);
            if (tmpTiles != null)
            {
                int tmpIndex = UnityEngine.Random.Range(0, tmpTiles.Count);
                shoppingList.Add(tmpTiles[tmpIndex]);
            }
        }

        //print("shoppingListLen: " + shoppingList.Count);

        // Generate a random shopping list from available shelves
        // int itemsToBuy = UnityEngine.Random.Range(1, shelves.Count + 1); // 1 to all shelves
        //shoppingList = new List<Vector3Int>(shelves);
        // for (int i = shelves.Count - 1; i >= itemsToBuy; i--)
        // {
        //     shoppingList.RemoveAt(UnityEngine.Random.Range(0, shoppingList.Count));
        // }


        exitPoint = exit;
        moveSpeed = speed;
        waitTime = wait;

        // Start moving toward the first shelf
        StartCoroutine(FadeIn());
        StartCoroutine(PerformShopping());
    }
    
    private IEnumerator PerformShopping()
    {
        //print("Customer Beginning to Shop");

        while (currentTargetIndex < shoppingList.Count)
        {
            if (path == null)
            {
                //print("Finding a path");
                ShoppingListItem targetShelf = shoppingList[currentTargetIndex];
                if (FindPath(targetShelf.pos));
                {
                    if (image != null && ShoppingItems != null && currentTargetIndex >= 0 && currentTargetIndex < shoppingList.Count && shoppingList[currentTargetIndex].id - 1 >= 0 && shoppingList[currentTargetIndex].id - 1 < ShoppingItems.Count && ShoppingItems[shoppingList[currentTargetIndex].id - 1].image != null)
                    {
                        //print("sprite changed to: " + (shoppingList[currentTargetIndex].id - 1) + ", " + ShoppingItems[shoppingList[currentTargetIndex].id - 1].image.ToString());
                        image.sprite = ShoppingItems[shoppingList[currentTargetIndex].id - 1].image;
                    }
                    else
                    {
                        print("Sprite not changed: " + (image != null) + ", " + (ShoppingItems != null) + ", " + (currentTargetIndex >= 0) + ", " + (currentTargetIndex < shoppingList.Count) + ", " + (shoppingList[currentTargetIndex].id >= 0) + ", " + (shoppingList[currentTargetIndex].id < ShoppingItems.Count) + ", " + (ShoppingItems[shoppingList[currentTargetIndex].id].image != null));
                    }
                    //print("Found new shelf: " + targetShelf.ToString());
                }

            }
            else
            {
                ShoppingListItem targetShelf = shoppingList[currentTargetIndex];
                NAVIGATIONSTATUS tmp = NavigatePath();
                if (tmp == NAVIGATIONSTATUS.ERROR || (tmp == NAVIGATIONSTATUS.FINISHED && getIntPos().Equals(targetShelf.pos)))
                {
                    //print("done");
                    if (getIntPos().Equals(targetShelf.pos))
                    {
                        //print("reached shelf: " + targetShelf.ToString());
                        currentTargetIndex++;
                    }
                    path = null;
                }
                else if ((tmp == NAVIGATIONSTATUS.ATTILE || tmp == NAVIGATIONSTATUS.FINISHED) && gridData != null)
                {
                    path = null;
                    if (getIntPos().Equals(targetShelf.pos))
                    {
                        //print("reached shelf: " + targetShelf.ToString());
                        currentTargetIndex++;
                    }
                    Zombie[] Zombies = Zombie.FindObjectsOfType<Zombie>();
                    float MinDist = float.MaxValue;
                    foreach (Zombie zombie in Zombies)
                    {
                        float tmpDist = Vector3Int.Distance(getIntPos(), zombie.getIntPos());
                        if (tmpDist < MinDist)
                        {
                            MinDist = tmpDist;
                        }
                    }
                    if(MinDist <= 2)
                    {
                        //print("running");
                        if (image != null && 6 < ShoppingItems.Count && ShoppingItems[6].image != null)
                        {
                            //print("sprite changed to: " + (6) + ", " + ShoppingItems[6].image.ToString());
                            image.sprite = ShoppingItems[6].image;
                        }
                        else
                        {
                            print("Sprite not changed: " + (image != null) + ", " + (6 < ShoppingItems.Count) + ", " + (ShoppingItems[6].image != null));
                        }
                        float maxDist = 0;
                        Vector3Int maxTile = getIntPos();
                        for (int x = -2; x <= 2; x++)
                        {
                            for (int z = -2; z <= 2; z++)
                            {
                                Vector3Int tmpTile = new Vector3Int(getIntPos().x + x, 0, getIntPos().z + z);
                                foreach (Zombie zombie in Zombies)
                                {
                                    float tmpDist = Vector3Int.Distance(tmpTile, zombie.getIntPos());
                                    if (tmpDist > maxDist && Navagation.IsOnGrid(tmpTile) && gridData != null && gridData.IsTileOpen(tmpTile))
                                    {
                                        if (FindPath(tmpTile))
                                        {
                                            maxDist = tmpDist;
                                            maxTile = new Vector3Int(tmpTile.x, 0, tmpTile.z);
                                        }
                                    }
                                }
                            }
                        }
                        if(maxDist > 0)
                        {
                            FindPath(maxTile);
                        }
                    }
                }
                //List<Vector3Int> outList = new List<Vector3Int> { };
                //Zombie[] Zombies = Zombie.FindObjectsOfType<Zombie>();
                //foreach (Zombie zombie in Zombies)
                //{
            }
            yield return new WaitForSeconds(waitTime);
        }

        while (true)
        {
            //print("leaving");
            if (path == null)
            {
                FindPath(exitPoint);
                if (image != null && 5 < ShoppingItems.Count && ShoppingItems[5].image != null)
                {
                    //print("sprite changed to: " + (5) + ", " + ShoppingItems[5].image.ToString());
                    image.sprite = ShoppingItems[5].image;
                }
                else
                {
                    print("Sprite not changed: " + (image != null) + ", " + (5 < ShoppingItems.Count) + ", " + (ShoppingItems[5].image != null));
                }
            }
            else
            {
                NAVIGATIONSTATUS tmp = NavigatePath();
                if (tmp == NAVIGATIONSTATUS.ERROR || tmp == NAVIGATIONSTATUS.FINISHED)
                {
                    if (pos.Equals(exitPoint))
                    {
                        StartCoroutine(FadeOut());
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(waitTime);
        }

        yield return new WaitForSeconds(1f);
        // Use health to determine star rating system
        EventManager.UpdateRatingSystem(health);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision with enemy");
        if (other.CompareTag("Enemy") && canTakeDamage)
        {
            StartCoroutine(takeDamageCoroutine());
        }
    }

    private IEnumerator takeDamageCoroutine()
    {
        takeDamage();

        // Prevent further damage for a short duration
        canTakeDamage = false;
        yield return new WaitForSeconds(1f); // Delay before taking damage again
        canTakeDamage = true;
    }

    private void takeDamage()
    {
        health--;
        Debug.Log($"Customer health: {health}");

        if (health <= 0)
        {
            Debug.Log("Customer died.");
            EventManager.UpdateRatingSystem(health);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        EventManager.OnCustomerDestroyed?.Invoke(gameObject);
    }

    internal void SetPlacementSystem(PlacementSystem placementSystem)
    {
        this.placementSystem = placementSystem;
    }

    private IEnumerator FadeOut()
    {
        float alpha = 1.0f;
        float fadeSpeed = 10f;    // Movement speed while shrinking
        bool bRunAgain = true;
        while (alpha >= 0.0f)
        {
            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                UnityEngine.Color color = meshRenderer.material.color;
                color.a = alpha;
                meshRenderer.material.color = color;
            }

            Image[] images = GetComponentsInChildren<Image>();
            foreach (Image image in images)
            {
                UnityEngine.Color color = image.color;
                color.a = alpha;
                image.color = color;
            }
            alpha -= fadeSpeed * Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        if (true)
        {
            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                UnityEngine.Color color = meshRenderer.material.color;
                color.a = 0.0f;
                meshRenderer.material.color = color;
            }

            Image[] images = GetComponentsInChildren<Image>();
            foreach (Image image in images)
            {
                UnityEngine.Color color = image.color;
                color.a = 0.0f;
                image.color = color;
            }
        }
    }
    private IEnumerator FadeIn()
    {
        float alpha = 0.0f;
        float fadeSpeed = 10f;    // Movement speed while shrinking
        UpdatePos();
        bool bRunAgain = true;
        while (alpha <= 1.0)
        {
            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                UnityEngine.Color color = meshRenderer.material.color;
                color.a = alpha;
                meshRenderer.material.color = color;
            }

            Image[] images = GetComponentsInChildren<Image>();
            foreach (Image image in images)
            {
                UnityEngine.Color color = image.color;
                color.a = alpha;
                image.color = color;
            }
            alpha += fadeSpeed * Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        if (true)
        {
            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                UnityEngine.Color color = meshRenderer.material.color;
                color.a = 1.0f;
                meshRenderer.material.color = color;
            }

            Image[] images = GetComponentsInChildren<Image>();
            foreach (Image image in images)
            {
                UnityEngine.Color color = image.color;
                color.a = 1.0f;
                image.color = color;
            }
        }
    }

}
