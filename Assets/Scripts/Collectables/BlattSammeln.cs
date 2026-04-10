using System;
#if UNITY_EDITOR
using UnityEditor.EditorTools;
#endif
using UnityEngine;

public class BlattSammeln : MonoBehaviour
{
    [SerializeField] private int leafID; // Jede Instanz bekommt eine eigene ID (0–19)
    [SerializeField] private Sprite collectedSprite;
    private CollectableManager collectableManager;
    [SerializeField]
    private AudioClip audioClip;


    private SpriteRenderer spriteRenderer;
    private bool isCollected = false;
    private void Awake()
    {
        collectableManager = GameObject.Find("GameManager").GetComponent<CollectableManager>();
    }
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Prüfen, ob das Blatt schon gesammelt wurde
        if (collectableManager.IsLeafCollected(leafID))
        {
            SetCollectedState();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isCollected && other.CompareTag("Player"))
        {
            CollectLeaf();
            isCollected = true;
            PlaySound();
        }
    }

    private void PlaySound()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        Debug.Assert(audioSource);
        if(audioSource && audioClip)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }

    private void CollectLeaf()
    {
        collectableManager.CollectLeaf(leafID);

        // Sprite ändern
        SetCollectedState();
    }

    private void SetCollectedState()
    {
        if (collectedSprite != null)
            spriteRenderer.sprite = collectedSprite;
    }
}

