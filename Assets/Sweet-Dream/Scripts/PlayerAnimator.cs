using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [FoldoutGroup("Variables")]
    private Animator anim;
    [FoldoutGroup("Variables")]
    private string currentAnim = "";
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
