using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CustomerSpawner : MonoBehaviour {
    public static CustomerSpawner Instance { get; private set; }

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }

        isSeatTaken.Clear();
        foreach (Transform seat in transform) {
            isSeatTaken[seat] = false;
        }

        
    }

    public GameObject[] customers;
    Dictionary<Transform, bool> isSeatTaken = new Dictionary<Transform, bool>();
    Dictionary<GameObject, Transform> customerSeats = new Dictionary<GameObject, Transform>();

    public void MoveCustomer(GameObject customer) {
        Transform oldSeat = customerSeats[customer];
        isSeatTaken[oldSeat] = false;
        SeatCustomer(customer, oldSeat);
    }

    void SeatCustomer(GameObject customer, Transform oldSeat = null) {
        Transform seat = transform.GetChild(Random.Range(0, transform.childCount));
        customer.transform.position = seat.position;
        customer.transform.rotation = seat.rotation;

        if (isSeatTaken[seat] || seat == oldSeat) {
            SeatCustomer(customer, oldSeat);
            return;
        }

        customerSeats[customer] = seat;
        isSeatTaken[seat] = true;

        customer.GetComponent<CustomerOrder>().GenerateOrder();

        CustomerOrder.Meal newOrder = customer.GetComponent<CustomerOrder>().GetDesiredMeal();

        if (FoodSpawner.Instance != null) {
            FoodSpawner.Instance.RequestFood(newOrder);
        }

        customer.GetComponent<CustomerAI>().SpawnMyServingZone();

        if (seat.gameObject.tag == "FaceLeft") customer.GetComponent<Animator>().Play("left facing seated");
        else customer.GetComponent<Animator>().Play("right facing seated");
    }

    void Start() {
        foreach (GameObject customer in customers) {
            SeatCustomer(customer);
        }
    }
}