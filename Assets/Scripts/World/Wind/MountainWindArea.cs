using UnityEngine;

public class MountainWindArea : MonoBehaviour
{
    [Header("Wind & Timer")]
    public float windForce = 5f; // Dein fester Wert zum Verändern (Variable 2)
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
    private float _originalMaxX; // Variable 1: Merkt sich den Ursprungswert
    private float _timer;
    private bool _isWindActive;
    private float _currentDir; // -1 = Links, 1 = Rechts
    private int _step;

    private void Start()
    {
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
                StartWind();
            }
        }
        else
        {
            if (_timer >= gustDuration)
            {
                StopWind();
            }
        }
    }

    private void StartWind()
    {
        _timer = 0;
        _isWindActive = true;

        // Zyklus: 1x Rechts (Schub), 2x Links (Widerstand)
        _currentDir = (_step == 0) ? 1f : -1f;
        _step = (_step + 1) % 3;

        ApplyShaderSpeed();
        ApplyWindToPlayer(); // Hier setzen wir den Wert auf Variable 2

        if (gustAudioSource && gustClips != null && gustClips.Length > 0)
            gustAudioSource.PlayOneShot(gustClips[Random.Range(0, gustClips.Length)]);
    }

    private void StopWind()
    {
        _timer = 0;
        _isWindActive = false;

        ResetPlayerValue(); // Hier setzen wir den Wert zurück auf Variable 1
        _currentDir = 0f;

        ApplyShaderSpeed();
    }

    private void ApplyWindToPlayer()
    {
        if (_playerRun != null && _isWindActive)
        {
            // Setzt den Wert auf (Original + Wind-Einfluss)
            _playerRun.MaxVelocityX = _originalMaxX + (_currentDir * windForce);
        }
    }

    private void ResetPlayerValue()
    {
        if (_playerRun != null)
        {
            // Setzt den Wert zurück auf den gespeicherten Originalwert
            _playerRun.MaxVelocityX = _originalMaxX;
        }
    }

    private void ApplyShaderSpeed()
    {
        if (desertWindRenderer == null) return;
        float targetSpeed = _isWindActive ? ((_currentDir > 0) ? -gustFogSpeed : gustFogSpeed) : idleFogSpeed;
        desertWindRenderer.material.SetVector("_FogSpeed", new Vector2(targetSpeed, 0));
    }

    // --- TRIGGER LOGIK ---

    private void OnTriggerEnter2D(Collider2D other)
    {
        var run = other.GetComponentInParent<PlayerRunning>();
        if (run != null)
        {
            _playerRun = run;
            _originalMaxX = run.MaxVelocityX; // Schritt 1 & 2: Originalwert in Variable 1 kopieren

            if (_isWindActive)
            {
                ApplyWindToPlayer(); // Falls der Wind schon weht, wenn man reinläuft
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var run = other.GetComponentInParent<PlayerRunning>();
        if (run != null && run == _playerRun)
        {
            ResetPlayerValue(); // Schritt 5: Wert zurücksetzen
            _playerRun = null;
        }
    }
}