using UnityEngine;
using System.Collections;

public class CamPicker : MonoBehaviour
{ 


    [Header("Einstellungen")]
    public Transform animationObject; // Dein Vogel/Animationsobjekt
    public float holdDuration = 3.0f; // Wie lange soll die Kamera folgen?

    private bool isAttached = false;
    private Vector3 originalPosition;

    void Update()
    {
        // Wenn noch nicht angehaftet und das Animationsobjekt den X-Wert erreicht/unterschreitet
        if (!isAttached && animationObject.position.x <= transform.position.x)
        {
            StartCoroutine(AttachAndFollow());
        }
    }

    IEnumerator AttachAndFollow()
    {
        isAttached = true;

        // Den Tracker an das Animationsobjekt heften
        // "worldPositionStays: true" sorgt dafür, dass er nicht wegspringt
        transform.SetParent(animationObject, true);

        // Die Dauer warten (einstellbar im Inspector)
        yield return new WaitForSeconds(holdDuration);

        // Den Tracker wieder lösen (er bleibt im Worldspace, wo er gerade ist)
        transform.SetParent(null);

        // Optional: Falls er danach gar nicht mehr triggern soll, 
        // kann man das Script hier deaktivieren:
        // this.enabled = false;
    }
}
