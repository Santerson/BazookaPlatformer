using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potatoify : MonoBehaviour
{
    [Tooltip("So thers all those people nowadays that are so obsessed with high frame rates. You and i know that we need to embrace powerpoint.")]
    [SerializeField] bool potatoify = false;
    [SerializeField] float PotatoifyBase = 1000000;
    [SerializeField] float PotatoifyIntensity = 1.0f;

    // Update is called once per frame
    void Update()
    {
        if (potatoify)
        {
            //lowering fps intentionally 
            for (int i = 0; i < PotatoifyBase * PotatoifyIntensity; i++)
            {
                Mathf.Sqrt(5);
            }
        }
    }
}
