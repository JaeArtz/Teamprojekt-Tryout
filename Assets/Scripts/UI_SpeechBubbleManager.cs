/* --------------------------------------------------
              Source: Tutorial from
                unitycodemonkey.com
            Code Monkey Youtube Tutorial
        "How to make Text Writing Effect in Unity"
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UI_SpeechBubbleManager : MonoBehaviour
{

    [Header("UI References")]
    [Tooltip("Drag SpeechBubble_TExt (TMP) in here")]
    [SerializeField] private TextMeshProUGUI messageText;
    [Tooltip("Optional- drag AudioSource in. If no Audio, just leave empty")]
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

    private RandomAudioPlayer audioPlayer;

    private void Awake()
    {
        speechBubbleContainer.SetActive(false);
        interactionVisual.SetActive(false); // Starts invisible, until closeness of Player triggers Visibility

        audioPlayer = GetComponents<RandomAudioPlayer>().FirstOrDefault(component => component.Name.Equals("Shimmer"));

        if (!audioPlayer) Debug.LogError(@"Random Audio Player with name ""Shimmer"" could not be found!");
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

            // Manages InteractionVisual (E) during Typing
            bool isDoneTyping = textWriterSingle == null || !textWriterSingle.IsActive();
            interactionVisual.SetActive(isDoneTyping && isWithinRadius);

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
        // interactionVisual.SetActive(false);
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

        cooldownIsRunning = true;
        cooldownTimer = cooldownTime;
        
    }

    private void HandleCooldown()
    {
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0)
        {
            cooldownIsRunning = false;
            // InteractionVisual will be reactivated during next Update-Frame by Radius-Check wieder
        }
    }

    private void StartTalkingSound()
    {
        if (talkingAudioSource != null) talkingAudioSource.Play();
        audioPlayer.PlayRandomSound();
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

/*
 * 
 * Texteingabe (im STring-Array)

Fett: <b>A</b> → A

Kursiv: <i>A</i> → A

Größe: <size=120%>GROSS</size> (Gut, um Tastenbefehle hervorzuheben)

Kombination: <b><color=red>A</color></b> (Fett UND Rot)

Zeilenumbruch:  <br>   z.B. Press A - Move Left <br> Press B - Move Right
*/