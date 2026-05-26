using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Collectible Type")]
    public CollectibleType type = CollectibleType.Blue;

    [Header("Visual Settings")]
    [SerializeField] private float rotationSpeed = 60f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.3f;

    [Header("Audio")]
    [SerializeField] private AudioClip[] collectSounds;

    private Vector3 startPosition;
    private GameManager gameManager;
    private bool isCollected = false;

    public enum CollectibleType
    {
        Blue,
        Gold,
        Rainbow
    }

    void Start()
    {
        startPosition = transform.position;
        
        // FIXED: Use FindFirstObjectByType for Unity 6
        #if UNITY_6000_0_OR_NEWER
        gameManager = FindFirstObjectByType<GameManager>();
        #else
        gameManager = FindObjectOfType<GameManager>();
        #endif
        
        if (gameManager == null)
        {
            Debug.LogError("[Collectible] GameManager NOT FOUND! Position: " + transform.position);
        }
        else
        {
            Debug.Log("[Collectible] GameManager found OK");
        }

        SetColor();
    }

    void Update()
    {
        if (isCollected) return;

        // Rotate
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Bob up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void SetColor()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = renderer.material;
            mat.EnableKeyword("_EMISSION");

            switch (type)
            {
                case CollectibleType.Blue:
                    mat.color = new Color(0.25f, 0.5f, 0.9f);
                    mat.SetColor("_EmissionColor", new Color(0.5f, 0.7f, 1f) * 0.5f);
                    break;
                case CollectibleType.Gold:
                    mat.color = new Color(1f, 0.84f, 0f);
                    mat.SetColor("_EmissionColor", new Color(1f, 0.9f, 0.3f) * 0.8f);
                    break;
                case CollectibleType.Rainbow:
                    mat.color = new Color(1f, 0.08f, 0.58f);
                    mat.SetColor("_EmissionColor", new Color(1f, 0.4f, 0.7f) * 1.2f);
                    break;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("[Collectible] Trigger by: " + other.name + " | Tag: " + other.tag);
        
        if (other.CompareTag("Player") && !isCollected)
        {
            Debug.Log("[Collectible] PLAYER DETECTED! Collecting...");
            Collect();
        }
    }

    void Collect()
    {
        if (isCollected)
        {
            Debug.LogWarning("[Collectible] Already collected!");
            return;
        }
        
        isCollected = true;
        Debug.Log("[Collectible] === COLLECT START === Type: " + type);

        int points = 0;
        int timeBonus = 0;

        switch (type)
        {
            case CollectibleType.Blue:
                points = 10;
                break;
            case CollectibleType.Gold:
                points = 50;
                break;
            case CollectibleType.Rainbow:
                points = 100;
                timeBonus = 15;
                break;
        }

        Debug.Log("[Collectible] Points: " + points + " | TimeBonus: " + timeBonus);

        // Add score
        if (gameManager != null)
        {
            Debug.Log("[Collectible] Calling AddScore(" + points + ")");
            gameManager.AddScore(points);
            Debug.Log("[Collectible] AddScore DONE");
            
            if (timeBonus > 0)
            {
                Debug.Log("[Collectible] Calling AddTime(" + timeBonus + ")");
                gameManager.AddTime(timeBonus);
            }
        }
        else
        {
            Debug.LogError("[Collectible] === CRITICAL: GameManager is NULL! ===");
            
            // Try find again
            #if UNITY_6000_0_OR_NEWER
            gameManager = FindFirstObjectByType<GameManager>();
            #else
            gameManager = FindObjectOfType<GameManager>();
            #endif
            
            if (gameManager != null)
            {
                Debug.Log("[Collectible] Found on retry! Adding score...");
                gameManager.AddScore(points);
                if (timeBonus > 0) gameManager.AddTime(timeBonus);
            }
            else
            {
                Debug.LogError("[Collectible] Still NULL after retry!");
            }
        }

        // Play sound
        if (collectSounds != null && collectSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, collectSounds.Length);
            if (collectSounds[randomIndex] != null)
            {
                AudioSource.PlayClipAtPoint(collectSounds[randomIndex], transform.position, 0.5f);
            }
        }

        Debug.Log("[Collectible] === COLLECT END === Destroying object");
        Destroy(gameObject);
    }
}