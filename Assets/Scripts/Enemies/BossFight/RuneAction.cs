using System.Collections;

using UnityEngine;

using UnityEngine.Events;



public class RuneAction : TriggerAction

{

    [Header("Rune Settings")]

    public string runeID;

    public RunePuzzleManager puzzleManager;



    [Header("Custom Events")]

    public UnityEvent onRuneActivated;



    [Header("Visual Effects (Nur Kinder!)")]

    public GameObject activatedVisuals;

    public GameObject lightPulseObject;

    public GameObject interactionHint;



    [Header("Continuous Effects")]

    public AudioSource hummingSound;



    private bool isActivated = false;

    private bool isPlayerInside = false;



    // --- Trying UPDATE instead of COROUTINE, see if that works better ---

    void Update()

    {

        if (isPlayerInside && !isActivated && Input.GetKeyDown(KeyCode.E))

        {

            ActivateRune();

        }

    }



    private void ActivateRune()

    {

        isActivated = true;



        // Visuals & Sound

        if (interactionHint != null) interactionHint.SetActive(false);

        if (activatedVisuals != null) activatedVisuals.SetActive(true);

        if (lightPulseObject != null) lightPulseObject.SetActive(true);

        if (hummingSound != null) hummingSound.Play();



        // Starts BossEndTrigger 

        onRuneActivated?.Invoke();



        if (puzzleManager != null)

            puzzleManager.RegisterRuneActivation(runeID, this, null);



        Debug.Log("Rune durch E-Taste aktiviert!");

    }



    public override IEnumerator Execute(TriggerInfoBundle ctx)

    {     

        yield break;

    }



    public void Deactivate()

    {

        isActivated = false;

        if (activatedVisuals != null) activatedVisuals.SetActive(false);

        if (lightPulseObject != null) lightPulseObject.SetActive(false);

        if (hummingSound != null) hummingSound.Stop();

        if (interactionHint != null) interactionHint.SetActive(false);

    }



    private void OnTriggerEnter2D(Collider2D other)

    {

        if (isActivated) return;

        if (other.CompareTag("Player"))

        {

            isPlayerInside = true;

            if (interactionHint != null) interactionHint.SetActive(true);

        }

    }



    private void OnTriggerExit2D(Collider2D other)

    {

        if (other.CompareTag("Player"))

        {

            isPlayerInside = false;

            if (interactionHint != null) interactionHint.SetActive(false);

        }

    }

}