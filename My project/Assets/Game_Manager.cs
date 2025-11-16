using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Game_Manager : MonoBehaviour
{
    public int points;
    public int timer = 0;
    public bool first = true;
    public TextMeshProUGUI pointDisplay;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        addPoints(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(points>500)
        {
            LoadScene("BreakScene");
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void addPoints(int points)
    {
        this.points = points;
        pointDisplay.text = "Points: " + points;
    }
}
