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
    private bool _hasStoredOriginalValue = false;

    private float _timer;
    private bool _isWindActive;
    private float _currentDir; // -1 or 1
    private int _step;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            var run = playerObj.GetComponentInParent<PlayerRunning>();
            if (run != null)
            {
                _originalMaxX = run.MaxVelocityX;
                _hasStoredOriginalValue = true;
                Debug.Log($"WindArea: Start-Speed {_originalMaxX} gespeichert.");
            }
        }
        ApplyShaderSpeed();
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (!_isWindActive)
        {
            if (_timer >= pauseDuration) StartWind();
        }
        else
        {
            // New: updates Player each Frame,applies current Windeffect into right direction
            // make sure Player ist pushed by Wind (X+5) or slowed by Wind (X-5)
            // depending on into which direction he and the Wind are moving
            ApplyWindToPlayer();

            if (_timer >= gustDuration) StopWind();
        }
    }

    private void StartWind()
    {
        _timer = 0;
        _isWindActive = true;
        // switches between 1 and -1
        _currentDir = (_step == 0) ? 1f : -1f;
        _step = (_step + 1) % 3;

        ApplyShaderSpeed();
        ApplyWindToPlayer();

        if (gustAudioSource && gustClips != null && gustClips.Length > 0)
            gustAudioSource.PlayOneShot(gustClips[Random.Range(0, gustClips.Length)]);
    }

    private void StopWind()
    {
        _timer = 0;
        _isWindActive = false;
        ResetPlayerValue();
        _currentDir = 0f;
        ApplyShaderSpeed();
    }

    private void ApplyWindToPlayer()
    {
        if (_playerRun == null || !_isWindActive || !_hasStoredOriginalValue) return;

        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // When WindGust stops blowing => Reset to originalX (should be 10)
        if (horizontalInput == 0)
        {
            _playerRun.MaxVelocityX = _originalMaxX;
            return;
        }

        // checks to make sure Player is "Blowin in The Wind" (in the Wind-direction)
        bool withWind = Mathf.Sign(horizontalInput) == Mathf.Sign(_currentDir);

        if (withWind)
            _playerRun.MaxVelocityX = _originalMaxX + windForce; // speed up, currently +5
        else
            _playerRun.MaxVelocityX = _originalMaxX - windForce; // slow down, currently -5
    }

    private void ResetPlayerValue()
    {
        if (_playerRun != null && _hasStoredOriginalValue)
        {
            _playerRun.MaxVelocityX = _originalMaxX;
        }
    }

    private void ApplyShaderSpeed()
    {
        if (desertWindRenderer == null) return;
        float targetSpeed = _isWindActive ? ((_currentDir > 0) ? -gustFogSpeed : gustFogSpeed) : idleFogSpeed;
        desertWindRenderer.material.SetVector("_FogSpeed", new Vector2(targetSpeed, 0));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var run = other.GetComponentInParent<PlayerRunning>();
        if (run != null)
        {
            _playerRun = run;
            if (_isWindActive) ApplyWindToPlayer();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var run = other.GetComponentInParent<PlayerRunning>();
        if (run != null && run == _playerRun)
        {
            ResetPlayerValue();
            _playerRun = null;
        }
    }
}