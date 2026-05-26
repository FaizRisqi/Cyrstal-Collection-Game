using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject collectiblePrefab;
    [SerializeField] private int level1Count = 15;
    [SerializeField] private int level2Count = 20;
    
    [Header("Spawn Area")]
    [SerializeField] private Vector3 spawnAreaCenter = Vector3.zero;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(40f, 0f, 40f);
    [SerializeField] private float spawnHeight = 1f;
    
    [Header("Crystal Distribution (%)")]
    [Range(0, 100)] [SerializeField] private int bluePercentage = 70;
    [Range(0, 100)] [SerializeField] private int goldPercentage = 20;
    // Rainbow = remaining percentage

    [Header("Current Level")]
    [SerializeField] private int currentLevel = 1;

    void Start()
    {
        SpawnCollectibles();
    }

    void SpawnCollectibles()
    {
        if (collectiblePrefab == null)
        {
            Debug.LogError("Collectible prefab not assigned!");
            return;
        }

        int count = (currentLevel == 1) ? level1Count : level2Count;

        for (int i = 0; i < count; i++)
        {
            // Random position within spawn area
            Vector3 randomPos = new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                spawnHeight,
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            ) + spawnAreaCenter;

            // Raycast to place on ground
            RaycastHit hit;
            if (Physics.Raycast(randomPos + Vector3.up * 50f, Vector3.down, out hit, 100f))
            {
                randomPos = hit.point + Vector3.up * spawnHeight;
            }

            // Spawn collectible
            GameObject crystal = Instantiate(collectiblePrefab, randomPos, Quaternion.identity);
            
            // Set random type based on percentage
            Collectible collectible = crystal.GetComponent<Collectible>();
            if (collectible != null)
            {
                int rand = Random.Range(0, 100);
                
                if (rand < bluePercentage)
                {
                    collectible.type = Collectible.CollectibleType.Blue;
                }
                else if (rand < bluePercentage + goldPercentage)
                {
                    collectible.type = Collectible.CollectibleType.Gold;
                }
                else
                {
                    collectible.type = Collectible.CollectibleType.Rainbow;
                }
            }
        }

        // --- TAMBAHKAN BARIS INI UNTUK FIX BUG MENANG DI KRISTAL PERTAMA ---
        #if UNITY_6000_0_OR_NEWER
        GameManager gm = FindFirstObjectByType<GameManager>();
        #else
        GameManager gm = FindObjectOfType<GameManager>();
        #endif
        
        if (gm != null) gm.SetTotalCollectibles(count);
        // ------------------------------------------------------------------

        Debug.Log("Spawned " + count + " collectibles for Level " + currentLevel);
    }

    // Visualize spawn area in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawCube(spawnAreaCenter, spawnAreaSize);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}