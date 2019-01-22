using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public int damage = 20;                     // The damage inflicted by each bullet
    public float timeBetweenBullets = 0.15f;    // The time between each shot
    public float range = 100f;                  // The distance the gun can fire
    public Joystick rightJoystick;              // Reference to the right joystick
    public bool isShooting = false;             // Is the player current shooting
    public Rigidbody playerRigidbody;           // Refence to the player rigidbody
    
    private float m_Timer;                      // A timer to determine when to fire
    private Ray m_ShootRay;                     // A ray from the gun end forwards
    private RaycastHit m_ShootHit;              // A raycast hit to get information about what was hit
    private int m_ShootableMask;                // A layer mask so the raycast only hits things on the shootable layer
    private ParticleSystem m_GunParticles;      // Reference to the particle system
    private LineRenderer m_GunLine;             // Reference to the line renderer
    private AudioSource m_GunAudio;             // Reference to the audio source
    private Light m_GunLight;                   // Reference to the light component
    private float m_EffectsDisplayTime = 0.2f;  // The proportion of the timeBetweenBullets that the effect will display for
    private Vector3 m_ShootDir;                 // Vector to store the direction of shooting
    
    // Start is called before the first frame update
    void Start()
    {
        m_ShootableMask = LayerMask.GetMask("Shootable");

        // Set up the references
        m_GunParticles = GetComponent<ParticleSystem>();
        m_GunLine = GetComponent<LineRenderer>();
        m_GunAudio = GetComponent<AudioSource>();
        m_GunLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        // Add the time since Update was last called to the timer
        m_Timer += Time.deltaTime;

        // If input on the right joystick and it's time to fire...
        if((rightJoystick.Horizontal != 0 || rightJoystick.Vertical != 0) && m_Timer >= timeBetweenBullets) {
            float h = rightJoystick.Horizontal;
            float v = rightJoystick.Vertical;

            // ... shoot the gun
            Shoot(h, v);
        }

        if(rightJoystick.Horizontal != 0 || rightJoystick.Vertical != 0) {
            isShooting = true;
        } else {
            isShooting = false;
        }

        // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
        if(m_Timer >= timeBetweenBullets * m_EffectsDisplayTime) {
            // ... disable the effects
            DisableEffects();
        }
    }

    public void DisableEffects()
    {
        m_GunLine.enabled = false;
        m_GunLight.enabled = false;
    }

    void Shoot(float h, float v)
    {
        // Set the shooting direction vector based on right joystick input
        m_ShootDir.Set(h, 0f, v);

        // Rotate to the shooting direction
        playerRigidbody.MoveRotation(Quaternion.LookRotation(m_ShootDir.normalized, Vector3.up));
        
        // Reset the timer
        m_Timer = 0f;

        // Play the gun shot audioclip
        m_GunAudio.Play();

        // Enable the light
        m_GunLight.enabled = true;

        // Stop the particles from playing if they were, then start the particles
        m_GunParticles.Stop();
        m_GunParticles.Play();

        // Enable the line renderer and set it's first position to be the end of the gun
        m_GunLine.enabled = true;
        m_GunLine.SetPosition(0, transform.position);

        // Set the shoot ray so that it starts at the end of the gun and points forward from the barrel
        m_ShootRay.origin = transform.position;
        m_ShootRay.direction = transform.forward;

        // Perform the raycast against gameobjects on the shootable layer and if it hits something...
        if(Physics.Raycast(m_ShootRay, out m_ShootHit, range, m_ShootableMask)) {
            // Hit something and deal damage to it
        }
        else {
            // Didn't hit anything set the second position of the line renderer to the fullest extent of the gun's range
            m_GunLine.SetPosition(1, m_ShootRay.origin + m_ShootRay.direction * range);
        }
    }
}
