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
        UnityEngine.SceneManagement.SceneManager.LoadScene("map");
    }
}
