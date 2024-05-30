using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    private PlayerController playerController;
    private Player player;


    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Climbing();
        }
    }

    public void Climbing()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !playerController.isClimbing)
        {
            if (playerController.IsFrontBlocked())
            {
                StartCoroutine(playerController.ClimbWall());                    
            }
        }
    }
}
