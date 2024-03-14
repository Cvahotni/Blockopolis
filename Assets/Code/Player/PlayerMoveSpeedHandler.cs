using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMoveSpeedHandler : MonoBehaviour
{
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform playerCamera;

    [SerializeField] private byte waterID;
    private PlayerEventSystem playerEventSystem;

    private void Start() {
        playerEventSystem = PlayerEventSystem.Instance;
    }

    private void FixedUpdate() {
        CheckUnderwater();
    }

    private void CheckUnderwater() {
        bool bodyUnderWater = WorldAccess.GetBlockAt(playerBody.position).id == waterID;
        bool cameraUnderWater = WorldAccess.GetBlockAt(playerCamera.position).id == waterID;

        if(bodyUnderWater) playerEventSystem.InvokePlayerEnterWater();
        if(!bodyUnderWater) playerEventSystem.InvokePlayerExitWater();
    }
}
