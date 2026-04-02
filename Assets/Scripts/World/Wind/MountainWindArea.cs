using UnityEngine;

public class MountainWindArea : MonoBehaviour
{
    [Header("Wind & Timer")]
    public float windForce = 5f;
    public float gustDuration = 4f;
    public float pauseDuration = 6f;

    [Header("Visuals (Shader: _FogSpeed)")]
    public SpriteRenderer desertWindRenderer;
    public float idleFogSpeed = 0.3f;
    public float gustFogSpeed = 4.0f;

    [Header("Audio")]
    public AudioSource gustAudioSource;
    public AudioClip[] gustClips;

    private PlayerRunning _playerRun;
    private float _originalMaxX;
    private float _timer;
    private bool _isWindActive;
    private float _currentDir; // -1 = Links (Gegenwind), 1 = Rechts (Rückenwind)
    private int _step;

    private void Start()
    {
        // Sicherstellen, dass zum Start alles auf Idle steht
        _isWindActive = false;
        _currentDir = 0f;
        ApplyShaderSpeed();
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (!_isWindActive)
        {
            if (_timer >= pauseDuration)
            {
                // WIND STARTET
                _timer = 0;
                _isWindActive = true;

                // Zyklus: 1x Rechts (Schub), 2x Links (Widerstand)
                _currentDir = (_step == 0) ? 1f : -1f;
                _step = (_step + 1) % 3;

                ApplyShaderSpeed();

                if (gustAudioSource && gustClips != null && gustClips.Length > 0)
                    gustAudioSource.PlayOneShot(gustClips[Random.Range(0, gustClips.Length)]);
            }
        }
        else
        {
            if (_timer >= gustDuration)
            {
                // WIND STOPPT
                _timer = 0;
                _isWindActive = false;
                _currentDir = 0f; // Kraft für Physik-Box auf 0

                ApplyShaderSpeed();
            }
        }
    }

    private void ApplyShaderSpeed()
    {
        if (desertWindRenderer == null) return;

        float targetSpeed;

        if (_isWindActive)
        {
            // DEINE REGEL: 
            // Rückenwind (_currentDir = 1) -> Shader Negativ
            // Gegenwind (_currentDir = -1) -> Shader Positiv
            targetSpeed = (_currentDir > 0) ? -gustFogSpeed : gustFogSpeed;
        }
        else
        {
            // IDLE: Standard-Laufrichtung (Positiv)
            targetSpeed = idleFogSpeed;
        }

        // HARTER WECHSEL - Direkte Zuweisung, kein Update-Zyklus
        desertWindRenderer.material.SetVector("_FogSpeed", new Vector2(targetSpeed, 0));
    }

    // --- TRIGGER PHYSIK ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        var run = other.GetComponentInParent<PlayerRunning>();
        if (run != null)
        {
            _playerRun = run;
            _originalMaxX = run.MaxVelocityX;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (_playerRun != null)
        {
            // Beeinflusst MaxVelocityX basierend auf der aktuellen Richtung (1, -1 oder 0)
            _playerRun.MaxVelocityX = _originalMaxX + (_currentDir * windForce);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_playerRun != null)
        {
            _playerRun.MaxVelocityX = _originalMaxX;
            _playerRun = null;
        }
    }
}