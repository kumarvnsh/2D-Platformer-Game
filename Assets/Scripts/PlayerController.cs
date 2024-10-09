using UnityEngine;
using System.Collections;
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
    public float fallDamageThreshold = -10f; // The speed at which fall damage starts
    public int maxHealth = 100;
    private int currentHealth;
    private float lastFallSpeed;

    private Rigidbody2D rb;
    private bool isGrounded;
    public string SceneName;

    void Start()
    {
        originalColliderSize = playerCollider.size;
        originalCollideroffset = playerCollider.offset;
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
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

        // Track the player's fall speed
        if (!isGrounded && rb.velocity.y < 0)
        {
            lastFallSpeed = rb.velocity.y;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;

            // Check for fall damage
            if (lastFallSpeed <= fallDamageThreshold)
            {
                TakeDamage(Mathf.Abs((int)(lastFallSpeed * 10))); // Apply damage based on fall speed
            }
        }

        if (collision.gameObject.CompareTag("Win"))
        {
            Invoke("SceneChanger", 1.5f);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Die();
        }

        if(collision.gameObject.CompareTag("key"))
        {
            collision.gameObject.GetComponent<Animator>().SetTrigger("gain");
            StartCoroutine(KeyDelay(collision.gameObject));
        }
    }

    private IEnumerator KeyDelay(GameObject gameObject)
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        animator.SetBool("isDead", true);
        StartCoroutine(ReloadSceneAfterDelay(2f)); // Adjust the delay time according to your animation length
    }

    private IEnumerator ReloadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReloadCurrentScene();
    }

    private void SceneChanger()
    {
        SceneManager.LoadScene(SceneName);
    }

    public void ReloadCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
