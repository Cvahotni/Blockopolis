using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public static PlayerHand Instance { get; private set; }

    [SerializeField] private GameObject handObject;
    
    private Animator handObjectAnimator;

    private PlayerMove playerMove;
    private ItemRegistry itemRegistry;

    private int currentHotbarSlot;
    private float currentAnimatorTime;
    
    public int CurrentHotbatSlot { set { currentHotbarSlot = value; }}

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        playerMove = PlayerMove.Instance;
        itemRegistry = ItemRegistry.Instance;
    }

    private void Update() {
        UpdateAnimator();
    }

    private void UpdateAnimator() {
        if(handObjectAnimator == null) return;
        handObjectAnimator.enabled = playerMove.IsWalking;
    }

    public void SwitchHeldItem(ushort id, ushort count) {
        handObjectAnimator = handObject.GetComponent<Animator>();
        currentAnimatorTime = GetAnimatorTime(0);
        
        Destroy(handObject);
        GameObject prefab = itemRegistry.GetItemHeldPrefabWithCount(id, count);

        handObject = Instantiate(prefab, this.transform, false);
        Material heldItemMaterial = itemRegistry.GetMaterialForIDWithCount(id, count);

        MeshRenderer meshRenderer = handObject.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = heldItemMaterial;

        handObjectAnimator = handObject.GetComponent<Animator>();
        handObjectAnimator.Update(currentAnimatorTime);
    }

    private float GetAnimatorTime(int index) {
        AnimatorStateInfo animationState = handObjectAnimator.GetCurrentAnimatorStateInfo(index);
		AnimatorClipInfo[] animatorClip = handObjectAnimator.GetCurrentAnimatorClipInfo(index);
		
        return animatorClip[index].clip.length * animationState.normalizedTime;
    }
}
