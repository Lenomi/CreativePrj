using System.Collections.Generic;
using UnityEngine;

public class PetMovement : MonoBehaviour
{
    private Animator m_animator = null;
    private Rigidbody m_rigidbody = null;

    [Header("Movement")]
    [SerializeField] private string m_horizontalAxis = "Horizontal";
    [SerializeField] private string m_verticalAxis = "Vertical";

    [Header("Movement Variables")]
    [SerializeField] private float m_turnSpeed = 100.0f;
    [SerializeField] private float m_moveSpeed = 2.0f;
    [SerializeField] private float m_jumpForce = 4.0f;

    private bool m_isGrounded = false;
    private bool m_jumpInput = false;
    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;

    private readonly float m_interpolation = 10.0f;
    private readonly float m_walkScale = 0.33f;
    private readonly float m_backwardsWalkScale = 0.25f;

    private List<Collider> m_collisions = new List<Collider>();

    private float m_currentV = 0;
    private float m_currentH = 0;

    private enum PetAnimationState { Idle, Eating, Sitting, Sleeping, Lying }
    private PetAnimationState m_animationState = PetAnimationState.Idle;

    private readonly string m_animationEating = "Eating";
    private readonly string m_animationSitting = "Sitting";
    private readonly string m_animationSleeping = "Sleeping";
    private readonly string m_animationLying = "Lying";

    [Header("Animation Input")]
    [SerializeField] private KeyCode m_walkKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode m_jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode m_sitKey = KeyCode.Z;
    [SerializeField] private KeyCode m_eatKey = KeyCode.X;
    [SerializeField] private KeyCode m_sleepKey = KeyCode.C;
    [SerializeField] private KeyCode m_lieKey = KeyCode.V;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        AnimationInput();

        if (!m_jumpInput && Input.GetKey(m_jumpKey))
        {
            m_jumpInput = true;
        }
    }

    private void FixedUpdate()
    {
        m_animator.SetBool("Grounded", m_isGrounded);

        MovementUpdate();
        JumpingAndLanding();
        m_jumpInput = false;

        m_collisions.Clear();
    }

    private void MovementUpdate()
    {
        float v = Input.GetAxis(m_verticalAxis);
        float h = Input.GetAxis(m_horizontalAxis);

        bool walk = Input.GetKey(m_walkKey);

        if (v < 0.0f)
        {
            v *= m_backwardsWalkScale;
        }
        else if (walk)
        {
            v *= m_walkScale;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        transform.position += transform.forward * m_currentV * m_moveSpeed * Time.deltaTime;
        transform.Rotate(0, m_currentH * m_turnSpeed * Time.deltaTime, 0);

        m_animator.SetFloat("MoveSpeed", m_currentV);
    }

    private void AnimationInput()
    {
        PetAnimationState newState = PetAnimationState.Idle;

        if (Input.GetKeyDown(m_eatKey))
        {
            newState = PetAnimationState.Eating;
        }
        else if (Input.GetKeyDown(m_sitKey))
        {
            newState = PetAnimationState.Sitting;
        }
        else if (Input.GetKeyDown(m_sleepKey))
        {
            newState = PetAnimationState.Sleeping;
        }
        else if (Input.GetKeyDown(m_lieKey))
        {
            newState = PetAnimationState.Lying;
        }

        if (newState != PetAnimationState.Idle)
        {
            if (newState != m_animationState)
            {
                DisableAnimationLoops();
                switch (newState)
                {
                    case PetAnimationState.Eating:
                        m_animator.SetBool(m_animationEating, true);
                        break;
                    case PetAnimationState.Sitting:
                        m_animator.SetBool(m_animationSitting, true);
                        break;
                    case PetAnimationState.Sleeping:
                        m_animator.SetBool(m_animationSleeping, true);
                        break;
                    case PetAnimationState.Lying:
                        m_animator.SetBool(m_animationLying, true);
                        break;
                    default:
                        break;
                }
                m_animationState = newState;
            }
            else
            {
                DisableAnimationLoops();
            }
        }
    }

    private void DisableAnimationLoops()
    {
        m_animationState = PetAnimationState.Idle;
        m_animator.SetBool(m_animationEating, false);
        m_animator.SetBool(m_animationSitting, false);
        m_animator.SetBool(m_animationSleeping, false);
        m_animator.SetBool(m_animationLying, false);
    }

    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

        if (jumpCooldownOver && m_isGrounded && m_jumpInput)
        {
            m_jumpTimeStamp = Time.time;
            m_rigidbody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider))
                {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if (validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        }
        else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }

}
