using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfLevel : MonoBehaviour
{

    public GameObject player;
    public GameObject pauseMenu;
    public GameObject endOfLevelMenu;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "EndOfLevelObject") 
        {
            Debug.Log("Hit the end of the level");
            endOfLevelMenu.SetActive(true);
            player.SetActive(false);

        }
    }
}
