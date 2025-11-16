using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CustomerOrder))]
public class CustomerAI : MonoBehaviour {
    public GameObject servingZonePrefab;
    public Transform servingZoneLocation;
    public Animator animator;
    public float timeToEat = 3.0f;

    private CustomerOrder myOrder;
    private ServingZone myServingZone;

    void Awake() {
        myOrder = GetComponent<CustomerOrder>();
    }

    void Start() {
    }

    public void SpawnMyServingZone() {
        if (servingZonePrefab == null || servingZoneLocation == null) {
            Debug.LogError("Customer is missing Serving Zone Prefab or Location!");
            return;
        }

        CustomerOrder.Meal desiredMeal = myOrder.GetDesiredMeal();

        GameObject zoneObject = Instantiate(servingZonePrefab, servingZoneLocation.position, servingZoneLocation.rotation);
        myServingZone = zoneObject.GetComponent<ServingZone>();

        if (myServingZone != null) {
            myServingZone.InitializeZone(this, desiredMeal);
        }
    }

    public void ReceiveFood(GameObject foodPlate) {
        Debug.Log(this.name + " received food!");

        if (animator != null) {
            animator.SetTrigger("EatFood");
        }

        Destroy(foodPlate, 0.5f);

        StartCoroutine(LeaveAfterEating());
    }

    IEnumerator LeaveAfterEating() {
        yield return new WaitForSeconds(timeToEat);

        CustomerSpawner.Instance.MoveCustomer(this.gameObject);
    }
}