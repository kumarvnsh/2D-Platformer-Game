using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public BoxCollider2D playerCollider;
    private Vector2 originalColliderSize;
    private Vector2 originalCollideroffset;
    public float crouchHeight = 0.5f;
    public float offset;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private bool isGrounded;
    public string SceneName;

    void Start()
    {
        originalColliderSize = playerCollider.size;
        originalCollideroffset = playerCollider.offset;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float speed = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(speed));
        Vector3 scale = transform.localScale;
        if (speed < 0)
        {
            scale.x = -1f * Mathf.Abs(scale.x);
        }
        else if (speed > 0)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        transform.localScale = scale;

        if (speed != 0)
        {
            transform.Translate(Vector3.right * speed * moveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            animator.SetBool("isCrouching", true);
            playerCollider.size = new Vector2(originalColliderSize.x, crouchHeight);
            playerCollider.offset = new Vector2(originalCollideroffset.x, offset);
        }
        else
        {
            animator.SetBool("isCrouching", false);
            playerCollider.size = originalColliderSize;
            playerCollider.offset = originalCollideroffset;
        }

        if (isGrounded && Input.GetAxisRaw("Vertical") > 0)
        {
            animator.SetBool("Jump", true);
            rb.velocity = new Vector2(rb.velocity.x, 0f); // Reset Y-velocity before jump
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            isGrounded = false;
        }

        if (isGrounded)
        {
            animator.SetBool("Jump", false);
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Win"))
        {
            SceneManager.LoadScene(SceneName);
        }
    }
}
