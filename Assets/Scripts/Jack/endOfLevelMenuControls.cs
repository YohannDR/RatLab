using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class endOfLevelMenuControls : MonoBehaviour
{
    void OnEnable()
    {
        Debug.Log("EOL Menu activated");
        AudioManager.Instance.PlaySound("WinJingle");
    }

    public Button quitButton;
    public Button continueButton;

    void Start()
    {
        Button btn1 = quitButton.GetComponent<Button>();
        Button btn2 = continueButton.GetComponent<Button>();
        btn1.onClick.AddListener(ReturnToMenu);
        btn2.onClick.AddListener(NextLevel);
    }

    void ReturnToMenu()
    {
        Debug.Log("return to menu");
        SceneManager.LoadScene(0);
    }

    void NextLevel()
    {
        Debug.Log("continue to next level");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //Sound

    


}
