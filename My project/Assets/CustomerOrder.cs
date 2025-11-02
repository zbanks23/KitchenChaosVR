using UnityEngine;
using TMPro;

public class CustomerOrder : MonoBehaviour
{
    public enum Meal
    {
        Burger,
        Curry,
        HotDog,
        Pancake,
        Pizza,
        Sushi,
        Udon
    }

    private Meal desiredMeal;

    public TextMeshProUGUI textBox;

    public Sprite[] mealIcons;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        desiredMeal = (Meal)Random.Range(0, System.Enum.GetValues(typeof(Meal)).Length);
        textBox.text = desiredMeal.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
