using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Bazooka : MonoBehaviour
{
    [Tooltip("The base launch speed")]
    [SerializeField] float Recoil = 10;
    [Tooltip("The rate at which the player slows down after a launch")]
    [SerializeField] float RecoilDecay = 0.3f;
    [SerializeField] float HorizontalLaunchMultiplier = 1f;
    [SerializeField] float VerticalLaunchMultiplier = 1f;
    [Tooltip("The particle system for the shot particles")]
    [SerializeField] ParticleSystem ShootParticles;
    [Tooltip("You can't commit arson. Okay? At least not that fast.")]
    [SerializeField] float ShootCooldown = 3f;
    [Tooltip("When the player moves their mouse, the bazooka will turn to face the mouse. This determines how fast that rotation is.")]
    [SerializeField] float RotateSpeed = 100f;
    //[SerializeField] TextMeshProUGUI ReloadText;
    //float ShotReadyIn = 0f;
    public float XRecoil = 0;
    public float YRecoil = 0;
    // Start is called before the first frame update

    private void Awake()
    {
        if (ShootParticles == null) Debug.LogError("No shootParticles found! (Check the bazooka serializeFields!)");
        //if (ReloadText == null) Debug.LogError("No ReloadText found! (Check bazooka serializeFields!)");

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Getting mouse world position
        Vector2 mouse = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, 10));
        
        //Reducing shot cooldown
        //shootCooldown();

        //Checking if mouse is down
        if (Input.GetMouseButtonDown(0)  /*ShotReadyIn <= 0*/)
        {
            //ShotReadyIn = ShootCooldown;
            CreateRecoil(-mouseWorldPosition); //We use negative to launch the player the opposite direciton
        }
        ApplyRecoil();
        RotateToMouse(mouseWorldPosition);
        

    }

    void CreateRecoil(Vector3 mousePos)
    {
        //Get the x and y cordinates of the mouse
        float mouseXPos = mousePos.x;
        float mouseYPos = mousePos.y;
        float playerXPos = transform.position.x;
        float playerYPos = transform.position.y;
        //Debug.Log($"Player position = {playerXPos},{playerYPos}");
        Vector2 relativeMousePosToPlayer = new Vector2(mouseXPos + playerXPos - 0.4f, mouseYPos + playerYPos);
        relativeMousePosToPlayer = -relativeMousePosToPlayer;
        //Debug.Log($"MouseRelativePosition = {relativeMousePosToPlayer.x},{relativeMousePosToPlayer.y}");
        XRecoil = (relativeMousePosToPlayer.normalized.x * -Recoil) * HorizontalLaunchMultiplier;
        YRecoil = (relativeMousePosToPlayer.normalized.y * -Recoil) * VerticalLaunchMultiplier;
        ShootParticles.Play();
    }

    void ApplyRecoil()
    {
        FindObjectOfType<PlayerPhysicsController>().GetComponent<Rigidbody2D>().AddForce(new Vector2(XRecoil, YRecoil));
        float frameDiff = Time.deltaTime * 100;
        if (XRecoil > 0) { XRecoil -= RecoilDecay * frameDiff; }
        if (XRecoil < 0) { XRecoil += RecoilDecay * frameDiff; }
        if (Mathf.Abs(XRecoil) < RecoilDecay) {XRecoil = 0;}
        if (YRecoil > 0) { YRecoil -= RecoilDecay * 3 * frameDiff; }
        if (YRecoil < 0) { YRecoil += RecoilDecay * 3 * frameDiff;}
        if (Mathf.Abs(YRecoil) < RecoilDecay * 3) {YRecoil = 0;}
        
    }

    /// <summary>
    /// Rotates the bazooka to face the mouse
    /// </summary>
    private void RotateToMouse(Vector3 mouseWorldPosition)
    {
        float angle = Mathf.Atan2(mouseWorldPosition.y - transform.position.y, mouseWorldPosition.x - transform.position.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, RotateSpeed);

        //Debug.Log(transform.eulerAngles);
        if (mouseWorldPosition.x > transform.position.x - 0.5f && mouseWorldPosition.x < transform.position.x + 0.5f) { }
        else if (transform.eulerAngles.z > 90 && transform.eulerAngles.z < 270)
        {
            //Debug.Log("Moving");
            Vector3 playerPos = FindObjectOfType<PlayerPhysicsController>().transform.position;
            transform.position = new Vector3(playerPos.x - 0.5f, playerPos.y, 0);
        }
        
        else
        {
            //Debug.Log("Moving");
            Vector3 playerPos = FindObjectOfType<PlayerPhysicsController>().transform.position;
            transform.position = new Vector3(playerPos.x + 0.5f, playerPos.y, 0);
        }
    }

   /*
    private void shootCooldown()
    {
        if (ReloadText == null) return;
        //if (ShotReadyIn > 0f) ShotReadyIn -= Time.deltaTime;
        ReloadText.color = Color.red;
        if (ShotReadyIn <= 0f)
        {
            ReloadText.color = Color.green;
        }
        ReloadText.text = $"Reloading: {ShotReadyIn}s left";
        
        
    }
    */
}
