using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4f;
    /*[SerializeField]
    private GameObject _enemyPrefab;*/
    [SerializeField]
    private GameObject _laserPrefab;


    private Player _player;
    private Animator _enemy_Animator;
    private AudioSource _explosionSound;
    private float _fireRate = 3.0f;
    private float _canFire = -1;

    // Start is called before the first frame update
    void Start()
    {
        _explosionSound = GetComponent<AudioSource>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("The Player is NULL.");
        }
        _enemy_Animator = GetComponent<Animator>();
        if (_enemy_Animator == null)
        {
            Debug.LogError("The animator is NULL.");
        }
        //StartCoroutine(FireLaserRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Time.time > _canFire) 
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    void CalculateMovement() 
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -5.5f)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 7, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") 
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null) 
            {
                player.Damage();
            }
            _enemy_Animator.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _explosionSound.Play();
            _canFire = 1.5f + Time.time;
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 1.4f);
        }

        if (other.tag == "Laser") 
        {
            Laser laser = other.GetComponent<Laser>();
            if (laser.GetEnemyLaserValue() == false)
            {
                Destroy(other.gameObject);

                if (_player != null)
                {
                    _player.UpdateScore(10);
                }
                _enemy_Animator.SetTrigger("OnEnemyDeath");
                _speed = 0;
                _explosionSound.Play();
                _canFire = 1.5f + Time.time;
                Destroy(GetComponent<Collider2D>());
                Destroy(this.gameObject, 1.4f);
            }
        }
    }
}
