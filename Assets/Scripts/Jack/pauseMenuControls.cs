using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*public class pauseMenuControls : MonoBehaviour
{

    private PlayerInputsControls playerControls;
    private InputAction menu;

    [SerializeField] private GameObject pauseUI;
    [SerializeField] private bool isPaused;

    void Awake()
    {
        playerControls = new PlayerControls();

    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        menu = playerControls.Menu.Escape;
        menu.Enable();

        menu.performed += Pause();
    }

    private void OnDisable()
    {
        menu.Disable();
    }

    public void Pause(InputAction.CallbackContext context)
    {
        isPaused = !isPaused;

        if(ifPaused)
        {
            ActivateMenu();
        }
        else
        {
            DeactivateMenu();
        }

        void ActivateMenu()
        {
            Time.timescale = 0;
            AudioListener.pause = true;
            pauseUI.SetActive(true);
        }

        void DeactivateMenu()
        {
            Time.timescale = 1;
            AudioListener.pause = false;
            pauseUI.SetActive(false);
            isPaused = false;
        }
    }

}*/
