using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;
    //ID for Powerups
    //0 = tripleshot
    //1 = Speed
    //2 = Shields
    [SerializeField]
    private int _powerupID;

    [SerializeField]
    private AudioClip _powerUpsSound;
    [SerializeField]
    private float _volume;
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < -5.5f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //camera position z axis
        
        if (other.tag == "Player") 
        {
            Vector3 cameraPos = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
            AudioSource.PlayClipAtPoint(_powerUpsSound, cameraPos, _volume);
            Player player = other.transform.GetComponent<Player>();

            switch (_powerupID) 
            {
                case 0:
                    player.TripleShotActive();
                    break;

                case 1:
                    player.SpeedPowerupActive();
                    break;

                case 2:
                    player.ShieldPowerupActive();
                    break;
                default:
                    Debug.Log("Default Value");
                    break;

            }
            Destroy(this.gameObject);
        }      
    }
}
