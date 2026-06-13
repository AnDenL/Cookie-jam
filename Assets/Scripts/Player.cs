using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static bool IsControlable = true,NeedToFlip = true;
    static public Player mainPlayer;

    //public Inventory Inventory;

    [HideInInspector] public GameObject HandItem;
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public Vector2 additionalVelocity;

    [SerializeField] private float moveSpeed = 5f;

    private LayerMask layer;
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Interactable lastInteractable;

    private void Awake()
    {
        //Inventory = new Inventory();
        mainPlayer = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        HandItem = transform.GetChild(0).gameObject;
        layer = 1 << 3;
    }

    private void FixedUpdate()
    {
        if (!IsControlable)
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("Horizontal", 0);
            animator.SetFloat("Vertical", 0);
            return;
        }
        moveDirection.x = Input.GetAxisRaw("Horizontal");
        moveDirection.y = Input.GetAxisRaw("Vertical");

        if (NeedToFlip)
        {
            if (moveDirection.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (moveDirection.x > 0)
            {
                spriteRenderer.flipX = false;
            }
        }
        animator.SetFloat("Horizontal", Mathf.Abs(moveDirection.x));
        animator.SetFloat("Vertical", moveDirection.y);

        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        rb.velocity = moveDirection * moveSpeed + additionalVelocity;
    }
    
    private void Update()
    {
        CheckInteractions();
    }
    public void CheckInteractions()
    {
        var temp = Physics2D.OverlapCircleAll(transform.position, 3f, layer);

        if (temp.Length == 0)
        {
            lastInteractable?.HideKey();
            lastInteractable = null;
            return;
        }

        Collider2D nearestCollider = null;
        float nearestDistance = float.MaxValue;

        foreach (var collider in temp)
        {
            if (collider.CompareTag("Interactable"))
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestCollider = collider;
                    nearestDistance = distance;
                }
            }
        }

        if (nearestCollider)
        {
            lastInteractable?.HideKey();

            lastInteractable = nearestCollider.GetComponent<Interactable>();
            lastInteractable.ShowKey();

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (nearestCollider.TryGetComponent(out Interactable interactable))
                {
                    interactable.Interact(this);
                }
            }
        }
    }
} 
