using UnityEngine;

public class BlattSammeln : MonoBehaviour
{
    [SerializeField] private int leafID; // Jede Instanz bekommt eine eigene ID (0–19)
    [SerializeField] private Sprite collectedSprite; 


    private SpriteRenderer spriteRenderer;
    private bool isCollected = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Prüfen, ob das Blatt schon gesammelt wurde
        if (CollectableManager.Instance.IsLeafCollected(leafID))
        {
            SetCollectedState();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isCollected && other.CompareTag("Player"))
        {
            CollectLeaf();
        }
    }

    private void CollectLeaf()
    {
        isCollected = true;
        CollectableManager.Instance.CollectLeaf(leafID);

        // Sprite ändern
        SetCollectedState();
    }

    private void SetCollectedState()
    {
        if (collectedSprite != null)
            spriteRenderer.sprite = collectedSprite;
    }
}

