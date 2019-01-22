using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Joystick leftJoystick;               // Reference to the left joystick
    public Joystick rightJoystick;              // Reference to the right joystick
    public Transform gunpoint;                  // The position of the gunpoint for ray tracing and muzzle flash particle effect
    public float runSpeed = 6f;                 // The speed that the player will move at.
    public float firingRange = 50f;             // The range of which the player can shoot
    public float damage = 10f;                  // The damage for each shot

    private Rigidbody m_Rigidbody;              // Reference to the player's rigidbody
    private Vector3 m_Movement;                 // The vector to store the direction of the player's movement
    private Vector3 m_ShootDir;                 // The vector to store the direction of the player's shooting
    private Vector3 m_CurrLookDir;              // The vector to store the current direction of the player
    private Vector3 m_NewLookDir;               // The vector to store the new direction of the player 
    private Animator m_Anim;                    // Reference to the animator component
    private float m_HorizontalMove;             // Float to store the horizontal movement input
    private float m_VerticalMove;               // Float to store the vertical movement input
    private float m_HorizontalShoot;            // Float to store the horizontal shooting input
    private float m_VerticalShoot;              // Float to store the vertical shooting input

    // Start is called before the first frame update
    void Start()
    {
        // Set up references
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Store the input axes for movement and shooting
        m_HorizontalMove = leftJoystick.Horizontal;
        m_VerticalMove = leftJoystick.Vertical;

        m_HorizontalShoot = rightJoystick.Horizontal;
        m_VerticalShoot = rightJoystick.Vertical;
    }

    private void FixedUpdate()
    {
        // Set the movement vector based on the left joystick input
        m_Movement.Set(m_HorizontalMove, 0f, m_VerticalMove);

        // Set the shooting vector based on the right joystick input
        m_ShootDir.Set(m_HorizontalShoot, 0f, m_VerticalShoot);

        // Move the player to new rotation
        if (m_HorizontalShoot == 0 && m_VerticalShoot == 0) {
            m_NewLookDir.Set(m_HorizontalMove, 0f, m_VerticalMove);

            if (m_NewLookDir != m_CurrLookDir) {
                m_CurrLookDir = m_NewLookDir;
                m_Rigidbody.MoveRotation(Quaternion.LookRotation(m_CurrLookDir.normalized, Vector3.up));
            }
        }

        if (m_HorizontalShoot != 0 || m_VerticalShoot != 0) {
            Shoot(m_HorizontalShoot, m_VerticalShoot);

            m_NewLookDir.Set(m_HorizontalShoot, 0f, m_VerticalShoot);

            if (m_NewLookDir != m_CurrLookDir) {
                m_CurrLookDir = m_NewLookDir;
                m_Rigidbody.MoveRotation(Quaternion.LookRotation(m_CurrLookDir.normalized, Vector3.up));
            }
        }

        // Normalize the movement vector and make it proportional to the speed per second
        m_Movement = m_Movement.normalized * runSpeed * Time.deltaTime;

        // Move the player to it's current position plus the movement
        m_Rigidbody.MovePosition(transform.position + m_Movement);

        //Animate the player.
        Animating(m_HorizontalMove, m_VerticalMove, m_HorizontalShoot, m_VerticalShoot);

        if(m_HorizontalShoot != 0 || m_VerticalShoot != 0) {
            m_Anim.SetBool("IsFiring", true);
        } else {
            m_Anim.SetBool("IsFiring", false);
        }
    }

    void Shoot(float h, float v)
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(gunpoint.position, gunpoint.forward, out hitInfo, firingRange)) {
            Debug.Log(hitInfo.transform.name);
            Debug.DrawLine(gunpoint.position, hitInfo.transform.position);
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
