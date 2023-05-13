using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class EnemyAI : MonoBehaviour
{
    // Helpful variables
    public NavMeshAgent thisAI;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public int maxHealth = 100;
    public int health = 100;
    private Renderer selfRenderer;
    public Slider healthBar;
    public Canvas healthCanvas;
    private Color defaultColor;

    // AI Movement
    public Vector3 walkPoint;
    public float timeBetweenPatrols;
    bool isWalking;
    bool walkPointSet;
    public float walkPointRange;

    // AI Attacks
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    bool canAttack = true;
    public GameObject projectile;

    // AI vision
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        SetHealthBar();
        SetRenderer();
        SetDefaultColor();
        player = GameObject.Find("PlayerCapsule").transform;
        thisAI = GetComponent<NavMeshAgent>();
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("PlayerBullet")) {
            FlashRed();
            TakeDamage(30);
        }
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // Invoke(nameof(ResetAttack), timeBetweenAttacks);

        if(!playerInSightRange && !playerInAttackRange) {
            if (!(isWalking)) 
                Patrolling();
        }

        else if(playerInSightRange && !playerInAttackRange)
            ChasePlayer();

        else if(playerInAttackRange && playerInSightRange) 
            AttackPlayer();
            
        
    }

    private void SetHealthBar()
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
    }

    private void Patrolling()
    {
        // If no walk point, search for one
        if (!walkPointSet)
            SearchWalkPoint();

        // If a walk point is set, set this to be our destination
        if (walkPointSet)
            thisAI.SetDestination(walkPoint);
            
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // If the distance to this walk point is less than 1 unit, current walk point has been 'reached'
        if (distanceToWalkPoint.magnitude < 1f) {
            walkPointSet = false;
            StartCoroutine(waitToPatrol());
        }
    }

    IEnumerator waitToPatrol()
    {
        isWalking = true;
        yield return new WaitForSecondsRealtime(timeBetweenPatrols);
        isWalking = false;
    }


    private void SearchWalkPoint()
    {
        // Randomly select a Z and X position based on the AI's walk range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        // These are the three coordinates that the AI will attempt to walk to
        float targetX = transform.position.x + randomX;
        float targetZ = transform.position.z + randomZ;
        float targetY = transform.position.y; // Y position doesn't change

        walkPoint = new Vector3(targetX, targetY, targetZ);

        /*
            Physics.Raycast() parameters:
                - origin        <starting point of ray>
                - direction     <direction of ray>
                - maxDistance   <max distance ray should check for collisions>
                = layerMask     <layer mask used to ignore Colliders when casting a ray>
        */
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }


    private void ChasePlayer()
    {
        thisAI.SetDestination(player.position);
        transform.LookAt(player); // I added this
    }

    private void AttackPlayer()
    {
        // Stand still and look at player
        thisAI.SetDestination(transform.position);
        transform.LookAt(player);
        

        // If haven't attacked yet
        if(!alreadyAttacked && canAttack)
        {
            // Code for shooting projectile
            // Important to have "transform.position + transform.forward" so that projectile doesn't hit self
            Rigidbody rbody = Instantiate(projectile, transform.position + transform.forward, Quaternion.identity).GetComponent<Rigidbody>();
            rbody.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rbody.AddForce(transform.up * 5f, ForceMode.Impulse);            

            // TODO: Invoke calls a function after however much time you tell it to
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    // Function for creating attack speed
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    // Function for taking damage
    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health <= 0) {
            
            selfRenderer.material.color = Color.red;
            canAttack = false;
            Invoke(nameof(DestroySelf), 0.5f);
        }
        SetHealthBar();
    }

    // Function for setting renderer
    private void SetRenderer()
    {
        selfRenderer = GetComponent<Renderer>();
    }

    // Function for setting the default color of this AI
    private void SetDefaultColor()
    {
        defaultColor = selfRenderer.material.color;
    }

    // Function for flashing red every time we take damage
    private void FlashRed()
    {
        // Checks that we're not going to die with the next hit (attacks do 30 damage)
        // This is so that we stay red before death instead of only blinking red
        if (health > 30) {
            selfRenderer.material.color = Color.red;
            Invoke(nameof(ResetToBlue), 0.05f);
        }
    }

    // Function for resetting color to blue
    private void ResetToBlue()
    {
        selfRenderer.material.color = defaultColor;
    }

    // Function for destroying self
    private void DestroySelf()
    {
        Destroy(gameObject);
    }

}
