using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Joystick leftJoystick;               // Reference to the left joystick
    public Joystick rightJoystick;              // Reference to the right joystick
    public Transform gunpoint;                  // The position of the gunpoint for ray tracing and muzzle flash particle effect
    public bool isShooting = false;             // Bool to see if the player is currently shooting or not
    public float runSpeed = 6f;                 // The speed that the player will move at.
    public float firingRange = 50f;             // The range of which the player can shoot
    public int damage = 20;                     // The damage for each shot
    public float timeBetweenBullets = 0.15f;    // The time between each shot
    public ParticleSystem m_HitParticles;       // Reference to the particle system for bullet hits
    public ParticleSystem m_GunParticles;       // Reference to the particle system for muzzle flash
    public LineRenderer m_GunLine;              // Reference to the line renderer
    public AudioSource m_GunAudio;              // Reference to the audio source
    public Light m_GunLight;                    // Reference to the light component

    private float m_Timer;                      // A timer to detmine when to fire
    private Ray m_ShootRay;                     // A ray from the gun end forwards
    private RaycastHit m_ShootHit;              // A raycast hit to get information about what was hit
    private float m_EffectsDisplayTime = 0.2f;  // The proportion of the timeBetweenBullets that the effect will display for
    private Rigidbody m_Rigidbody;              // Reference to the player's rigidbody
    private Vector3 m_Movement;                 // The vector to store the direction of the player's movement
    private Vector3 m_ShootDir;                 // The vector to store the direction of the player's shooting
    private Animator m_Anim;                    // Reference to the animator component

    // Start is called before the first frame update
    void Start()
    {
        // Set up references
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Anim = GetComponent<Animator>();
        m_GunParticles.Stop();
        m_HitParticles.Stop();
        m_GunLight.enabled = false;
        m_GunLine.enabled = false;
    }

    private void Update()
    {
        // Add the time since Update was last called to the timer
        m_Timer += Time.deltaTime;        

        // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for
        if(m_Timer >= timeBetweenBullets * m_EffectsDisplayTime) {
            DisableEffects();
        }
    }

    private void FixedUpdate()
    {
        // Store the input axes for movement and shooting
        float hMove = leftJoystick.Horizontal;
        float vMove = leftJoystick.Vertical;

        float hShoot = rightJoystick.Horizontal;
        float vShoot = rightJoystick.Vertical;

        if (hMove != 0 || vMove != 0) {
            // Move the player
            Move(hMove, vMove);
        }
                
        // Set isShooting if the player is shooting
        if (hShoot != 0 || vShoot != 0) {
            isShooting = true;

            if (m_Timer >= timeBetweenBullets) {
                // Shoot the gun
                Shoot(hShoot, vShoot);
            }
        } else {
            isShooting = false;
        }        

        // Rotate the player
        Turning();

        // Animate the player.
        Animating(hMove, vMove, hShoot, vShoot);
    }

    public void DisableEffects()
    {
        m_GunLine.enabled = false;
        m_GunLight.enabled = false;
    }

    void Move(float h, float v)
    {
        // Set the movement vector based on the left joystick input
        m_Movement.Set(h, 0f, v);

        // Normalize the movement vector and make it proportional to the speed per second
        m_Movement = m_Movement.normalized * runSpeed * Time.deltaTime;

        // Move the player to it's current position plus the movement
        m_Rigidbody.MovePosition(transform.position + m_Movement);
    }

    void Shoot(float h, float v)
    {
        // Set the shooting vector based on the right joystick input
        m_ShootDir.Set(h, 0f, v);

        // Reset the timer
        m_Timer = 0f;

        // Play the gun shot audioclip
        m_GunAudio.Play();

        // Enable the light
        m_GunLight.enabled = true;

        // Stop the partibles from playing if they were, then start the particles
        m_GunParticles.Stop();
        m_GunParticles.Play();

        // Enable the line renderer and set it's first position to the end of the gun
        //m_GunLine.enabled = true;
        //m_GunLine.SetPosition(0, gunpoint.position);

        // Set the shoot ray so that it starts at the end of the gun and points forward from the barrel
        m_ShootRay.origin = gunpoint.position;
        m_ShootRay.direction = m_ShootDir;

        // Perform the raycast
        if (Physics.Raycast(m_ShootRay, out m_ShootHit, firingRange)) {
            // Hit something, deal damage
            Debug.Log(m_ShootHit.transform.name + "-" + m_ShootHit.transform.position);

            // Not working
            m_HitParticles.transform.position = m_ShootHit.transform.position;
            m_HitParticles.Play(true);
        } else {
            m_GunLine.SetPosition(1, m_ShootRay.origin + m_ShootRay.direction * firingRange);
        }
    }

    void Turning()
    {
        if (isShooting) {
            m_Rigidbody.MoveRotation(Quaternion.LookRotation(m_ShootDir.normalized, Vector3.up));
        } else {
            m_Rigidbody.MoveRotation(Quaternion.LookRotation(m_Movement.normalized, Vector3.up));
        }
    }

    void Animating(float h, float v, float h2, float v2)
    {
        // Create a boolean that is true if either the input axes is non-zero
        bool walking = h != 0f || v != 0f;

        // Tell the animator whether or not the player is walking.
        m_Anim.SetBool("IsWalking", walking);
    }
}
