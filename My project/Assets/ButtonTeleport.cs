using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class ButtonTeleport : MonoBehaviour {
    [Header("Settings")]
    [Tooltip("Drag your XR Origin here")]
    public TeleportationProvider teleportationProvider;

    [Tooltip("The Transform where the player should land")]
    public Transform targetDestination;

    // Call this function from your UI Button OnClick() event
    public void PerformTeleport() {
        if (teleportationProvider == null || targetDestination == null) {
            Debug.LogError("Teleport Provider or Target is missing!");
            return;
        }

        // Create a teleport request
        TeleportRequest request = new TeleportRequest() {
            destinationPosition = targetDestination.position,
            destinationRotation = targetDestination.rotation,
            matchOrientation = MatchOrientation.TargetUpAndForward
        };

        // Queue the teleport
        teleportationProvider.QueueTeleportRequest(request);
    }
}