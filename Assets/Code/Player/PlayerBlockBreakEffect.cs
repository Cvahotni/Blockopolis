using System.Collections;
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

    public void PlayBlockBreakParticle(object sender, BlockModifyData data) {
        if(itemRegistry == null) {
            Debug.LogError("The ItemRegistry script must be present in order to play a block break particle.");
            return;
        }

        Vector3 position = new Vector3(data.x, data.y, data.z);

        ParticleSystem particleSystem = Instantiate(blockBreakParticle, position, blockBreakParticle.transform.rotation);
        particleSystem.transform.position = position;
        
        ParticleSystemRenderer particleSystemRenderer = particleSystem.gameObject.GetComponent<ParticleSystemRenderer>();
        
        Material crackMaterial = itemRegistry.GetBreakMaterialForID(data.block.id);
        particleSystemRenderer.material = crackMaterial;

        Sprite particleSprite = itemRegistry.GetBreakSpriteForID(data.block.id);
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
