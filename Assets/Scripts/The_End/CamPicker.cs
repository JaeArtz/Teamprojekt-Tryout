using UnityEngine;
using System.Collections;

public class CamPicker : MonoBehaviour
{ 


    [Header("Einstellungen")]
    [Tooltip("Place Phoenix/ AnimationObject here")]
    public Transform animationObject;
    [Tooltip("Duration in seconds: How long will the Cam be carried along?")]
    public float holdDuration = 3.0f;

    private bool isAttached = false;
    private Vector3 originalPosition;

    void Update()
    {
        // checks if X-Value is reached, or under X-Value, and if Cam is attached
        if (!isAttached && animationObject.position.x <= transform.position.x)
        {
            StartCoroutine(AttachAndFollow());
        }
    }

    IEnumerator AttachAndFollow()
    {
        isAttached = true;
        
        // Attaches Tracker to AnimationObject (Phoenix), by setting it as Parent of CamTrackerPoint
        transform.SetParent(animationObject, true);

        // Carries Cam this amount of seconds
        yield return new WaitForSeconds(holdDuration);

        // Lets go of Tracker (just drops it completely)
        transform.SetParent(null);

        // For Script Deactivation, if necessary:
        // this.enabled = false;
    }
}
