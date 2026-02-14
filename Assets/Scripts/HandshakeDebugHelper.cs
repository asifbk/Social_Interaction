using UnityEngine;

public class HandshakeDebugHelper : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform avatar1;
    [SerializeField] private Transform avatar2;
    
    private void Update()
    {
        if (avatar1 != null && avatar2 != null)
        {
            float distance = Vector3.Distance(avatar1.position, avatar2.position);
            
            if (Input.GetKeyDown(KeyCode.H))
            {
                Debug.Log("═══════════════════════════════════════");
                Debug.Log("HANDSHAKE DEBUG INFO:");
                Debug.Log($"Distance between avatars: {distance:F2} units");
                Debug.Log($"Avatar 1 position: {avatar1.position}");
                Debug.Log($"Avatar 2 position: {avatar2.position}");
                
                AvatarController controller1 = avatar1.GetComponent<AvatarController>();
                AvatarController controller2 = avatar2.GetComponent<AvatarController>();
                
                if (controller1 != null)
                {
                    Debug.Log($"Avatar 1 - IsWalking: {controller1.IsWalking}, IsHandshaking: {controller1.IsHandshaking}");
                }
                
                if (controller2 != null)
                {
                    Debug.Log($"Avatar 2 - IsWalking: {controller2.IsWalking}, IsHandshaking: {controller2.IsHandshaking}");
                }
                
                ProceduralAnimator anim1 = avatar1.GetComponent<ProceduralAnimator>();
                ProceduralAnimator anim2 = avatar2.GetComponent<ProceduralAnimator>();
                
                Debug.Log($"ProceduralAnimator 1: {(anim1 != null ? "Found" : "NOT FOUND")}");
                Debug.Log($"ProceduralAnimator 2: {(anim2 != null ? "Found" : "NOT FOUND")}");
                Debug.Log("═══════════════════════════════════════");
            }
        }
    }
    
    private void OnGUI()
    {
        if (avatar1 != null && avatar2 != null)
        {
            float distance = Vector3.Distance(avatar1.position, avatar2.position);
            
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 20;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.UpperLeft;
            
            GUI.Label(new Rect(10, 10, 400, 30), $"Distance: {distance:F2} units", style);
            
            if (distance <= 3.0f)
            {
                style.normal.textColor = Color.green;
                GUI.Label(new Rect(10, 35, 400, 30), "✓ Close enough! Press H to handshake", style);
            }
            else
            {
                style.normal.textColor = Color.red;
                GUI.Label(new Rect(10, 35, 400, 30), "✗ Too far! Move closer", style);
            }
        }
    }
}
