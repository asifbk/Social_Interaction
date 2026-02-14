using UnityEngine;

public class AvatarController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float rotationSpeed = 100f;
    
    [Header("Boundary Settings")]
    [SerializeField] private Transform roomTransform;
    [SerializeField] private Vector3 boundaryMin = new Vector3(-10f, 0f, -10f);
    [SerializeField] private Vector3 boundaryMax = new Vector3(10f, 0f, 10f);
    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    
    [Header("Handshake Settings")]
    [SerializeField] private AvatarController otherAvatar;
    [SerializeField] private float handshakeDistance = 1.5f;
    
    [Header("Procedural Animation")]
    [SerializeField] private ProceduralAnimator proceduralAnimator;
    
    private bool isWalking = false;
    private bool isHandshaking = false;
    
    private const string ANIM_WALKING = "Walking";
    private const string ANIM_HANDSHAKE = "Handshake";
    
    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        
        if (proceduralAnimator == null)
        {
            proceduralAnimator = GetComponent<ProceduralAnimator>();
        }
    }
    
    private void Update()
    {
        if (isHandshaking)
        {
            return;
        }
        
        HandleMovement();
        HandleRotation();
        HandleHandshake();
    }
    
    private void HandleMovement()
    {
        bool walkInput = Input.GetKey(KeyCode.W);
        
        if (walkInput)
        {
            Vector3 movement = transform.forward * walkSpeed * Time.deltaTime;
            Vector3 newPosition = transform.position + movement;
            
            newPosition = ClampPositionToBoundary(newPosition);
            
            transform.position = newPosition;
            
            if (!isWalking)
            {
                isWalking = true;
                UpdateAnimation();
            }
        }
        else
        {
            if (isWalking)
            {
                isWalking = false;
                UpdateAnimation();
            }
        }
    }
    
    private void HandleRotation()
    {
        float rotationInput = 0f;
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rotationInput = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rotationInput = 1f;
        }
        
        if (rotationInput != 0f)
        {
            float rotation = rotationInput * rotationSpeed * Time.deltaTime;
            transform.Rotate(0f, rotation, 0f);
        }
    }
    
    private void HandleHandshake()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (otherAvatar != null)
            {
                float distance = Vector3.Distance(transform.position, otherAvatar.transform.position);
                
                Debug.Log($"{gameObject.name}: Distance to other avatar = {distance}, Required = {handshakeDistance}");
                
                if (distance <= handshakeDistance)
                {
                    Debug.Log($"{gameObject.name}: Starting handshake!");
                    StartHandshake();
                    otherAvatar.StartHandshake();
                }
                else
                {
                    Debug.Log($"{gameObject.name}: Too far for handshake! Move closer.");
                }
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: Other avatar reference is null!");
            }
        }
    }
    
    private Vector3 ClampPositionToBoundary(Vector3 position)
    {
        if (roomTransform != null)
        {
            Vector3 localPos = roomTransform.InverseTransformPoint(position);
            
            localPos.x = Mathf.Clamp(localPos.x, boundaryMin.x, boundaryMax.x);
            localPos.y = Mathf.Clamp(localPos.y, boundaryMin.y, boundaryMax.y);
            localPos.z = Mathf.Clamp(localPos.z, boundaryMin.z, boundaryMax.z);
            
            return roomTransform.TransformPoint(localPos);
        }
        else
        {
            position.x = Mathf.Clamp(position.x, boundaryMin.x, boundaryMax.x);
            position.y = Mathf.Clamp(position.y, boundaryMin.y, boundaryMax.y);
            position.z = Mathf.Clamp(position.z, boundaryMin.z, boundaryMax.z);
            
            return position;
        }
    }
    
    public void StartHandshake()
    {
        isHandshaking = true;
        isWalking = false;
        Debug.Log($"{gameObject.name}: Handshake started!");
        UpdateAnimation();
        Invoke(nameof(StopHandshake), 3.5f);
    }
    
    private void StopHandshake()
    {
        isHandshaking = false;
        Debug.Log($"{gameObject.name}: Handshake stopped!");
        UpdateAnimation();
    }
    
    private void UpdateAnimation()
    {
        if (proceduralAnimator != null)
        {
            proceduralAnimator.SetWalking(isWalking);
            proceduralAnimator.SetHandshaking(isHandshaking);
        }
        
        if (animator == null)
        {
            return;
        }
        
        animator.SetBool(ANIM_WALKING, isWalking);
        animator.SetBool(ANIM_HANDSHAKE, isHandshaking);
    }
    
    public bool IsWalking => isWalking;
    public bool IsHandshaking => isHandshaking;
}
