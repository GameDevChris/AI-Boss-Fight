using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Bullet : MonoBehaviour
{
    public enum BombType
    {
        Timer,
        Trigger
    }

    public BombType myType;
    
    public Transform nibPos;
    public LineRenderer sight;
    public ParticleSystem explosion;
    public Canvas CountdownCanvas;
    public TextMeshProUGUI countdown;
    public PlayerBehaviour player;
    
    public int explosionTime = 5;

    void Start()
    {
        player = FindObjectOfType<PlayerBehaviour>();
    }
    
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            sight.SetPosition(0, nibPos.position);
            sight.SetPosition(1, hit.point);
        }
        
        if (myType == BombType.Timer)
        {
            Quaternion cameraRotation = Camera.main.transform.rotation;
            cameraRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);
            CountdownCanvas.transform.rotation = cameraRotation;
        }

        if (myType == BombType.Trigger)
        {
            if (Input.GetMouseButtonDown(1))
            {
                explosion.Play();
                player.triggerArmed = false;
                Destroy(gameObject, 2f);
            }
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (myType == BombType.Timer)
        {
            StartCoroutine(Countdown(explosionTime));
        }
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
            countdown.text = "Bomb time";
            explosion.Play();
            Destroy(gameObject, 2f);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("TeleportPlane"))
        {
            if (myType == BombType.Timer)
            {
                Destroy(gameObject);
            }

            if (myType == BombType.Trigger)
            {
                player.triggerArmed = false;
                Destroy(gameObject);
            }
        }
    }
}
