using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayGrid : MonoBehaviour
{
    [SerializeField] bool ShowGrid = false;
    [SerializeField] Color GridColor = Color.white;
    [SerializeField] float HeightLow = 0;
    [SerializeField] float HeightHigh = 100;
    [SerializeField] float LengthLow = 0;
    [SerializeField] float LengthHigh = 200;
    
    // Start is called before the first frame update
    private void OnDrawGizmos()
    {
        if (ShowGrid)
        {
            for (float i = HeightLow; i <=  HeightHigh; i += 10)
            {
                Debug.DrawLine(new Vector2(LengthLow, i), new Vector3(LengthHigh, i), GridColor);
            }
            for (float i = LengthLow; i <= LengthHigh; i += 18)
            {
                Debug.DrawLine(new Vector2(i, HeightLow), new Vector3(i, HeightHigh), GridColor);
            }
        }
    }
}
