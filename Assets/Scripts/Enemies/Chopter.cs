using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chopter : MonoBehaviour
{
    [Header("Attak!")]
    public float playerAttackDist = 15.0f;
    public float attackInterval = 1.5f;
    public float buletSpeed = 2.5f;
    public GameObject bulet;

    [Header("Animation")]
    public List<Sprite> frames = new List<Sprite>();
    public float animTime = 0.5f;

    [Header("Skull Emoji")]
    public GameObject deathPrefab;

    int currentAnim = 0;
    float time = 0.0f;
    Vector2 originalPos;
    GameObject player = null;

    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
        InvokeRepeating("ChangeAnimation", animTime, animTime); 
        InvokeRepeating("Attak", attackInterval, attackInterval);
    }

    // Update is called once per frame
    void Update()
    {
        // Floating
        time += Time.deltaTime;
        transform.position = originalPos + new Vector2(0, 0.25f * Mathf.Sin(2 * Mathf.PI * time * 0.33f));
    }

    public void Attak()
    {
        // No player, no target, no war, everyone wins!
        if (player == null) return;

        // Not bothering if the player is in front of the chopter, or if the player is grappled to the chopter
        if (player.transform.position.x >= transform.position.x || PlayerMove.grappleTarget == gameObject)
            return;

        // Not bothering also if the player is too far from chopter
        if (Vector3.Distance(player.transform.position, transform.position) > playerAttackDist)
            return;

        GameObject buletObj = Instantiate(bulet, transform.position, Quaternion.Euler(0, 0, 0));
        buletObj.transform.right = -(player.transform.position - transform.position);
        buletObj.GetComponent<Rigidbody2D>().velocity = -(transform.position - player.transform.position).normalized * buletSpeed;
        buletObj.GetComponent<Arrow>().OnCreation(GetComponentInChildren<Collider2D>());
    }

    public void ChangeAnimation()
    {
        // Counter baybee
        currentAnim += 1;
        if (currentAnim >= frames.Count)
            currentAnim = 0;

        // Set da anim
        GetComponentInChildren<SpriteRenderer>().sprite = frames[currentAnim];
    }

    public void Dienow()
    {
        // Spawn the death thingie
        GameObject deathThingie = Instantiate(deathPrefab, transform.position, Quaternion.identity);
        deathThingie.GetComponent<SpriteRenderer>().color = GetComponentInChildren<SpriteRenderer>().color;
        Destroy(deathThingie, 2.0f);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Do the thing that makes gameplay somewhat difficult and turns this into a game
        if (collision.gameObject.tag == "Player")
            collision.gameObject.GetComponent<PlayerMove>().TakeDamage(transform.position);
    }
}
