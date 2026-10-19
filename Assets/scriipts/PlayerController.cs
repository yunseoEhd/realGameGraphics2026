
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public string playerName = "Player";
    public int hp = 100;

    public float moveSpeed = 3f;
    public Vector3 startPosition;
    public Transform visual;

    private Vector2 moveInput;
    public float jumpPower = 8f;
    private Rigidbody2D rb;
    private bool isGrounded = false;
    public float fallLimit = -10f;

    public int lives = 3;
    private bool isGameOver = false;

    public TMP_Text livesText;
    public string nextSceneName = "Stage2";

    public Vector2 airScale = new Vector2(0.95f, 1.05f);
    public GameObject gameOverPanel;

    private float facing = 1f;

    // 이미지의 원래 크기를 저장
    private Vector3 originalVisualScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.position = startPosition;

        if (visual != null)
        {
            originalVisualScale = visual.localScale;
        }

        UpdateLivesText();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        Debug.Log(playerName + " 시작. 체력 " + hp);
        Debug.Log("피격 후 체력 " + (hp - 30));
        Debug.Log("달리기 속도 " + moveSpeed * 2);
        Debug.Log("10f / 4f = " + 10f / 4f);

        if (hp >= 70)
        {
            Debug.Log("건강");
        }
        else if (hp >= 30)
        {
            Debug.Log("주의");
        }
        else
        {
            Debug.Log("위험");
        }
    }

    void UpdateLivesText()
    {
        if (livesText != null)
        {
            livesText.text = "LIFE " + lives;
        }
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded && !isGameOver)
        {
            Debug.Log("점프!");

            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x, jumpPower);
        }
    }

    void OnMove(InputValue value)
    {
        if (isGameOver) return;

        moveInput = value.Get<Vector2>();

        if (moveInput.x > 0)
        {
            facing = 1f;
        }
        else if (moveInput.x < 0)
        {
            facing = -1f;
        }
    }

    void Update()
    {
        if (isGameOver) return;

        transform.Translate(
            Vector3.right * moveInput.x
            * moveSpeed * Time.deltaTime);

        if (transform.position.y < fallLimit)
        {
            LoseLife();
        }

        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (visual == null) return;

        float scaleX = Mathf.Abs(originalVisualScale.x) * facing;
        float scaleY = originalVisualScale.y;

        if (!isGrounded)
        {
            scaleX *= airScale.x;
            scaleY *= airScale.y;
        }

        visual.localScale = new Vector3(
            scaleX,
            scaleY,
            originalVisualScale.z
        );
    }

    void LoseLife()
    {
        if (isGameOver) return;

        lives--;
        UpdateLivesText();

        transform.position = startPosition;
        rb.linearVelocity = Vector2.zero;
        isGrounded = false;

        Debug.Log("낙사. 남은 목숨 " + lives);

        if (lives <= 0)
        {
            isGameOver = true;
            moveInput = Vector2.zero;

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Goal"))
        {
            Debug.Log("골인!");
            SceneManager.LoadScene(nextSceneName);
        }
        else if (other.CompareTag("Trap"))
        {
            LoseLife();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
