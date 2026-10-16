using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyPatrol : MonoBehaviour
{
    public float speed = 2f, leftDistance = 3f, rightDistance = 3f;
    Rigidbody2D rb;
    float minX, maxX, dir = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        minX = transform.position.x - leftDistance;
        maxX = transform.position.x + rightDistance;
    }

    void FixedUpdate()
    {
        if (transform.position.x >= maxX) dir = -1;
        else if (transform.position.x <= minX) dir = 1;
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        var player = c.collider.GetComponent<PlayerController>();
        if (player == null) return;

        bool stomped = c.transform.position.y > transform.position.y + 0.3f;
        if (stomped)
        {
            GameManager.Instance.AddScore(50);
            var prb = c.collider.GetComponent<Rigidbody2D>();
            prb.linearVelocity = new Vector2(prb.linearVelocity.x, 9f); // bounce
            Destroy(gameObject);
        }
        else
        {
            GameManager.Instance.LoseLife();
            player.Respawn();
        }
    }
}