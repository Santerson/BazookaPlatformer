using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazookaBullet : MonoBehaviour
{
    private Vector2 target;
    [Tooltip("The speed of the bullet. The higher the number, the faster. (Should be like < 1)")]
    [SerializeField] float BulletSpeed = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        //TODO: make the bullet continue even further than the cursor
        Vector3 mouse = Input.mousePosition;
        target = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, 10));
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {

        Vector2 position = transform.position;
        Vector2 vecToTarget = new Vector2(target.x - position.x, target.y - position.y);
        vecToTarget.Normalize();
        vecToTarget *= BulletSpeed;
        transform.position = new Vector2(position.x + vecToTarget.x, position.y + vecToTarget.y);
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.name != "BEAN" && collision.name != "Bazooka") Destroy(gameObject);
    }
}
