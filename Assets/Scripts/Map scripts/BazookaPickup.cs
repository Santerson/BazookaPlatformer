using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazookaPickup : MonoBehaviour
{
    Bazooka PlayerBazooka;
    private void Awake()
    {
         PlayerBazooka = FindObjectOfType<Bazooka>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerBazooka.ActivateBazooka();
        Destroy(gameObject);
    }
}
