using System.Collections;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CheckpointTrigger : MonoBehaviour
{
    [Header("Trigger")]
    public string playerTag = "Player";
    public bool triggerOnce = true;

    [Tooltip("If empty, all TriggerAction components on this GameObject will be used.")]
    public TriggerAction[] actions;

    [Header("Optional Context")]
    [Tooltip("If assigned, it will be passed to the actions (e.g. for camera zoom).")]
    public CinemachineCamera virtualCamera;

    [Tooltip("Name of the players movement component ('PlayerMovement'). Leave empty to disable auto-fetch.")]
    public string playerMovementComponentName = "PlayerMovement";

    private bool _hasTriggered;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void Awake()
    {
        var col = GetComponent<Collider2D>();
        if (!col.isTrigger) col.isTrigger = true;

        if (actions == null || actions.Length == 0)
            actions = GetComponents<TriggerAction>();
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

        MonoBehaviour movement = null;
        if (!string.IsNullOrEmpty(playerMovementComponentName))
        {
            // Searches for a component by type name (e.g. "PlayerMovement")
            var comp = player.GetComponents<MonoBehaviour>()
                             .FirstOrDefault(m => m != null && m.GetType().Name == playerMovementComponentName);
            movement = comp;
        }

        var ctx = new TriggerInfoBundle(gameObject, player, movement, virtualCamera);

        // Executes actions in the order they appear in the Inspector
        foreach (var action in actions)
        {
            if (action == null) continue;

            var routine = StartCoroutine(action.Execute(ctx));
            if (action.blockUntilFinished)
                yield return routine;
        }
    }
}
