using UnityEngine;
using UnityEditor;

public static class AnimationSetupHelper
{
    [MenuItem("Tools/Setup Avatar Animations")]
    public static void SetupAvatarAnimations()
    {
        CreateAnimationClips();
        CreateAnimatorController();
        AssignAnimatorsToAvatars();
        
        Debug.Log("Avatar animation setup complete!");
    }
    
    private static void CreateAnimationClips()
    {
        CreateIdleAnimation();
        CreateWalkingAnimation();
        CreateHandshakeAnimation();
    }
    
    private static void CreateIdleAnimation()
    {
        AnimationClip clip = new AnimationClip();
        clip.name = "Idle";
        clip.legacy = false;
        clip.wrapMode = WrapMode.Loop;
        
        AnimationCurve breathingCurve = AnimationCurve.EaseInOut(0f, 0f, 2f, 0.02f);
        breathingCurve.postWrapMode = WrapMode.Loop;
        breathingCurve.preWrapMode = WrapMode.Loop;
        
        clip.SetCurve("Armature/Hips/Spine/Chest", typeof(Transform), "localPosition.y", breathingCurve);
        
        AssetDatabase.CreateAsset(clip, "Assets/Animations/Idle.anim");
        Debug.Log("Created Idle animation");
    }
    
    private static void CreateWalkingAnimation()
    {
        AnimationClip clip = new AnimationClip();
        clip.name = "Walking";
        clip.legacy = false;
        clip.wrapMode = WrapMode.Loop;
        
        float duration = 1f;
        
        AnimationCurve leftLegRotation = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(duration * 0.25f, 45f),
            new Keyframe(duration * 0.5f, 0f),
            new Keyframe(duration * 0.75f, -30f),
            new Keyframe(duration, 0f)
        );
        leftLegRotation.postWrapMode = WrapMode.Loop;
        leftLegRotation.preWrapMode = WrapMode.Loop;
        
        AnimationCurve rightLegRotation = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(duration * 0.25f, -30f),
            new Keyframe(duration * 0.5f, 0f),
            new Keyframe(duration * 0.75f, 45f),
            new Keyframe(duration, 0f)
        );
        rightLegRotation.postWrapMode = WrapMode.Loop;
        rightLegRotation.preWrapMode = WrapMode.Loop;
        
        AnimationCurve leftArmRotation = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(duration * 0.25f, -20f),
            new Keyframe(duration * 0.5f, 0f),
            new Keyframe(duration * 0.75f, 20f),
            new Keyframe(duration, 0f)
        );
        leftArmRotation.postWrapMode = WrapMode.Loop;
        leftArmRotation.preWrapMode = WrapMode.Loop;
        
        AnimationCurve rightArmRotation = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(duration * 0.25f, 20f),
            new Keyframe(duration * 0.5f, 0f),
            new Keyframe(duration * 0.75f, -20f),
            new Keyframe(duration, 0f)
        );
        rightArmRotation.postWrapMode = WrapMode.Loop;
        rightArmRotation.preWrapMode = WrapMode.Loop;
        
        AnimationCurve bodyBounce = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(duration * 0.25f, -0.05f),
            new Keyframe(duration * 0.5f, 0f),
            new Keyframe(duration * 0.75f, -0.05f),
            new Keyframe(duration, 0f)
        );
        bodyBounce.postWrapMode = WrapMode.Loop;
        bodyBounce.preWrapMode = WrapMode.Loop;
        
        clip.SetCurve("Armature/Hips/Left leg", typeof(Transform), "localEulerAngles.x", leftLegRotation);
        clip.SetCurve("Armature/Hips/Right leg", typeof(Transform), "localEulerAngles.x", rightLegRotation);
        clip.SetCurve("Armature/Hips/Spine/Chest/Left shoulder/Left arm", typeof(Transform), "localEulerAngles.x", leftArmRotation);
        clip.SetCurve("Armature/Hips/Spine/Chest/Right shoulder/Right arm", typeof(Transform), "localEulerAngles.x", rightArmRotation);
        clip.SetCurve("Armature/Hips", typeof(Transform), "localPosition.y", bodyBounce);
        
        AssetDatabase.CreateAsset(clip, "Assets/Animations/Walking.anim");
        Debug.Log("Created Walking animation");
    }
    
    private static void CreateHandshakeAnimation()
    {
        AnimationClip clip = new AnimationClip();
        clip.name = "Handshake";
        clip.legacy = false;
        clip.wrapMode = WrapMode.Once;
        
        float duration = 3f;
        
        AnimationCurve rightArmRaise = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.5f, -45f),
            new Keyframe(2.5f, -45f),
            new Keyframe(duration, 0f)
        );
        
        AnimationCurve rightElbowBend = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.5f, -90f),
            new Keyframe(2.5f, -90f),
            new Keyframe(duration, 0f)
        );
        
        AnimationCurve shake = new AnimationCurve(
            new Keyframe(0.5f, 0f),
            new Keyframe(1f, 5f),
            new Keyframe(1.5f, -5f),
            new Keyframe(2f, 5f),
            new Keyframe(2.5f, 0f)
        );
        
        clip.SetCurve("Armature/Hips/Spine/Chest/Right shoulder/Right arm", typeof(Transform), "localEulerAngles.z", rightArmRaise);
        clip.SetCurve("Armature/Hips/Spine/Chest/Right shoulder/Right arm/Right elbow", typeof(Transform), "localEulerAngles.y", rightElbowBend);
        clip.SetCurve("Armature/Hips/Spine/Chest/Right shoulder/Right arm/Right elbow", typeof(Transform), "localEulerAngles.x", shake);
        
        AssetDatabase.CreateAsset(clip, "Assets/Animations/Handshake.anim");
        Debug.Log("Created Handshake animation");
    }
    
    private static void CreateAnimatorController()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Animations"))
        {
            AssetDatabase.CreateFolder("Assets", "Animations");
        }
        
        UnityEditor.Animations.AnimatorController controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/Animations/AvatarController.controller");
        
        UnityEditor.Animations.AnimatorControllerLayer layer = controller.layers[0];
        UnityEditor.Animations.AnimatorStateMachine stateMachine = layer.stateMachine;
        
        controller.AddParameter("Walking", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Handshake", AnimatorControllerParameterType.Bool);
        
        AnimationClip idleClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animations/Idle.anim");
        AnimationClip walkClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animations/Walking.anim");
        AnimationClip handshakeClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animations/Handshake.anim");
        
        UnityEditor.Animations.AnimatorState idleState = stateMachine.AddState("Idle");
        idleState.motion = idleClip;
        
        UnityEditor.Animations.AnimatorState walkState = stateMachine.AddState("Walking");
        walkState.motion = walkClip;
        
        UnityEditor.Animations.AnimatorState handshakeState = stateMachine.AddState("Handshake");
        handshakeState.motion = handshakeClip;
        
        stateMachine.defaultState = idleState;
        
        UnityEditor.Animations.AnimatorStateTransition idleToWalk = idleState.AddTransition(walkState);
        idleToWalk.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "Walking");
        idleToWalk.hasExitTime = false;
        idleToWalk.duration = 0.25f;
        
        UnityEditor.Animations.AnimatorStateTransition walkToIdle = walkState.AddTransition(idleState);
        walkToIdle.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, "Walking");
        walkToIdle.hasExitTime = false;
        walkToIdle.duration = 0.25f;
        
        UnityEditor.Animations.AnimatorStateTransition idleToHandshake = idleState.AddTransition(handshakeState);
        idleToHandshake.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "Handshake");
        idleToHandshake.hasExitTime = false;
        idleToHandshake.duration = 0.1f;
        
        UnityEditor.Animations.AnimatorStateTransition walkToHandshake = walkState.AddTransition(handshakeState);
        walkToHandshake.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "Handshake");
        walkToHandshake.hasExitTime = false;
        walkToHandshake.duration = 0.1f;
        
        UnityEditor.Animations.AnimatorStateTransition handshakeToIdle = handshakeState.AddTransition(idleState);
        handshakeToIdle.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, "Handshake");
        handshakeToIdle.hasExitTime = true;
        handshakeToIdle.exitTime = 0.9f;
        handshakeToIdle.duration = 0.25f;
        
        AssetDatabase.SaveAssets();
        Debug.Log("Created Animator Controller");
    }
    
    private static void AssignAnimatorsToAvatars()
    {
        UnityEditor.Animations.AnimatorController controller = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>("Assets/Animations/AvatarController.controller");
        
        GameObject avatar1 = GameObject.Find("Avater 1");
        if (avatar1 != null)
        {
            Animator animator1 = avatar1.GetComponent<Animator>();
            if (animator1 != null)
            {
                animator1.runtimeAnimatorController = controller;
                EditorUtility.SetDirty(avatar1);
                Debug.Log("Assigned Animator Controller to Avater 1");
            }
        }
        
        GameObject avatar2 = GameObject.Find("Avater 2");
        if (avatar2 != null)
        {
            Animator animator2 = avatar2.GetComponent<Animator>();
            if (animator2 != null)
            {
                animator2.runtimeAnimatorController = controller;
                EditorUtility.SetDirty(avatar2);
                Debug.Log("Assigned Animator Controller to Avater 2");
            }
        }
    }
}
