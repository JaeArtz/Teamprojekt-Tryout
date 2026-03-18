using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class PlayerLight : MonoBehaviour
{
    [SerializeField] private Light2D playerLight;

    private void Start()
    {
        // Prüft beim Start des Levels, ob der SoulManager existiert 
        // und ob die "catSoul" (Katzen-Seele) bereits im Besitz des Spielers ist.
        bool hasLight = SoulManager.Instance != null && SoulManager.Instance.HasSoul("catSoul");
        
        // Das GameObject des Lichts wird sicherheitshalber aktiviert
        playerLight.gameObject.SetActive(true);

        // die Licht-Komponente selbst wird nur eingeschaltet, 
        // wenn die Seele tatsächlich vorhanden ist (hasLight = true).
        playerLight.enabled = hasLight;
    }

    // Diese Methode wird aufgerufensobald die Katzen-Seele frisch eingesammelt wurde.
    public void ActivateLight()
    {
        Debug.Log("CatSoul collected: Activating player light.");
        // Schaltet licht ein
        playerLight.gameObject.SetActive(true);
        playerLight.enabled = true;
        Debug.Log("Player light enabled: " + playerLight.enabled);
    }

}