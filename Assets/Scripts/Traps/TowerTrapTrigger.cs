using UnityEngine;

public class TowerTrapTrigger : MonoBehaviour
{
    [SerializeField] private ArrowTrap[] trapsToActivate;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (ArrowTrap trap in trapsToActivate)
            {
                trap.Activate();
            }
            gameObject.SetActive(false);
        }
    }
}