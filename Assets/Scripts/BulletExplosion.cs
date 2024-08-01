using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class BulletExplosion : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Breakable") Destroy(collision.gameObject);
    }

}
