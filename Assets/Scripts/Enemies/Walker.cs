using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.0f;
    public bool movingLeft = true;

    [Header("Animation")]
    public List<Sprite> frames = new List<Sprite>();
    public float animTime = 0.5f;

    int currentAnim = 0;
    float leftBounds = 0.0f;
    float rightBounds = 0.0f;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        FindBounds();
        InvokeRepeating("ChangeAnimation", animTime, animTime);
    }

    // Update is called once per frame
    void Update()
    {
        // Direction gaming
        if (transform.position.x <= leftBounds)
        {
            transform.position = new Vector2(leftBounds, transform.position.y);
            movingLeft = false;
        }
        else if (transform.position.x >= rightBounds)
        {
            transform.position = new Vector2(rightBounds, transform.position.y);
            movingLeft = true;
        }

        // Velocity gaming
        rb.velocity = new Vector2(moveSpeed * (movingLeft ? -1 : 1), 0);
    }

    void FindBounds()
    {
        // Finding the ground
        GameObject ground;
        RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(0, 0.65f), Vector2.down);
        ground = hit.collider.gameObject;

        // Calculating the bounds
        leftBounds = ground.transform.position.x - ground.GetComponent<Collider2D>().bounds.size.x / 2 + 0.5f;
        rightBounds = ground.transform.position.x + ground.GetComponent<Collider2D>().bounds.size.x / 2 - 0.5f;

        // Setting the position
        transform.position = ground.transform.position + new Vector3(0, ground.GetComponent<Collider2D>().bounds.size.y / 2 + 0.65f);
    }

    public void ChangeAnimation()
    {
        // Counter baybee
        currentAnim += 1;
        if (currentAnim >= frames.Count)
            currentAnim = 0;

        // Gaming animation gaming gaming I am losing my mind hahahahahah
        GetComponentInChildren<SpriteRenderer>().sprite = frames[currentAnim];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Do the thing that makes gameplay somewhat difficult and turns this into a game
        if (collision.gameObject.tag == "Player")
            collision.gameObject.GetComponent<PlayerMove>().TakeDamage(transform.position);
    }
}
