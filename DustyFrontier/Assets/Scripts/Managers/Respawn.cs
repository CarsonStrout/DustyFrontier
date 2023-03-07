using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public GameObject player;
    public Transform respawnPos;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RespawnPlayer();
        }
    }

    void RespawnPlayer()
    {
        player.SetActive(false);
        player.transform.position = respawnPos.position;
        player.SetActive(true);
    }
}
