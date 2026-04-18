using System;
using TMPro;

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
    private GameObject notifyPanel;

    private SpriteRenderer spriteRenderer;
    private bool isCollected = false;
    private void Awake()
    {
        collectableManager = GameObject.Find("GameManager").GetComponent<CollectableManager>();
        notifyPanel = GameObject.Find("NotifyPanel");
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

            string currentStory = processDisplay();
            Debug.Log(currentStory);
            if(currentStory.Length > 1)
            {
                notifyPanel.SetActive(true);
                StartCoroutine(notifyPanel.GetComponent<ShowStoryAnimation>().ShowCollectedAnimation(currentStory));
            }
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
        isCollected = true;
    }


    private string processDisplay()
    {
        if (leafID == 1)
        {
            return "Unlocked Story Nr.1";
        }
        if (leafID == 2)
        {
            return "Unlocked Story Nr.6";
        }
        if (leafID == 4)
        {
            return "Unlocked Story Nr.7";
        }
        if (leafID == 5)
        {
            return "Unlocked Story Nr.8";
        }
        if (leafID == 6)
        {
            return "Unlocked Story Nr.2";
        }
        if (leafID == 7)
        {
            return "Unlocked Story Nr.10";
        }
        if (leafID == 9)
        {
            return "Unlocked Story Nr.5";
        }
        if (leafID == 11)
        {
            return "Unlocked Story Nr.4";
        }
        if (leafID == 15)
        {
            return "Unlocked Story Nr.9";
        }
        if (leafID == 20)
        {
            return "Unlocked Story Nr.3";
        }
        if (leafID == 22)
        {
            return "Unlocked Story Nr.12";
        }
        if (leafID == 24)
        {
            return "Unlocked Story Nr.11";
        }
        return "";
    }
}

