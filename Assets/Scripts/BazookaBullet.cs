using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazookaBullet : MonoBehaviour
{
    private Vector3 target;
    [Tooltip("The speed of the bullet. The higher the number, the faster. (Should be like < 1)")]
    [SerializeField] float BulletSpeed = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        //TODO: make the bullet continue even further than the cursor
        Vector3 mouse = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, 10));
        Vector2 bazookaPos = FindObjectOfType<Bazooka>().transform.position;
        Vector2 relativeMouseToBazookaPos = new Vector2(mouseWorldPos.x - bazookaPos.x, mouseWorldPos.y - bazookaPos.y);
        target = relativeMouseToBazookaPos.normalized;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        transform.position += target * BulletSpeed;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name != "BEAN" && collision.name != "Bazooka")
        {
            GameObject.Find("ExplosionParticles").GetComponent<ExplosionParticles>().PlayParticles(transform.position);
            Destroy(gameObject);
        }
    }
}
