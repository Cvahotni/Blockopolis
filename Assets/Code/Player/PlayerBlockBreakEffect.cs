using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlockBreakEffect : MonoBehaviour
{
    public static PlayerBlockBreakEffect Instance { get; private set; }

    [SerializeField]
    private ParticleSystem blockBreakParticle;

    private ItemRegistry itemRegistry;
    private float destroyDelay = 5.0f;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        itemRegistry = ItemRegistry.Instance;
    }

    public void PlayBlockBreakParticle(ushort block, Vector3 position) {
        ParticleSystem particleSystem = Instantiate(blockBreakParticle, position, blockBreakParticle.transform.rotation);
        particleSystem.transform.position = position;

        Sprite particleSprite = itemRegistry.GetSpriteForID(block);
        var shape = particleSystem.shape;

        shape.sprite = particleSprite;

        particleSystem.Simulate(0.0f, true, true);
        particleSystem.Play(true);

        StartCoroutine(DestoryBlockBreakParticle(particleSystem.gameObject));
    }

    private IEnumerator DestoryBlockBreakParticle(GameObject gameObject) {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
