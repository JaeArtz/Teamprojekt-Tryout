/* --------------------------------------------------
              Source: Tutorial from
                unitycodemonkey.com
            Code Monkey Youtube Tutorial
        "How to make Text Writing Effect in Unity"
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_SpeechBubbleManager_Soul : MonoBehaviour
{

    [Header("UI References")]
    [Tooltip("Drag SpeechBubble_TExt (TMP) in here")]
    [SerializeField] private TextMeshProUGUI messageText;
    [Tooltip("Optional- drag AudioSource in. Plays while typing the text.")]
    [SerializeField] private AudioSource talkingAudioSource;
    [Tooltip("Drag Sprite-Object with Interaction-Button E in here")]
    [SerializeField] private GameObject interactionVisual;
    [Tooltip("Drag SpeechBubble_Canvas in here")]
    [SerializeField] private GameObject speechBubbleContainer;

    [Header("Radius & Player")]
    [Tooltip("Drag Player in here, as measure for actionRadius")]
    [SerializeField] private Transform playerTransform;
    [Tooltip("Distance of InteractionButton-Spawn, triggert by Closeness of Player")]
    [SerializeField] private float activationRadius = 5f;
    [Header("Abort Setting")]
    [Tooltip("Time in seconds until Bubble disappears, when Player leaves")]
    [SerializeField] private float exitRadiusCooldown = 2f; // 2f = 2 seconds until Bubble disappears

    private float exitTimer;

    [Header("General Settings")]
    [Tooltip("Cooldown, until E-Button appears again, for Reset of this SpeechBubble")]
    [SerializeField] private float cooldownTime = 3f;
    [SerializeField] private float typingSpeed = .02f;

    private TextWriter.TextWriterSingle textWriterSingle;
    private int messageIndex = 0;
    private bool conversationIsActive = false;
    private bool cooldownIsRunning = false;
    private float cooldownTimer;

    [SerializeField] private string[] messageArray;

    private void Awake()
    {
        speechBubbleContainer.SetActive(false);
        interactionVisual.SetActive(false); // Starts invisible, until closeness of Player triggers Visibility
    }

    private void Update()
    {
        // distance to Player
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool isWithinRadius = distance <= activationRadius;

        if (cooldownIsRunning)
        {
            HandleCooldown();
            return;
        }

        if (!conversationIsActive)
        {
            // shows E-Button only when Player is close
            interactionVisual.SetActive(isWithinRadius);

            if (isWithinRadius && Input.GetKeyDown(KeyCode.E))
            {
                StartConversation();
            }
        }
        // else => if Conversation IS active
        else
        {
            if (!isWithinRadius)
            {
                exitTimer -= Time.deltaTime;
                if (exitTimer <= 0)
                {
                    EndConversation();
                    return;
                }
            }
            else
            {
                // If Player comes back during Cooldown => reset 
                exitTimer = exitRadiusCooldown;
            }

            // Deaktiviert den E-Button sofort und dauerhaft, solange das Gespräch aktiv ist
            // => Textpopup wird einmalig kurz gezeigt
            interactionVisual.SetActive(false);

            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            {
                HandleInput();
            }
        }
    }

    private void StartConversation()
    {
        conversationIsActive = true;
        messageIndex = 0;

        // Prüft, ob irgendwo eine Seele hängt
        SoulPickup soul = GetComponentInChildren<SoulPickup>();
        if (soul != null)
        {
            // => Damit die Seele nur optisch weg ist, aber noch nicht deaktiviert
            soul.StartPickupSequence();
        }

        speechBubbleContainer.SetActive(true);
        ShowNextMessage();
    }

    private void HandleInput()
    {
        if (textWriterSingle != null && textWriterSingle.IsActive())
        {
            textWriterSingle.WriteAllAndDestroy();
        }
        else
        {
            messageIndex++;
            if (messageIndex < messageArray.Length)
            {
                ShowNextMessage();
            }
            else
            {
                EndConversation();
            }
        }
    }

    private void ShowNextMessage()
    {
        string message = messageArray[messageIndex];
        StartTalkingSound();
        textWriterSingle = TextWriter.AddWriter_Static(messageText, message, typingSpeed, true, true, StopTalkingSound);
    }

    private void EndConversation()
    {
        conversationIsActive = false;
        speechBubbleContainer.SetActive(false);

        interactionVisual.SetActive(false);

        // Wenn eine Seele "involviert" war, regelt das SoulPickup Skript 
        // über die Coroutine das Deaktivieren des gesamten Objekts.
        // Ansonsten bleibt der Cooldown aktiv (=> beim Sprechen mit Glühwürmchen)
        if (GetComponentInChildren<SoulPickup>() == null)
        {
            cooldownIsRunning = true;
            cooldownTimer = cooldownTime;
        }
    }

    private void HandleCooldown()
    {
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0)
        {
            cooldownIsRunning = false;
        }
    }

    private void StartTalkingSound()
    {
        if (talkingAudioSource != null) talkingAudioSource.Play();
    }

    private void StopTalkingSound()
    {
        if (talkingAudioSource != null) talkingAudioSource.Stop();
    }

    // Visualises Radius in Unity-Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }
}