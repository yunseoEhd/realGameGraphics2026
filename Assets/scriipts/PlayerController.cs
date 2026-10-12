
using UnityEngine;
using UnityEngine.InputSystem;

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

    // 점프 중 캐릭터 크기와 좌우 방향
    public Vector2 airScale = new Vector2(0.8f, 1.2f);
    private float facing = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.position = startPosition;

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

        // 낙사 및 목숨 처리
        if (transform.position.y < fallLimit)
        {
            lives--;

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

        // 점프 중에는 길쭉하게, 착지하면 원래 크기로
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
}
