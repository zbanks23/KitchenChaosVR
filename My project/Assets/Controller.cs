using UnityEngine;

public class Controller : MonoBehaviour
{
    public int points;
    public int timer = 0;
    public SceneChanger sceneLoader;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer++;
        if (timer == 1000)
        {
            points = 0;
            sceneLoader.LoadScene("MainScene");
        }
    }
}
