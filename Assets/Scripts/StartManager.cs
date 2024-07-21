using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    public Button rulesButton;
    public Button startButton;
    public GameObject startCanva;
    public GameObject rulesCanva;
    public Dropdown diffucltyDrodown;

    public static string difficulty = "medium";

    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        rulesButton.onClick.AddListener(ShowRules);
        rulesCanva.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowRules()
    {
        startCanva.SetActive(false);
        rulesCanva.SetActive(true);
    }

    public void StartGame()
    {
        Debug.Log("Game Started");

        Debug.Log(diffucltyDrodown.value);


        switch (diffucltyDrodown.value)
        {
            case 0:
                difficulty = "medium";
                break;
            case 1:
                difficulty = "easy";
                break;
            case 2:
                difficulty = "hard";
                break;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("map");
    }
}
