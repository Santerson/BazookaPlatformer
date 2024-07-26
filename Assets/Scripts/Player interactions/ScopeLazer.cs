using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScopeLazer : MonoBehaviour
{
    private void Awake()
    {
        if (transform.position != Vector3.zero)
        {
            Debug.LogError("The Scope should be located at position 0,0 to function correctly");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = FindObjectOfType<PlayerPhysicsController>().transform.position;
        Vector2 mouse = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, 10));
        //transform.position = FindObjectOfType<PlayerPhysicsController>().transform.position;
        //transform.localScale = new Vector3(mouseWorldPos.x - playerPos.x, mouseWorldPos.y - playerPos.y, 10);
        GetComponent<LineRenderer>().SetPosition(0, FindObjectOfType<Bazooka>().transform.position);
        GetComponent<LineRenderer>().SetPosition(1, mouseWorldPos);
    }
}
