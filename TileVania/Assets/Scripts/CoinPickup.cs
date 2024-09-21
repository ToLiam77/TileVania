using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] AudioClip coinPickupSFX;
    [SerializeField] int pointsPerCoin = 100;

    bool wasCollected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !wasCollected)
        {
            wasCollected = true;
            FindObjectOfType<GameSession>().IncreaseScore(pointsPerCoin);
            AudioSource.PlayClipAtPoint(coinPickupSFX, Camera.main.transform.position); 
            Destroy(gameObject);
        }
    }
}
