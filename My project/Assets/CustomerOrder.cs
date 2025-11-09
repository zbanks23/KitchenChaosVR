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

    public void GenerateOrder()
    {
        desiredMeal = (Meal)Random.Range(0, System.Enum.GetValues(typeof(Meal)).Length);
        textBox.text = desiredMeal.ToString();
    }
}
