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

    // 목숨 텍스트
    public TMP_Text livesText;

    // 다음 씬 이름
    public string nextSceneName = "Stage2";

    // 점프 중 캐릭터 크기와 좌우 방향
    public Vector2 airScale = new Vector2(0.8f, 1.2f);
    private float facing = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.position = startPosition;

        UpdateLivesText();

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

        // 낙사 처리
        if (transform.position.y < fallLimit)
        {
            LoseLife();
        }

        // 캐릭터 크기와 방향
        if (visual != null)
        {
            if (isGrounded)
            {
                visual.localScale = new Vector3(
                    facing, 1f, 1f);
            }
            else
            {
                visual.localScale = new Vector3(
                    facing * airScale.x,
                    airScale.y,
                    1f);
            }
        }
    }

    // 목숨 감소 및 낙사 후 복귀
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

            Debug.Log("게임 오버");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
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
}