using System.Linq;
using UnityEngine;
public class ArrowTrap : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float arrowRange = 10f;
    [SerializeField] private float arrowSpeed = 10f;
    [SerializeField] private bool activateOnStart = false; // Startet die Falle sofort oder erst durch eine Aktivierung?
    
    private float cooldownTimer;
    private bool isActive = false;

    private RandomAudioPlayer audioPlayer;

    private void Awake()
    {
        audioPlayer = GetComponents<RandomAudioPlayer>().FirstOrDefault(component => component.Name.Equals("ArrowShoot"));

        if (!audioPlayer) Debug.LogError(@"Random Audio Player with name ""ArrowShoot"" could not be found!");
    }

    private void Start()
    {
        isActive = activateOnStart;
    }

    public void Activate()
    {
        isActive = true;
    }

    private void Attack()
    {
        cooldownTimer = 0;

        // 1. Pfeil aus dem ProjectilePool holen (Object Pooling)
        EnemyProjectile arrow = ProjectilePool.Instance.GetArrow();
        
        // 2. Position auf den FirePoint setzen
        arrow.transform.position = firePoint.position;
        
        // 3. Individuelle Werte an das Projektil übergeben
        arrow.setRange(arrowRange);
        arrow.setSpeed(arrowSpeed);
        
        // 4. Pfeil scharf schalten und Bewegung starten
        arrow.ActivateProjectile();
    }

    private void Update()
    {
        if(!isActive) return;
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= attackCooldown) // Wenn die Abklingzeit erreicht ist, angreifen
        {
            Attack();
            audioPlayer.PlayRandomSound();
        }
    }

    // Visualisierung der Schussbahn im Editor
    private void OnDrawGizmos()
    {
        if (firePoint == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(firePoint.position, firePoint.position + Vector3.left * arrowRange);
    }
}