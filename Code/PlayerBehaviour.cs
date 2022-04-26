using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : MonoBehaviour
{
    public Vector3 teleportDistance;
    public Vector3 spawnDistance;
    public GameObject timerBullet;
    public GameObject triggerBullet;
    public bool canShoot = true;
    public TextMeshProUGUI countdown;
    public TextMeshProUGUI triggerText;
    public bool triggerArmed = false;
    public Vector3 SpawnPos;
    public ParticleSystem deathParticles;
    public PlayerHealth health;
    public ThirdPersonController Controller;

    void Start()
    {
        SpawnPos = transform.position;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("TeleportPlane"))
        {
            GetComponent<CharacterController>().enabled = false;
            transform.Translate(teleportDistance);
            GetComponent<CharacterController>().enabled = true;
        }
        
        if (other.gameObject.CompareTag("DeathPlane"))
        {
            StartCoroutine(Damage());
        }
        
        if (other.gameObject.CompareTag("Boss"))
        {
            if (Controller.Grounded)
            {
                StartCoroutine(Damage());
            }
        }
        
        if (other.gameObject.CompareTag("SpikeZone"))
        {
            StartCoroutine(Damage());
        }
    }
    

    void Update()
    {
        if (!triggerArmed)
        {
            triggerText.text = "Trigger Bomb Available";
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            if (canShoot)
            {
                Instantiate(timerBullet, transform.position + spawnDistance, Quaternion.identity);
                canShoot = false;
                StartCoroutine(Countdown(5));
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (!triggerArmed)
            {
                Instantiate(triggerBullet, transform.position + spawnDistance, Quaternion.identity);
                triggerArmed = true;
                triggerText.text = "Trigger Bomb Armed";
            }
        }
    }

    public IEnumerator Damage()
    {
        deathParticles.Play();
        health.TakeDamage(1);
        GetComponent<CharacterController>().enabled = false;
        yield return new WaitForSeconds(1.0f);
        transform.position = SpawnPos;
        GetComponent<CharacterController>().enabled = true;
    }
    
    public IEnumerator Countdown(int time)
    {
        if (time > 0)
        {
            countdown.text = time.ToString();
            yield return new WaitForSeconds(1);
            time--;
            StartCoroutine(Countdown(time));
        }

        else
        {
            countdown.text = "Can Drop Bomb";
            canShoot = true;
        }
    }
}
