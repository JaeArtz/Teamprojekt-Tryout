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

public class UI_SpeechBubbleManager : MonoBehaviour
{

    [Header("UI Referenzen")]
    [Tooltip("Drag SPeechBubble_TExt (TMP) in here")]
    [SerializeField] private TextMeshProUGUI messageText;
    [Tooltip("Optional- drag AudioSource in. If no Audio, just leave empty")]
    [SerializeField] private AudioSource talkingAudioSource;
    [Tooltip("Drag Sprite with Interaction-Button E in here")]
    [SerializeField] private GameObject interactionVisual;
    [Tooltip("Drag SpeechBubble_Canvas in here")]
    [SerializeField] private GameObject speechBubbleContainer;

    [Header("Radius & Spieler")]
    [Tooltip("Drag Player in here, as measure for actionRadius")]
    [SerializeField] private Transform playerTransform;
    [Tooltip("Distance of InteractionButton-Spawn, triggert by Closeness of Player")]
    [SerializeField] private float activationRadius = 5f;
    [Header("Abbruch Einstellungen")]
    [SerializeField] private float exitRadiusCooldown = 2f; // Zeit in Sek., bis Gespräch abbricht
    private float exitTimer;

    [Header("Einstellungen")]
    [SerializeField] private float cooldownTime = 3f;
    [SerializeField] private float typingSpeed = .02f;

    private TextWriter.TextWriterSingle textWriterSingle;
    private int messageIndex = 0;
    private bool isConversationActive = false;
    private bool isCooldown = false;
    private float cooldownTimer;

    [SerializeField] private string[] messageArray;

    private void Awake()
    {
        speechBubbleContainer.SetActive(false);
        interactionVisual.SetActive(false); // Startet unsichtbar, bis Spieler nah dran ist
    }

    private void Update()
    {
        // Distanz berechnen
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool isWithinRadius = distance <= activationRadius;

        if (isCooldown)
        {
            HandleCooldown();
            return;
        }

        // FALL A: Konversation NICHT aktiv
        if (!isConversationActive)
        {
            // Zeige das "E" nur, wenn der Spieler im Radius ist
            interactionVisual.SetActive(isWithinRadius);

            if (isWithinRadius && Input.GetKeyDown(KeyCode.E))
            {
                StartConversation();
            }
        }
        // FALL B: Konversation IST aktiv
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
                // Wenn der Spieler wieder zurück in den Radius kommt, Timer resetten
                exitTimer = exitRadiusCooldown;
            }

            // InteractionVisual (E) während des Schreibens steuern
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
        isConversationActive = true;
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
        isConversationActive = false;
        speechBubbleContainer.SetActive(false);

        interactionVisual.SetActive(false);

        isCooldown = true;
        cooldownTimer = cooldownTime;
        
    }

    private void HandleCooldown()
    {
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0)
        {
            isCooldown = false;
            // InteractionVisual wird im nächsten Update-Frame durch Radius-Check wieder aktiviert
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

    // Visualisierung des Radius im Unity-Editor (hilfreich zum Einstellen)
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