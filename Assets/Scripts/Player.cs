using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    private float _speedMultiplier = 2;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _shieldVizualizer;
    [SerializeField]
    private GameObject _leftEngine, _rightEngine;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;

    private bool _isTripleShotActive = false;
    private bool _isSpeedPowerupActive = false;
    [SerializeField]
    private bool _isShieldPowerupActive = false;
    [SerializeField]
    private float _powerupTime = 5f;

    [SerializeField]
    private int _score = 0;
    private UIManager _uiManager;

    //variable to store the audio clip
    [SerializeField]
    private AudioClip _laserSoundClip;
    private AudioSource _audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _score = 0;

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is null");
        }
        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is null");
        }
        if (_audioSource == null)
        {
            Debug.LogError("AudioSource on the player is NULL");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(direction * _speed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }

    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;
        Vector3 laserOffset = new Vector3(0, 1.05f, 0);

        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + laserOffset, Quaternion.identity);
        }

        //play the laser audio clip
        //_laserBeamSound.Play();
        _audioSource.Play();


    }

    public void Damage()
    {
        //do nothing
        //deactivate shield
        //return;
        if (_isShieldPowerupActive == true)
        {
            _isShieldPowerupActive = false;
            _shieldVizualizer.SetActive(false);
            return;

        }

        _lives--;

        if (_lives == 2)
        {
            _rightEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _leftEngine.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDead();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine(_powerupTime));
    }

    public void SpeedPowerupActive()
    {
        _isSpeedPowerupActive = true;
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedPowerupDownRoutine(_powerupTime));
    }

    public void ShieldPowerupActive()
    {
        _isShieldPowerupActive = true;
        _shieldVizualizer.SetActive(true);
    }

    IEnumerator TripleShotPowerDownRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        _isTripleShotActive = false;
    }

    IEnumerator SpeedPowerupDownRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        _isSpeedPowerupActive = false;
        _speed /= _speedMultiplier;
    }
    public void UpdateScore(int points) 
    {
        _score += points;
        UpdateUIScore();
    }

    public void UpdateUIScore() 
    {
        _uiManager.UpdatePlayerScore(_score);
    }
    //method to add 10 to the score
    //Communicate with UI Manager to update the score
}
