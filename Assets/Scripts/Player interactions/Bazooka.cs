using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Bazooka : MonoBehaviour
{
    [SerializeField] GameObject BulletPrefab;
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
    [SerializeField] TextMeshProUGUI ReloadText;
    [Tooltip("Whether or not the player starts with the bazooka, (good for debugging, should be off by default)")]
    [SerializeField] bool PlayerHasBazooka = false;
    [SerializeField] float StrongFireThreshold = 0.4f;
    [SerializeField] float StrongFireMultiplier = 2f;

    [SerializeField] AudioSource SFXShoot;
    [SerializeField] AudioSource SFXStrongShoot;

    float TimeSinceLastShot = 0f;
    float ShotReadyIn = 0f;
    public float XRecoil = 0;
    public float YRecoil = 0;
    // Start is called before the first frame update

    private void Awake()
    {
        if (ShootParticles == null) Debug.LogError("No shootParticles found! (Check the bazooka serializeFields!)");
        if (ReloadText == null) Debug.LogError("No ReloadText found! (Check bazooka serializeFields!)");

    }

    // Update is called once per frame
    void Update()
    {
        //Getting mouse world position
        Vector2 mouse = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, 10));
        
        //Reducing shot cooldown
        shootCooldown();
        TimeSinceLastShot += Time.deltaTime;

        //Checking if mouse is down, shot is ready, player has the bazooka, and the cursor isn't over the player
        if (Input.GetMouseButtonDown(0) && ShotReadyIn <= 0 && PlayerHasBazooka && !SUtilities.IsInRange(FindObjectOfType<Crossair>().transform.position,
            new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f), new Vector2(transform.position.x + 0.5f, transform.position.y + 0.5f)))
        {
            if (TimeSinceLastShot  < StrongFireThreshold + ShootCooldown)
            {
                PlaySFX(SFXStrongShoot);
                CreateRecoil(-mouseWorldPosition, HorizontalLaunchMultiplier * StrongFireMultiplier, VerticalLaunchMultiplier * StrongFireMultiplier); //We use negative to launch the player the opposite direciton
            }
            else
            {
                PlaySFX(SFXShoot);
                CreateRecoil(-mouseWorldPosition, HorizontalLaunchMultiplier, VerticalLaunchMultiplier); //We use negative to launch the player the opposite direciton
            }
            ShotReadyIn = ShootCooldown;
            TimeSinceLastShot = 0;
            Instantiate(BulletPrefab, transform.position, Quaternion.identity);
        }
        ApplyRecoil();
        RotateToMouse(mouseWorldPosition);
        

    }

    void CreateRecoil(Vector3 mousePos, float HorizontalLaunchMultiplier, float VerticalLaunchMultiplier)
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
        Debug.Log("XRecoil: " + XRecoil + ", YRecoil" + YRecoil);
        float frameDiff = Time.deltaTime * 300;
        FindObjectOfType<PlayerPhysicsController>().GetComponent<Rigidbody2D>().AddForce(new Vector2(XRecoil * frameDiff, YRecoil * frameDiff));

        if (XRecoil > 0) { 
            XRecoil -= RecoilDecay * frameDiff;
            if (XRecoil < 0)
            {
                XRecoil = 0;
            }
        }
        if (XRecoil < 0) { 
            XRecoil += RecoilDecay * frameDiff ;
            if (XRecoil > 0)
            {
                XRecoil = 0;
            }
        }
        if (Mathf.Abs(XRecoil) < RecoilDecay) {XRecoil = 0;}
        if (YRecoil > 0) { 
            YRecoil -= RecoilDecay * 3 * frameDiff;
            if (YRecoil < 0)
            {
                YRecoil = 0;
            }
        }
        if (YRecoil < 0) { 
            YRecoil += RecoilDecay * 3 * frameDiff;
            if (YRecoil > 0)
            {
                YRecoil = 0;
            }
        }
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

    private void shootCooldown()
    {
        if (ReloadText == null) return;
        if (ShotReadyIn > 0f) ShotReadyIn -= Time.deltaTime;
        ReloadText.color = Color.red;
        if (ShotReadyIn <= 0f)
        {
            ReloadText.color = Color.green;
        }
        ReloadText.text = $"Reloading: {ShotReadyIn:#0.0}s left";
        
        
    }

    public void ActivateBazooka()
    {
        PlayerHasBazooka = true;
    }

    /// <summary>
    /// Plays a sound effect
    /// </summary>
    /// <param name="SFX">The sound effect (duh)</param>
    private void PlaySFX(AudioSource SFX)
    {
        try
        {
            SFX.Play();
        }
        catch
        {
            Debug.LogError("No Audio Source found!");
        }
    }
}
