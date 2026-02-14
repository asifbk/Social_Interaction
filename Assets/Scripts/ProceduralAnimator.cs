using UnityEngine;

public class ProceduralAnimator : MonoBehaviour
{
    [Header("Bone References")]
    [SerializeField] private Transform hips;
    [SerializeField] private Transform spine;
    [SerializeField] private Transform chest;
    [SerializeField] private Transform leftUpperLeg;
    [SerializeField] private Transform rightUpperLeg;
    [SerializeField] private Transform leftLowerLeg;
    [SerializeField] private Transform rightLowerLeg;
    [SerializeField] private Transform leftFoot;
    [SerializeField] private Transform rightFoot;
    [SerializeField] private Transform leftShoulder;
    [SerializeField] private Transform rightShoulder;
    [SerializeField] private Transform leftUpperArm;
    [SerializeField] private Transform rightUpperArm;
    [SerializeField] private Transform leftLowerArm;
    [SerializeField] private Transform rightLowerArm;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    
    [Header("Walk Animation Settings")]
    [SerializeField] private float walkCycleSpeed = 4f;
    [SerializeField] private float legSwingAngle = 30f;
    [SerializeField] private float armSwingAngle = 15f;
    [SerializeField] private float bodyBounceAmount = 0.03f;
    [SerializeField] private float kneeAngle = 45f;
    
    [Header("Handshake Animation Settings")]
    [SerializeField] private float handshakeRaiseSpeed = 2f;
    [SerializeField] private float handshakeShakeSpeed = 3f;
    [SerializeField] private float handshakeShakeAmount = 10f;
    
    private bool isWalking = false;
    private bool isHandshaking = false;
    private float walkCycle = 0f;
    private float handshakeProgress = 0f;
    
    private Vector3 hipsInitialPosition;
    private Quaternion leftUpperLegInitial;
    private Quaternion rightUpperLegInitial;
    private Quaternion leftLowerLegInitial;
    private Quaternion rightLowerLegInitial;
    private Quaternion leftUpperArmInitial;
    private Quaternion rightUpperArmInitial;
    private Quaternion leftLowerArmInitial;
    private Quaternion rightLowerArmInitial;
    
    private void Start()
    {
        AutoFindBones();
        StoreInitialPoses();
        
        Debug.Log($"{transform.name} ProceduralAnimator initialized:");
        Debug.Log($"  Hips: {(hips != null ? "Found" : "NOT FOUND")}");
        Debug.Log($"  Left Upper Leg: {(leftUpperLeg != null ? "Found" : "NOT FOUND")}");
        Debug.Log($"  Right Upper Leg: {(rightUpperLeg != null ? "Found" : "NOT FOUND")}");
        Debug.Log($"  Left Upper Arm: {(leftUpperArm != null ? "Found" : "NOT FOUND")}");
        Debug.Log($"  Right Upper Arm: {(rightUpperArm != null ? "Found" : "NOT FOUND")}");
        Debug.Log($"  Right Lower Arm: {(rightLowerArm != null ? "Found" : "NOT FOUND")}");
    }
    
    private void AutoFindBones()
    {
        Transform armature = transform.Find("Armature");
        if (armature == null) return;
        
        hips = armature.Find("Hips");
        if (hips == null) return;
        
        spine = hips.Find("Spine");
        if (spine != null)
        {
            chest = spine.Find("Chest");
            
            if (chest != null)
            {
                leftShoulder = chest.Find("Left shoulder");
                rightShoulder = chest.Find("Right shoulder");
                
                if (leftShoulder != null)
                {
                    leftUpperArm = leftShoulder.Find("Left arm");
                    if (leftUpperArm != null)
                    {
                        leftLowerArm = leftUpperArm.Find("Left elbow");
                        if (leftLowerArm != null)
                        {
                            leftHand = leftLowerArm.Find("Left wrist");
                        }
                    }
                }
                
                if (rightShoulder != null)
                {
                    rightUpperArm = rightShoulder.Find("Right arm");
                    if (rightUpperArm != null)
                    {
                        rightLowerArm = rightUpperArm.Find("Right elbow");
                        if (rightLowerArm != null)
                        {
                            rightHand = rightLowerArm.Find("Right wrist");
                        }
                    }
                }
            }
        }
        
        leftUpperLeg = hips.Find("Left leg");
        rightUpperLeg = hips.Find("Right leg");
        
        if (leftUpperLeg != null)
        {
            leftLowerLeg = leftUpperLeg.Find("Left knee");
            if (leftLowerLeg != null)
            {
                leftFoot = leftLowerLeg.Find("Left ankle");
            }
        }
        
        if (rightUpperLeg != null)
        {
            rightLowerLeg = rightUpperLeg.Find("Right knee");
            if (rightLowerLeg != null)
            {
                rightFoot = rightLowerLeg.Find("Right ankle");
            }
        }
    }
    
    private void StoreInitialPoses()
    {
        if (hips != null) hipsInitialPosition = hips.localPosition;
        if (leftUpperLeg != null) leftUpperLegInitial = leftUpperLeg.localRotation;
        if (rightUpperLeg != null) rightUpperLegInitial = rightUpperLeg.localRotation;
        if (leftLowerLeg != null) leftLowerLegInitial = leftLowerLeg.localRotation;
        if (rightLowerLeg != null) rightLowerLegInitial = rightLowerLeg.localRotation;
        if (leftUpperArm != null) leftUpperArmInitial = leftUpperArm.localRotation;
        if (rightUpperArm != null) rightUpperArmInitial = rightUpperArm.localRotation;
        if (leftLowerArm != null) leftLowerArmInitial = leftLowerArm.localRotation;
        if (rightLowerArm != null) rightLowerArmInitial = rightLowerArm.localRotation;
    }
    
    private void LateUpdate()
    {
        if (isHandshaking)
        {
            AnimateHandshake();
        }
        else if (isWalking)
        {
            AnimateWalk();
        }
        else
        {
            AnimateIdle();
        }
    }
    
    private void AnimateWalk()
    {
        walkCycle += Time.deltaTime * walkCycleSpeed;
        
        float leftLegCycle = Mathf.Sin(walkCycle);
        float rightLegCycle = Mathf.Sin(walkCycle + Mathf.PI);
        
        if (leftUpperLeg != null)
        {
            Quaternion legRotation = Quaternion.Euler(leftLegCycle * legSwingAngle, 0, 0);
            leftUpperLeg.localRotation = leftUpperLegInitial * legRotation;
        }
        
        if (rightUpperLeg != null)
        {
            Quaternion legRotation = Quaternion.Euler(rightLegCycle * legSwingAngle, 0, 0);
            rightUpperLeg.localRotation = rightUpperLegInitial * legRotation;
        }
        
        if (leftLowerLeg != null)
        {
            float leftKneeBend = Mathf.Max(0, leftLegCycle);
            Quaternion kneeRotation = Quaternion.Euler(-leftKneeBend * kneeAngle, 0, 0);
            leftLowerLeg.localRotation = leftLowerLegInitial * kneeRotation;
        }
        
        if (rightLowerLeg != null)
        {
            float rightKneeBend = Mathf.Max(0, rightLegCycle);
            Quaternion kneeRotation = Quaternion.Euler(-rightKneeBend * kneeAngle, 0, 0);
            rightLowerLeg.localRotation = rightLowerLegInitial * kneeRotation;
        }
        
        if (leftUpperArm != null)
        {
            Quaternion armRotation = Quaternion.Euler(rightLegCycle * armSwingAngle, 0, 0);
            leftUpperArm.localRotation = leftUpperArmInitial * armRotation;
        }
        
        if (rightUpperArm != null)
        {
            Quaternion armRotation = Quaternion.Euler(leftLegCycle * armSwingAngle, 0, 0);
            rightUpperArm.localRotation = rightUpperArmInitial * armRotation;
        }
        
        if (hips != null)
        {
            float bounce = Mathf.Abs(Mathf.Sin(walkCycle * 2f)) * bodyBounceAmount;
            hips.localPosition = hipsInitialPosition + Vector3.down * bounce;
        }
    }
    
    private void AnimateIdle()
    {
        walkCycle = 0f;
        
        if (hips != null)
        {
            float breathing = Mathf.Sin(Time.time * 0.5f) * 0.01f;
            hips.localPosition = hipsInitialPosition + Vector3.up * breathing;
        }
        
        ResetToInitialPose();
    }
    
    private void AnimateHandshake()
    {
        handshakeProgress += Time.deltaTime;
        
        float totalDuration = 3.5f;
        float raiseTime = 0.5f;
        float shakeStartTime = 0.5f;
        float shakeEndTime = 3.0f;
        float lowerTime = 3.5f;
        
        float raiseProgress = Mathf.Clamp01(handshakeProgress / raiseTime);
        
        if (rightUpperArm != null)
        {
            float shoulderRotationZ = 0f;
            float shoulderRotationX = 0f;
            
            if (handshakeProgress < raiseTime)
            {
                shoulderRotationZ = Mathf.Lerp(0f, -90f, raiseProgress);
                shoulderRotationX = Mathf.Lerp(0f, 45f, raiseProgress);
            }
            else if (handshakeProgress < shakeEndTime)
            {
                shoulderRotationZ = -90f;
                shoulderRotationX = 45f;
            }
            else
            {
                float lowerProgress = (handshakeProgress - shakeEndTime) / (lowerTime - shakeEndTime);
                shoulderRotationZ = Mathf.Lerp(-90f, 0f, lowerProgress);
                shoulderRotationX = Mathf.Lerp(45f, 0f, lowerProgress);
            }
            
            Quaternion raiseRotation = Quaternion.Euler(shoulderRotationX, 0f, shoulderRotationZ);
            rightUpperArm.localRotation = rightUpperArmInitial * raiseRotation;
        }
        
        if (rightLowerArm != null)
        {
            float elbowBend = 0f;
            float shake = 0f;
            
            if (handshakeProgress < raiseTime)
            {
                elbowBend = Mathf.Lerp(0f, 80f, raiseProgress);
            }
            else if (handshakeProgress < shakeEndTime)
            {
                elbowBend = 80f;
                float shakePhase = (handshakeProgress - shakeStartTime) / (shakeEndTime - shakeStartTime);
                shake = Mathf.Sin(handshakeProgress * 8f) * 15f * shakePhase;
            }
            else
            {
                float lowerProgress = (handshakeProgress - shakeEndTime) / (lowerTime - shakeEndTime);
                elbowBend = Mathf.Lerp(80f, 0f, lowerProgress);
            }
            
            Quaternion elbowRotation = Quaternion.Euler(shake, elbowBend, 0f);
            rightLowerArm.localRotation = rightLowerArmInitial * elbowRotation;
        }
        
        if (handshakeProgress >= totalDuration)
        {
            handshakeProgress = 0f;
            Debug.Log($"{transform.name}: Handshake animation complete");
        }
    }
    
    private void ResetToInitialPose()
    {
        if (leftUpperLeg != null) leftUpperLeg.localRotation = leftUpperLegInitial;
        if (rightUpperLeg != null) rightUpperLeg.localRotation = rightUpperLegInitial;
        if (leftLowerLeg != null) leftLowerLeg.localRotation = leftLowerLegInitial;
        if (rightLowerLeg != null) rightLowerLeg.localRotation = rightLowerLegInitial;
        if (leftUpperArm != null) leftUpperArm.localRotation = leftUpperArmInitial;
        if (rightUpperArm != null) rightUpperArm.localRotation = rightUpperArmInitial;
        if (leftLowerArm != null) leftLowerArm.localRotation = leftLowerArmInitial;
        if (rightLowerArm != null) rightLowerArm.localRotation = rightLowerArmInitial;
    }
    
    public void SetWalking(bool walking)
    {
        isWalking = walking;
        if (!walking)
        {
            walkCycle = 0f;
        }
        Debug.Log($"{transform.name}: Walking = {walking}");
    }
    
    public void SetHandshaking(bool handshaking)
    {
        isHandshaking = handshaking;
        if (handshaking)
        {
            handshakeProgress = 0f;
            Debug.Log($"{transform.name}: Starting handshake animation!");
            
            if (rightUpperArm == null || rightLowerArm == null)
            {
                Debug.LogWarning($"{transform.name}: Arm bones not found! Handshake won't animate.");
            }
        }
        else
        {
            Debug.Log($"{transform.name}: Stopping handshake animation");
            ResetToInitialPose();
        }
    }
}
