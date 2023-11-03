using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GrappleStates
{
    None,
    ToPoint,
    Swinging
}

public enum PlayerAnimations
{
    Idle,
    Walking,
    Grappling,
    Falling,
    Jumping
} // Todo: dashing

public class PlayerMove : MonoBehaviour
{
    [Header("Speed")]
    public float moveSpeed = 2.5f;
    public float sprintMultiplier = 1.5f;

    [Header("Jumper")]
    public float jumpForce = 7.5f;
    bool grounded = false;

    [Header("Dash")]
    public bool canDash = true;
    public float dashCooldown = 0.25f;
    public float dashSpeed = 5.0f;
    public float dashTime = 0.1f;
    public static bool dashEnabled = true;
    bool dashing = false;
    float dashCooldownTimer = 0.0f;
    float dashTimer = 0.0f;

    [Header("Grapple")]
    public SpriteRenderer grappleLine;
    public GrappleStates grappleState = GrappleStates.None;
    public float swingDist = 2.5f;
    public float swingMaxAngle = 40.0f;
    public float swingSpeedMul = 2.0f;
    public float toPointSpeed = 6.0f;
    public static bool isGrappling = false;
    public static GameObject grappleTarget;
    float swingT = 0.0f;

    [Header("Animation Time Baybee")]
    public Animator animator;
    public PlayerAnimations curAnim = PlayerAnimations.Idle;

    [Header("Health & Damage")]
    public bool dead = false;
    public int health = 3;
    public float knockbackForce = 6.0f;
    public float knockbackTime = 0.2f;
    public GameObject deathPrefab;
    public Color highHealth;
    public Color medHealth;
    public Color lowHealth;

    [Header("Points")]
    public int points = 0;
    public int dollarPoints = 100;
    public int chopterPoints = 500;
    public int sudoPoints = 1000;

    [Header("SUDO")]
    public int sudosCollected = 0;
    public string winScene = "Win";
    public bool won = false;

    [Header("UI")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI sudoText;

    [Header("Binds n Stuff")]
    public string titleScreenScene = "Title";

    bool knockingBack = false;
    float knockbackTimer = 0.0f;
    float gravScale;
    Rigidbody2D rb;
    SpriteRenderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gravScale = rb.gravityScale;
        rend = GetComponentInChildren<SpriteRenderer>();

        SudoThingie.sudoCharIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // There's nothing here for dead folks
        if (dead || won)
            return;

        // Movement stuff
        if ((!dashing && grappleState == GrappleStates.None) && !knockingBack)
        {
            Move();
            Jump();
        }

        // Abilities
        //Dash();
        Grapple();

        // (B)Fallin'
        if (!grounded && grappleState == GrappleStates.None)
        {
            if (rb.velocity.y < 0)
            {
                SetAnimation(PlayerAnimations.Falling);
            }
            else
            {
                SetAnimation(PlayerAnimations.Jumping);
            }
        }

        // Knockback gaming!
        if (knockingBack)
        {
            knockbackTimer += Time.deltaTime;
            if (knockbackTimer >= knockbackTime)
                knockingBack = false;
        }

        // Sprite stuff
        rend.flipX = (rb.velocity.x < 0);

        // Binds
        if (Input.GetKeyDown(KeyCode.R))
            RestartLevelOnDeath();
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(titleScreenScene);
    }

    void Move()
    {
        // Making sure we CANNOT move during grapple
        if (grappleState != GrappleStates.None)
            return;
        
        // Initialise
        float xVel = 0;

        // Schmove
        if (Input.GetKey(KeyCode.A))
            xVel -= moveSpeed;
        if (Input.GetKey(KeyCode.D))
            xVel += moveSpeed;

        // Agile Sprint Scrum PM Synergy
        if (Input.GetKey(KeyCode.LeftShift))
            xVel *= sprintMultiplier;

        // Actually schmove?
        rb.velocity = new Vector2(xVel, rb.velocity.y);

        // Animate
        if (grounded && grappleState == GrappleStates.None)
        {
            if (xVel != 0)
                SetAnimation(PlayerAnimations.Walking);
            else
                SetAnimation(PlayerAnimations.Idle);
        }
    }

    void Jump()
    {
        // No jumping if not in air!!!
        if (!grounded || knockingBack)
            return;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Boing!
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            grounded = false;
        }
    }

    void Dash()
    {
        // Can't dash if we don't want it
        if (!dashEnabled) return;

        // Timer handling first
        if (!canDash)
        {
            dashCooldownTimer += Time.deltaTime;

            if (dashCooldownTimer >= dashCooldown)
            {
                dashCooldownTimer = 0;
                canDash = true;
            }
            else
            {
                return;
            }
        }

        // Beginning the dash
        if (canDash && !dashing && Input.GetMouseButtonDown(0))
        {
            // We now dashing
            dashing = true;
            dashTimer = 0.0f;

            // Figure out which direction the mouse is pointing
            float mouseX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            float xDiff = mouseX - transform.position.x;
            float dashDir = xDiff / Mathf.Abs(xDiff); //(mouseX - transform.position.x >= 0) ? 1 : -1; // -1 for left, 1 for right

            // Set the velocity + rigidbody constraints
            rb.velocity = new Vector2(dashDir * dashSpeed, 0);
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }

        // Dash handling
        if (dashing)
        {
            dashTimer += Time.deltaTime;

            if (dashTimer >= dashTime)
            {
                // No more dashing
                dashTimer = 0.0f;
                dashing = false;
                canDash = false;

                // Resetting the rigidbody stuff
                rb.gravityScale = gravScale;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                rb.velocity = Vector2.zero;

                return;
            }
            else
            {
                return;
            }
        }
    }

    void Grapple()
    {
        // When a grapple is stopped, we disable the grapple and make 'em BOING
        if ((grappleState != GrappleStates.None && Input.GetMouseButtonUp(1)) || knockingBack)
        {
            
            // If swinging we shoot player up a little
            if (grappleState == GrappleStates.Swinging)
            {
                // Usual stuff
                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

                // Destroying a chopter if we're grappled onto it
                Chopter chopter = grappleTarget.GetComponent<Chopter>();
                if (chopter != null)
                {
                    chopter.Dienow();
                    points += chopterPoints;
                    UpdatePointsText();
                }
            }

            // The REAL usual stuff
            rb.gravityScale = gravScale;
            grappleState = GrappleStates.None;
            isGrappling = false;

            //SetAnimation(PlayerAnimations.Falling);
        }
        
        // Starting the grapple, the player is now going to the grapple point
        if (grappleState == GrappleStates.None && Input.GetMouseButtonDown(1) && grappleTarget != null)
        {
            grappleState = GrappleStates.ToPoint;
            isGrappling = true;
            rb.gravityScale = 0;
            SetAnimation(PlayerAnimations.Grappling);
        }

        // If the player is going to the grapple point, we move 'em there
        if (grappleState == GrappleStates.ToPoint)
        {
            // If we are now at the point, it's time to actually swing
            if (Vector2.Distance(grappleTarget.transform.position, transform.position) <= swingDist)
            {
                grappleState = GrappleStates.Swinging;
                swingT = 0.0f;
            }
            // Otherwise we schmove 'em
            else
            {
                // First get the direction to the point
                Vector2 toPoint = (grappleTarget.transform.position - transform.position).normalized;

                // Now we set our velocity to that
                rb.velocity = toPoint * toPointSpeed;
            }
        }

        // Finally, swinging!
        if (grappleState == GrappleStates.Swinging)
        {
            // Man this sucks, but it should work (I hope)
            Vector3 swingPos = new Vector2(
                    Mathf.Cos(Mathf.PI * swingT * swingSpeedMul - Mathf.PI / 2), 
                    -Mathf.Pow( Mathf.Sin( Mathf.PI * swingT * swingSpeedMul - Mathf.PI / 2 ), 2 )
                ) * swingDist;

            transform.position = grappleTarget.transform.position + swingPos;

            swingT += Time.deltaTime;
        }

        // Grapple line
        if (grappleState != GrappleStates.None)
        {
            // Show
            grappleLine.gameObject.SetActive(true);

            // Point towards the target
            grappleLine.transform.right = grappleTarget.transform.position - transform.position;

            // Set it to be between the player and point
            grappleLine.transform.position = Vector2.Lerp(transform.position, grappleTarget.transform.position, 0.5f);

            // Scale its tiled scale
            float distToTarget = Vector2.Distance(transform.position, grappleTarget.transform.position);
            grappleLine.size = new Vector2(1.28f * distToTarget, 1.28f);
        }
        else
        {
            // Hide
            grappleLine.gameObject.SetActive(false);
        }
    }

    void SetAnimation(PlayerAnimations newAnim)
    {
        if (curAnim == newAnim)
            return;

        switch (newAnim)
        {
            case PlayerAnimations.Idle:
                {
                    animator.SetTrigger("Idle");
                    break;
                }
            case PlayerAnimations.Walking:
                {
                    animator.SetTrigger("Walk");
                    break;
                }
            case PlayerAnimations.Grappling:
                {
                    animator.SetTrigger("Grapple");
                    break;
                }
            case PlayerAnimations.Falling:
                {
                    animator.SetTrigger("Fall");
                    break;
                }
        }

        curAnim = newAnim;
    }

    public void TakeDamage(Vector2 attackerPos, int damage = 1)
    {
        // We don't wanna punish the poor player too much now do we?
        if (knockingBack || won)
            return;

        // Health '''logic'''
        health -= 1;
        if (health <= 0)
        {
            // Restart the level after a second
            Invoke("RestartLevelOnDeath", 2.0f);

            // Spawn the death thingie
            GameObject deathThingie = Instantiate(deathPrefab, transform.position, Quaternion.identity);
            deathThingie.GetComponent<SpriteRenderer>().color = rend.color;
            Destroy(deathThingie, 2.0f);

            // Rest of stuff
            rend.enabled = false;
            dead = true;
        }

        // Beginning knockback
        knockingBack = true;
        knockbackTimer = 0.0f;

        // Now we actually knock the player back
        rb.velocity = -(attackerPos - (Vector2)transform.position).normalized * knockbackForce;

        // Time to update the text!
        UpdateHealthText();
    }

    void UpdateHealthText()
    {
        switch (health)
        {
            case 5:
                healthText.color = highHealth;
                healthText.text = " [♥♥♥♥♥]";
                break;
            case 4:
                healthText.color = medHealth;
                healthText.text = " [♥♥♥♥-]";
                break;
            case 3:
                healthText.color = medHealth;
                healthText.text = " [♥♥♥--]";
                break;
            case 2:
                healthText.color = lowHealth;
                healthText.text = " [♥♥---]";
                break;
            case 1:
                healthText.color = lowHealth;
                healthText.text = " [♥----]";
                break;
            case 0:
                healthText.color = lowHealth;
                healthText.text = " [-----]";
                break;
        }
    }

    void UpdatePointsText()
    {
        pointsText.text = $"${points}";
    }

    void UpdateSudosCollectedText()
    {
        switch (sudosCollected)
        {
            case 0:
                sudoText.text = ">----";
                break;
            case 1:
                sudoText.text = ">S---";
                break;
            case 2:
                sudoText.text = ">SU--";
                break;
            case 3:
                sudoText.text = ">SUD-";
                break;
            case 4:
                sudoText.text = ">SUDO";
                break;
        }
    }

    void RestartLevelOnDeath()
    {
        // Yeah just restarts the level
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    [ContextMenu("Win!")]
    void Win()
    {
        won = true;
        animator.SetTrigger("ASCEND");
        //GetComponentInChildren<Collider2D>().enabled = false;
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0.0f;
        Invoke("GoToWinScene", 2.0f);
    }

    void GoToWinScene()
    {
        SceneManager.LoadScene(winScene);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Grounded, circuit is safe
        if (collision.gameObject.CompareTag("Ground"))
            grounded = true;

        // Damage gaming
        if (collision.gameObject.CompareTag("Enemy"))
            TakeDamage(collision.gameObject.transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Collectables stuff!
        if (collision.CompareTag("Heart"))
        {
            if (health < 5)
                health++;
            UpdateHealthText();

            points += dollarPoints;
            UpdatePointsText();

            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Dollar"))
        {
            points += dollarPoints;
            UpdatePointsText();

            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("SUDO"))
        {
            sudosCollected += 1;
            UpdateSudosCollectedText();

            points += sudoPoints;
            UpdatePointsText();

            collision.GetComponent<SudoThingie>().UpdateAllSudos();

            Destroy(collision.gameObject);

            if (sudosCollected >= 4)
                Win();
        }

        // Damage gaming
        if (collision.gameObject.CompareTag("Enemy"))
            TakeDamage(collision.gameObject.transform.position);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Ungrounded, pray for mercy
        if (collision.gameObject.CompareTag("Ground"))
            grounded = false;
    }
}
