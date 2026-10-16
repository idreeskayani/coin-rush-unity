using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float speed = 7f, acceleration = 60f;
    public float jumpVelocity = 12f;
    public float coyoteTime = 0.12f, jumpBufferTime = 0.12f;
    public float normalGravity = 2f, fallGravity = 3.5f, lowJumpGravity = 3f;
    public float killY = -10f;

    Rigidbody2D rb;
    Vector3 startPos;
    float move, coyote, buffer;
    bool jumpHeld, grounded, touching;

    void Awake() { rb = GetComponent<Rigidbody2D>(); startPos = transform.position; }

    void Update()
    {
        var k = Keyboard.current;
        if (k == null) return;

        move = 0;
        if (k.aKey.isPressed || k.leftArrowKey.isPressed) move -= 1;
        if (k.dKey.isPressed || k.rightArrowKey.isPressed) move += 1;
        jumpHeld = k.spaceKey.isPressed;

        buffer = k.spaceKey.wasPressedThisFrame ? jumpBufferTime : buffer - Time.deltaTime;
        coyote = grounded ? coyoteTime : coyote - Time.deltaTime;

        if (buffer > 0 && coyote > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
            buffer = 0; coyote = 0;
        }

        if (transform.position.y < killY)
        {
            GameManager.Instance.LoseLife();
            Respawn();
        }
    }

    void FixedUpdate()
    {
        grounded = touching;
        touching = false;

        var v = rb.linearVelocity;
        v.x = Mathf.MoveTowards(v.x, move * speed, acceleration * Time.fixedDeltaTime);
        rb.linearVelocity = v;

        // Variable jump height: heavier on the way down, or when jump is released early
        rb.gravityScale = v.y < 0 ? fallGravity : (v.y > 0 && !jumpHeld ? lowJumpGravity : normalGravity);
    }

    void OnCollisionStay2D(Collision2D c)
    {
        foreach (var p in c.contacts)
            if (p.point.y < transform.position.y - 0.4f) { touching = true; return; }
    }

    public void Respawn()
    {
        transform.position = startPos;
        rb.linearVelocity = Vector2.zero;
    }
}