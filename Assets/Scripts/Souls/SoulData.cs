using UnityEngine;

[CreateAssetMenu(fileName = "New Soul", menuName = "Souls/SoulData")]
public class SoulData : ScriptableObject
{
    public string soulID;         // eindeutige ID
    public string displayName;
    public Sprite icon;
    [TextArea] public string description;

    // optional: categorie, cooldown, prefab reference etc.
}
