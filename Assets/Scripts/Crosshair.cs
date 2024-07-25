using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossair : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouse = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, 10));
        transform.position = mouseWorldPos;
    }
}
