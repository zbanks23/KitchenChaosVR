using UnityEngine;

public class ServingZone : MonoBehaviour {
    private CustomerAI ownerCustomer;

    private CustomerOrder.Meal requiredFood;

    public void InitializeZone(CustomerAI owner, CustomerOrder.Meal desiredFood) {
        ownerCustomer = owner;
        requiredFood = desiredFood;
        Debug.Log("Zone initialized, waiting for: " + desiredFood.ToString());
    }

    // This is called by the FoodItem when it's dropped in
    public void TryAcceptFood(FoodItem item) {
        if (ownerCustomer == null) {
            Debug.LogError("This zone has no owner!");
            return;
        }

        if (item.GetFoodType() == requiredFood) {
            Debug.Log("Correct food delivered!");

            ownerCustomer.ReceiveFood(item.gameObject);

            Destroy(this.gameObject);
        } else {
            Debug.Log("Wrong food! Wanted " + requiredFood.ToString() + ", got " + item.GetFoodType().ToString());
        }
    }
}