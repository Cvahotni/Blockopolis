using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerHand : MonoBehaviour
{
    public static PlayerHand Instance { get; private set; }

    [SerializeField] private GameObject handObject;
    
    private Animator handObjectAnimator;

    private PlayerMove playerMove;
    private ItemRegistry itemRegistry;

    private int currentHotbarSlot;

    private float currentWalkAnimatorTime;
    private float currentSwingAnimatorTime;

    private bool isSwingingRepeat;

    private WaitForSeconds shortTime = new WaitForSeconds(0.128f);
    private WaitForSeconds swingTime = new WaitForSeconds(0.161f);
    
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
        if(playerMove == null) return;
        if(handObjectAnimator == null) return;

        handObjectAnimator.SetBool("isWalking", playerMove.IsWalking);
    }

    public void SwitchHeldItem(object sender, SwitchedItemStack stack) {
        if(!handObject.activeSelf) return;
        AssignAnimator();

        handObjectAnimator.SetBool("isSwitchingUp", false);
        handObjectAnimator.SetBool("isSwitchingDown", true);

        handObjectAnimator.SetLayerWeight(1, 0.975f);
        StartCoroutine(SwitchHeldItemCoroutine(stack));
    }

    public void SwingHeldItem(BlockBreakData data) {
        AssignAnimator();
        handObjectAnimator.SetBool("isSwinging", true);

        ActivateSwingLayer();
        StartCoroutine(SwingItemHandReset());
    }

    public void SwingHeldItemRepeating(object sender, EventArgs e) {
        AssignAnimator();

        handObjectAnimator.SetBool("isSwinging", false);
        handObjectAnimator.SetBool("isSwingingRepeat", true);

        ActivateSwingLayer();
    }

    public IEnumerator SwitchHeldItemCoroutine(SwitchedItemStack switchedStack) {
        yield return shortTime;
        ItemStack stack = switchedStack.itemStack;

        if(itemRegistry == null) {
            Debug.LogError("The ItemRegistry script must be present in order to switch the held item.");
            yield return null;
        }

        float animatorSpeed = 1.0f / (Mathf.Abs(switchedStack.switchTime) * 20f);

        AssignAnimator();
        handObjectAnimator.speed = animatorSpeed;

        isSwingingRepeat = handObjectAnimator.GetBool("isSwingingRepeat");
        
        SetCurrentWalkAnimatorTime();
        SetCurrentSwingAnimatorTime();

        handObjectAnimator.SetBool("isSwitchingDown", false);
        Destroy(handObject);

        GameObject prefab = itemRegistry.GetItemHeldPrefabWithCount(stack.ID, stack.Amount);

        handObject = Instantiate(prefab, this.transform, false);
        Material heldItemMaterial = itemRegistry.GetMaterialForIDWithCount(stack.ID, stack.Amount);

        MeshRenderer meshRenderer = handObject.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = heldItemMaterial;

        AssignAnimator();
        UpdateAnimatorTime(1, currentWalkAnimatorTime);
        UpdateAnimatorTime(2, currentSwingAnimatorTime);

        handObjectAnimator.SetBool("isSwitchingUp", true);
        handObjectAnimator.SetBool("isSwingingRepeat", isSwingingRepeat);

        StartCoroutine(SwitchHeldItemHandReset());
        yield return null;
    }

    private IEnumerator SwitchHeldItemHandReset() {
        yield return shortTime;
        
        handObjectAnimator.SetBool("isSwitchingUp", false);
        handObjectAnimator.SetBool("isSwitchingDown", false);
    
        ResetLayers();
    }

    private IEnumerator SwingItemHandReset() {
        yield return swingTime;
        ResetHandSwing();
    }

    public void ResetLayers(object sender, BlockBreakData data) {
        ResetLayers();
    }

    public void ResetHandSwing(object sender, EventArgs e) {
        ResetHandSwing();
    }

    public void ResetHandSwing() {
        AssignAnimator();

        handObjectAnimator.SetBool("isSwinging", false);
        handObjectAnimator.SetBool("isSwingingRepeat", false);

        ResetLayers();
    }

    public void ResetLayers() {
        handObjectAnimator.SetLayerWeight(1, 1f);
        handObjectAnimator.SetLayerWeight(2, 0.8f);
    }

    private void ActivateSwingLayer() {
        handObjectAnimator.SetLayerWeight(2, 1f);
        handObjectAnimator.SetLayerWeight(1, 0.995f);
    }

    private void AssignAnimator() {
        handObjectAnimator = handObject.GetComponent<Animator>();

        if(handObjectAnimator == null) {
            Debug.LogError("The hand object must have an animator attatched to it.");
        }
    }

    private void UpdateAnimatorTime(int layer, float time) {
        AnimatorStateInfo stateInfo = handObjectAnimator.GetCurrentAnimatorStateInfo(layer);
        float normalizedTime = time / stateInfo.length;

        handObjectAnimator.PlayInFixedTime(stateInfo.fullPathHash, layer, time);
    }

    private void SetCurrentWalkAnimatorTime() {
        AnimatorStateInfo animatorState = handObjectAnimator.GetCurrentAnimatorStateInfo(1);
        currentWalkAnimatorTime = animatorState.normalizedTime;
    }

    private void SetCurrentSwingAnimatorTime() {
        AnimatorStateInfo animatorState = handObjectAnimator.GetCurrentAnimatorStateInfo(2);
        currentSwingAnimatorTime = animatorState.normalizedTime;
    }
}
