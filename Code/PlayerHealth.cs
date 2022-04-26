using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    public Canvas myCanvas;
    public GameObject heart;
    public Transform heartContainer;
    
    void Start()
    {
        currentHealth = maxHealth;
        
        for (int i = 0; i < currentHealth; i++)
        {
            GameObject newHeart = Instantiate(heart, new Vector3(heartContainer.position.x - (i*0.4f), heartContainer.position.y, heartContainer.position.z), quaternion.identity);
            newHeart.transform.SetParent(heartContainer);
        }
    }

    void Update()
    {
        Quaternion cameraRotation = Camera.main.transform.rotation;
        cameraRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);
        myCanvas.transform.rotation = cameraRotation;
    }

    void UpdateHealth()
    {
        foreach (Transform heart in heartContainer)
        {
            GameObject.Destroy(heart.gameObject);
        }
        
        for (int i = 0; i < currentHealth; i++)
        {
            GameObject newHeart = Instantiate(heart, new Vector3(heartContainer.position.x - (i*0.4f), heartContainer.position.y, heartContainer.position.z), quaternion.identity);
            newHeart.transform.SetParent(heartContainer);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        UpdateHealth();
    }
}
