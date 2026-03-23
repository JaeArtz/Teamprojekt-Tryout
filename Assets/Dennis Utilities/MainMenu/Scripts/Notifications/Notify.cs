using UnityEngine;

[CreateAssetMenu(fileName = "Notify", menuName = "Scriptable Objects/Notify")]
public class Notify : ScriptableObject
{
    [SerializeField, TextArea] private string message;
    [SerializeField] private float displayDuration = 2.0f;
    [SerializeField] private float fadeDuration = 1.0f;

    public string Message => message;

    public float DisplayDuration => displayDuration;

    public float FadeDuration => fadeDuration;
}
