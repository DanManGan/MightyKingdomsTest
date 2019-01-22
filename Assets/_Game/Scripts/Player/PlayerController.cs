using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Joystick leftJoystick;               // Reference to the left joystick
    public Joystick rightJoystick;              // Reference to the right joystick
    public Transform gunpoint;                  // The position of the gunpoint for ray tracing and muzzle flash particle effect
    public float runSpeed = 6f;                 // The speed that the player will move at.

    private Rigidbody m_Rigidbody;              // Reference to the player's rigidbody
    private Vector3 m_Movement;                 // The vector to store the direction of the player's movement
    private Vector3 m_ShootDir;                 // The vector to store the direction of the player's shooting
    private Animator m_Anim;                    // Reference to the animator component
    private int m_FloorMask;                    // A layer mask so that a ray can be case just at gameobjects on the floor layer
    private float m_CamRayLength = 100f;        // The length of the ray from the camera into the scene

    // Start is called before the first frame update
    void Start()
    {
        // Create a layer mask for the floor layer
        m_FloorMask = LayerMask.GetMask("Floor");

        // Set up references
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        // Store the input axes
        float horizontalMove = leftJoystick.Horizontal;
        float verticalMove = leftJoystick.Vertical;

        float horizontalShoot = rightJoystick.Horizontal;
        float verticalShoot = rightJoystick.Vertical;

        // Move the player around the scene
        Move(horizontalMove, verticalMove);

        //Animate the player.
        Animating(horizontalMove, verticalMove);
    }

    void Move(float h, float v)
    {
        // Set the movement vector based on the axis input
        m_Movement.Set(h, 0f, v);

        // Move the player to new rotation
        m_Rigidbody.MoveRotation(Quaternion.LookRotation(m_Movement.normalized, Vector3.up));

        // Normalize the movement vector and make it proportional to the speed per second
        m_Movement = m_Movement.normalized * runSpeed * Time.deltaTime;

        // Move the player to it's current position plus the movement
        m_Rigidbody.MovePosition(transform.position + m_Movement);

    }

    void Animating(float h, float v)
    {
        // Create a boolean that is true if either the input axes is non-zero
        bool walking = h != 0f || v != 0f;

        // Tell the animator whether or not the player is walking.
        m_Anim.SetBool("IsWalking", walking);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
