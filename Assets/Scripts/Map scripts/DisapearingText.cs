using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisapearingText : MonoBehaviour
{
    [Tooltip("This is the area where the object will appear. MUST BE THE BOTTOM LEFT TO FUNCTION PROPERLY. Set both to the same to make the text appear constantly")]
    [SerializeField] Vector2 AppearBottomLeft;
    [Tooltip("This is the area where the object will appear. MUST BE THE TOP RIGHT TO FUNCTION PROPERLY. Set both to the same to make the text appear constantly")]
    [SerializeField] Vector2 AppearTopRight;
    [Tooltip("This is the area where the object will disapear. MUST BE THE BOTTOM LEFT TO FUNCTION PROPERLY")]
    [SerializeField] Vector2 DisappearBottomLeft;
    [Tooltip("This is the area where the object will disapear. MUST BE THE TOP RIGHT TO FUNCTION PROPERLY")]
    [SerializeField] Vector2 DisappearTopRight;
    
    bool fading = false;
    bool appearing = false;
    bool fadedIn = false;
    TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        if (text == null)
        {
            Debug.LogError("No text found on text object!");
        }
        if (AppearBottomLeft != AppearTopRight)
        {
            text.color = new Color (text.color.r, text.color.g, text.color.b, 0);
        }
        else
        {
            fadedIn = true;
        }
        if (AppearBottomLeft.x > AppearTopRight.x || AppearBottomLeft.y > AppearTopRight.y)
        {
            Debug.LogError("AppearBottomLeft is greater than AppearTopRight. The collisions for this text object will not work.");
        }
        if (DisappearBottomLeft.x > DisappearTopRight.x || DisappearBottomLeft.y > DisappearTopRight.y)
        {
            Debug.LogError("DisappearBottomLeft is greater than DisappearTopRight. The collisions for this text object will not work.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        DebugExtensions.DrawBox(DisappearBottomLeft, DisappearTopRight, Color.red);
        DebugExtensions.DrawBox(AppearBottomLeft, AppearTopRight, Color.green);
    }

    // Update is called once per frame
    void Update()
    {
        if (SUtilities.IsInRange(FindObjectOfType<PlayerPhysicsController>().transform.position, DisappearBottomLeft, DisappearTopRight))
        {
            fading = true;
        }
        if (SUtilities.IsInRange(FindObjectOfType<PlayerPhysicsController>().transform.position, AppearBottomLeft, AppearTopRight) && !fadedIn)
        {
            appearing = true;
        }
    }

    private void FixedUpdate()
    {
        if (fading) { FadeOut(); }
        if (appearing && !fadedIn) { FadeIn(); }
    }

    void FadeOut()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - 0.05f);
        if (text.color.a <= 0)
        {
            Destroy(gameObject);
        }
    }

    void FadeIn()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + 0.05f);
        if (text.color.a >= 1)
        {
            fadedIn = true;
        }
    }
}
