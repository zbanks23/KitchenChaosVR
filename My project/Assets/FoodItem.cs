using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class FoodItem : MonoBehaviour {
    public CustomerOrder.Meal foodType;

    private XRGrabInteractable grabInteractable;
    private ServingZone currentZone = null;

    public CustomerOrder.Meal GetFoodType() {
        return foodType;
    }
    private void Awake() {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null) {
            grabInteractable.selectExited.AddListener(OnDropped);
        }
    }

    private void OnTriggerEnter(Collider other) {
        ServingZone zone = other.GetComponent<ServingZone>();
        if (zone != null) {
            currentZone = zone;
        }
    }

    private void OnTriggerExit(Collider other) {
        ServingZone zone = other.GetComponent<ServingZone>();
        if (zone != null && zone == currentZone) {
            currentZone = null;
        }
    }

    private void OnDropped(SelectExitEventArgs args) {
        if (currentZone != null) {
            // Tell the zone to check this food item
            currentZone.TryAcceptFood(this);
        }
    }
}