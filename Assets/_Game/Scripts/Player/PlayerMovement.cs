using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;                // The speed that the player will move at
    public Joystick leftJoystick;           // Reference to the left joystick
    public PlayerShooting playerShooting;   // Reference to the player shooting script
    private Vector3 m_Movement;             // The vector to store the direction of the player's movement
    private Animator m_Anim;                // Reference to the animator component
    private Rigidbody m_Rigidbody;          // Reference to the player's rigidbody

    // Start is called before the first frame update
    void Start()
    {
        // Set up references
        m_Anim = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Store the input axes
        float h = leftJoystick.Horizontal;
        float v = leftJoystick.Vertical;

        // Move the player
        Move(h, v);

        // Animate the player
        Animating(h, v);
    }

    void Move(float h, float v)
    {
        // Set the movement vector based on the left joystick input
        m_Movement.Set(h, 0f, v);

        // Rotate to the movement direction
        if (playerShooting.isShooting == false) {
            m_Rigidbody.MoveRotation(Quaternion.LookRotation(m_Movement.normalized, Vector3.up));
        }

        //Normalize the movement vector and make it proportional to the speed per second
        m_Movement = m_Movement.normalized * speed * Time.deltaTime;

        // Move the player to it's current position plus the movement
        m_Rigidbody.MovePosition(transform.position + m_Movement);
    }

    void Animating(float h, float v)
    {
        // Create a bool that is true if either of the input axes is non-zero
        bool walking = h != 0f || v != 0f;

        // Tell the animator whether or not the player is walking
        m_Anim.SetBool("IsWalking", walking);
    }
}
