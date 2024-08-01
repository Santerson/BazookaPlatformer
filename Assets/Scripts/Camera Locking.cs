using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLocking : MonoBehaviour
{
    [SerializeField] float xScrollSpeed = 15f;
    [SerializeField] float yScrollSpeed = 5f;
    Vector2 MaxRecoil;
    bool Scrolling = false;
    bool LLoop = false;
    bool RLoop = false;
    bool ULoop = false;
    bool DLoop = false;
    Vector2 ScrollingDirection = Vector2.zero;
    Rigidbody2D RefCameraRb = null;

    // Start is called before the first frame update
    void Start()
    {
        RefCameraRb = Camera.main.GetComponent<Rigidbody2D>();
        // Camera.main.transform.position = Vector3.zero;
        MaxRecoil = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //Getting the player's position and the camera's position (note the script is on the player)
        Vector3 playerPosition = transform.position;
        Vector3 cameraPosition = Camera.main.transform.position;

        //Calculating the x and y difference between the player and the camera (this is what will pull the
        //camera if the player is too far)
        float xDiff = playerPosition.x - cameraPosition.x;
        float yDiff = playerPosition.y - cameraPosition.y;

        //Calculating the player's highest recoil before screen scroll (this is to better latch the camera)
        float xRecoil = FindObjectOfType<Bazooka>().XRecoil;
        float yRecoil = FindObjectOfType<Bazooka>().YRecoil;
        if (Mathf.Abs(xRecoil) > MaxRecoil.x) MaxRecoil.x = Mathf.Abs(xRecoil); //We use Math.Abs as the player can have a positive or negative recoil
        if (Mathf.Abs(yRecoil) > MaxRecoil.y) MaxRecoil.y = Mathf.Abs(yRecoil);

            //We set these to 0 if the player's bazooka recoil hits 0 to prevent large jarring camera-snap-grid moments
        if (xRecoil == 0) MaxRecoil.x = 0;
        if (yRecoil == 0) MaxRecoil.y = 0;

        Vector2 movementForce = Vector2.zero;

        Debug.Log(xDiff + ", " + yDiff);

        //Calculate if the player is off screen, and if so, apply a force to pull the camera with the player
        if (yDiff > 5)
        {
            movementForce += Vector2.up;
            ScrollingDirection += Vector2.up;
            Scrolling = true;
        }
        else if (yDiff < -5)
        {
            movementForce += Vector2.down;
            ScrollingDirection += Vector2.down;
            Scrolling = true;

        }
        if (xDiff > 9.3)
        {
            movementForce += Vector2.right;
            ScrollingDirection += Vector2.right;
            Scrolling = true;

        }
        else if (xDiff < -9.3)
        {
            movementForce += Vector2.left;
            ScrollingDirection += Vector2.left;
            Scrolling = true;

        }

        movementForce.Normalize();
        movementForce = new Vector2(movementForce.x * xScrollSpeed, movementForce.y * yScrollSpeed);
        RefCameraRb.AddForce(movementForce);
        if (RefCameraRb.velocity.x > xScrollSpeed || (RefCameraRb.velocity.x != 0 && RefCameraRb.velocity.y == 0)) RefCameraRb.velocity = new Vector2(RefCameraRb.velocity.normalized.x * xScrollSpeed, RefCameraRb.velocity.y);
        if (RefCameraRb.velocity.y > yScrollSpeed || (RefCameraRb.velocity.y != 0 && RefCameraRb.velocity.x == 0)) RefCameraRb.velocity = new Vector2(RefCameraRb.velocity.x, RefCameraRb.velocity.normalized.y * yScrollSpeed);

        /*
        (This is a big if loop, so i will try my best to explain it)
        We check if the camera's position is in a position to latch, this being a multiple of 18 on the x and 10 on the y
        This is where the camera should stop so the camera is in a new, unexplored area.

        We use the max recoil here to add to the camera's latching area. This makes it less probable that the camera will miss
        the latch section due to moving too fast. However, to prevent the player from having a large screen jump if they are going
        slow, we multiply the Max Recoil variable to see if they are going fast (AKA using the bazooka). This allows the camera
        to latch a larger area if the player is moving fast, and vice versa.

        We also check if the player is not on the edges of the screen to where the screen, instead of being stopped, should
        be getting pulled.
        */
        if ((IsInRange(cameraPosition.x % 18.0f, -0.3f * (1 + MaxRecoil.x / 100), 0.3f * (1 + MaxRecoil.x / 100)) ||
            IsInRange(Mathf.Abs(cameraPosition.x) % 18.0f, 17.7f * (1 + MaxRecoil.x / 100), 18.1f * (1 + MaxRecoil.x / 100))) &&
            (IsInRange(cameraPosition.y % 10.0f, -0.5f * (1 + MaxRecoil.y / 100), 0.5f * (1 + MaxRecoil.y / 100)) ||
            IsInRange(Mathf.Abs(cameraPosition.y) % 10.0f, 9.5f * (1 + MaxRecoil.y / 100), 10.1f * (1 + MaxRecoil.y / 100))) &&
            xDiff < 9 && xDiff > -9 && yDiff < 5 && yDiff > -5)
        {
            LockTheCamera(cameraPosition);
            
        }

        if (Scrolling) CheckIfCameraOffset(cameraPosition, xDiff, yDiff);
    }

    public static bool IsInRange(float value, float low, float high)
    {
        return value >= low && value <= high;
    }

    private void CheckIfCameraOffset(Vector3 cameraPos, float xDiff, float yDiff)
    {
        

        //Debug.Log($"Camera % 18 = {Mathf.Abs(cameraPos.x % 18)}, LLoop = {LLoop} , RLoop = {RLoop}, ULoop = {ULoop} , DLoop = {DLoop}");
        if (ScrollingDirection.x > 0)
        {
            if (Mathf.Abs(cameraPos.x % 18) >= 15 && LLoop == false)
            {
                LLoop = true;
            }
            if (Mathf.Abs(cameraPos.x % 18) < 0.2 && LLoop == true)
            {
                Debug.LogWarning("The Camera is off center. Re-centering");
                LockTheCamera(cameraPos);
            }
        }
        if (ScrollingDirection.x < 0)
        {
            if (Mathf.Abs(cameraPos.x % 18) <= 5 && Mathf.Abs(cameraPos.x % 18) > 1 && RLoop == false)
            {
                RLoop = true;
            }
            if (Mathf.Abs(cameraPos.x % 18) > 17.8 && RLoop == true)
            {
                Debug.LogWarning("The Camera is off center. Re-centering");
                LockTheCamera(cameraPos);
            }
        }
        if (ScrollingDirection.y > 0)
        {
            if (Mathf.Abs(cameraPos.y % 10) >= 8 && ULoop == false)
            {
                ULoop = true;
            }
            if (Mathf.Abs(cameraPos.y % 10) < 0.2 && ULoop == true)
            {
                Debug.LogWarning("The Camera is off center. Re-centering");
                LockTheCamera(cameraPos);
            }
        }
        if (ScrollingDirection.y < 0)
        {
            if (Mathf.Abs(cameraPos.y % 10) <= 5 && Mathf.Abs(cameraPos.y % 10) > 1 && DLoop == false)
            {
                DLoop = true;
            }
            if (Mathf.Abs(cameraPos.y % 10) > 9 && DLoop == true)
            {
                Debug.LogWarning("The Camera is off center. Re-centering");
                LockTheCamera(cameraPos);
            }
        }
    }


    private void MoveCameraInRange()
    {
        float cameraX = Camera.main.transform.position.x;
        float cameraY = Camera.main.transform.position.y;
        if (cameraX % 18 != 0)
        {
            //Find which 'edge' is closer to the camera
            if (Mathf.Abs(cameraX % 18) < 9 || true)
            {
                //Skew the camera over one unit, repeating until it locks to a factor of 18
                while (cameraX % 18 != 0)
                {
                    //Skewing left
                    cameraX++;
                    Camera.main.transform.position = new Vector3(cameraX, cameraY, -10);
                }
            }
            else
            {
                while (cameraX % 18 != 0)
                {
                    //Skewing Right
                    cameraX--;
                    Camera.main.transform.position = new Vector3(cameraX, cameraY, -10);
                }
            }
        }
        if (cameraY % 10 != 0)
        {
            if (Mathf.Abs(cameraY % 10) > 5 || true)
            {
                while (cameraY % 10 != 0)
                {
                    //Skewing down
                    cameraY++;
                    Camera.main.transform.position = new Vector3(cameraX, cameraY, -10);
                }
            }
            else
            {
                while (cameraY % 10 != 0)
                {
                    cameraY--;
                    Camera.main.transform.position = new Vector3(cameraX, cameraY, -10);
                }
            }
        }
    }

    public void LockTheCamera(Vector3 cameraPos)
    {
        //Debug.Log("Locking");
        //Stops the camera and resets the max recoil
        MaxRecoil = Vector2.zero;
        RefCameraRb.velocity = Vector2.zero;
        LLoop = false;
        RLoop = false;
        ULoop = false;
        DLoop = false;
        Scrolling = false;
        ScrollingDirection = Vector2.zero;

        //Latches the camera to the nearest whole position.
        Camera.main.transform.position = new Vector3(Mathf.Floor(cameraPos.x), Mathf.Floor(cameraPos.y), -10);

        //Due to insane speed, the camera can technically go over 1 unit and get latched in an incorrect position,
        //usually one unit over or under. Due to this, we have a failsafe, skewing the camera back into place if
        //This is to happen
        MoveCameraInRange();
    }
}
