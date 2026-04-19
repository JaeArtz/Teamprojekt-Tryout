using System.Collections;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class CheckpointTriggerAutoStart : MonoBehaviour
{
    [Header("Trigger")]
    public string playerTag = "Player";
    public bool triggerOnce = true;
    [Tooltip("drag Player in here.")]
    public GameObject manualPlayerReference;

    [Tooltip("If empty, all TriggerAction components on this GameObject will be used.")]
    public TriggerAction[] actions;

    [Header("Optional Context")]
    [Tooltip("If assigned, it will be passed to the actions (e.g. for camera zoom).")]
    public CinemachineCamera virtualCamera;

    private bool _hasTriggered;

    private GameObject HUD;
    private GameObject pauseCanvas;
    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void Awake()
    {

        HUD = GameObject.Find("HUDCanvas");
        pauseCanvas = GameObject.Find("PauseCanvas");
        var col = GetComponent<Collider2D>();
        if (!col.isTrigger) col.isTrigger = true;

        if (actions == null || actions.Length == 0)
            actions = GetComponents<TriggerAction>();
    }

    // difference to other CheckpointTrigger: starts automatically at start of Level, no "Triggerbox"
    private void Start()
    {
        HUD.SetActive(false);
        pauseCanvas.SetActive(false);
        _hasTriggered = true;

        GameObject targetPlayer = manualPlayerReference;
        if (targetPlayer == null)
        {
            targetPlayer = GameObject.FindGameObjectWithTag(playerTag);
        }

        if (targetPlayer != null)
        {
            StartCoroutine(Run(targetPlayer));
        }
       
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasTriggered && triggerOnce) return;
        if (!other.CompareTag(playerTag)) return;

        _hasTriggered = true;
        StartCoroutine(Run(other.gameObject));
    }

    private IEnumerator Run(GameObject player)
    {
        if (actions == null || actions.Length == 0)
            yield break;

        // we give 'null' for Movement, because Actions directly access the Player
        var ctx = new TriggerInfoBundle(gameObject, player, null, virtualCamera);

        foreach (var action in actions)
        {
            if (action == null) continue;

            var routine = StartCoroutine(action.Execute(ctx));
            if (action.blockUntilFinished)
                yield return routine;
        }
    }
}