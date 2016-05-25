﻿/*
**  GameManager.cs: Holds variables and functions that need to be accessed from multiple scripts
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject localPlayer;
    //UI attack slots
    public AttackSlots attackSlots;

    private Text startText;
    private string startTextString;

    void Awake()
    {
        instance = this;

        if (GameObject.Find("StartText"))
        {
            startText = GameObject.Find("StartText").GetComponent<Text>();
            startTextString = startText.text;
        }
    }

    //Initilises game by calling required initilisers
    public void Initialise()
    {
        if (localPlayer)
        {
            attackSlots.InitialiseSlots(localPlayer.GetComponent<PlayerAttack>());
        }
    }
}