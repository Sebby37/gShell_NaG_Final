using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // We don't like """memory leaks"""
        Destroy(gameObject, 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCreation(Collider2D creator)
    {
        Physics2D.IgnoreCollision(creator, GetComponent<Collider2D>());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Do the thing that makes gameplay somewhat difficult and turns this into a game
        if (collision.gameObject.tag == "Player")
            collision.gameObject.GetComponent<PlayerMove>().TakeDamage(transform.position);

        // Bye bye arrow!
        Destroy(gameObject);
    }
}
