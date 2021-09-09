using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class GameUI : MonoBehaviour
{
    public PlayerUIContainer[] playerContainers;
    public TextMeshProUGUI winText;

    public static GameUI instance;

    public Slider timeSlider;

    public Material spectatorMat;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        timeSlider.maxValue = GameManager.instance.timeToWin;
        timeSlider.value = GameManager.instance.timeToWin;
        InitializePlayerUI();
    }
    private void Update()
    {
        timeSlider.value -= Time.deltaTime;
    }

    void InitializePlayerUI()
    {      
        for(int i = 0; i < playerContainers.Length; ++i)
        {
            PlayerUIContainer container = playerContainers[i];
            
            if (i < PhotonNetwork.PlayerList.Length)
            {
                container.obj.SetActive(true);
                container.nameText.text = PhotonNetwork.PlayerList[i].NickName;
                container.nameText.faceColor = container.colorDenoter.color;
            }
            else
                container.obj.SetActive(false);
        }
    }

    void UpdatePlayerUI()
    {
        //for (int i = 0; i < GameManager.instance.players.Length; ++i)
        //{
        //    if (GameManager.instance.players[i] != null)
        //        playerContainers[i].hatTimeSlider.value = GameManager.instance.players[i].currHatTime;
        //}
    }
    //public void MakeSpectatorsUI()
    //{
    //    for (int i = 0; i < playerContainers.Length; ++i)
    //    {
    //        PlayerUIContainer container = playerContainers[i];

    //        if (i < PhotonNetwork.PlayerList.Length)
    //        {
    //            print("playerlist: " + PhotonNetwork.PlayerList.Length);
    //            print("player length: " + GameManager.instance.players.Length);
    //            print("i: " + i);
    //            print(GameManager.instance);
    //            print(GameManager.instance.players[i]);
    //            if (GameManager.instance.players[i] != null && GameManager.instance.players[i].isSpectator)
    //            {
    //                container.nameText.faceColor = spectatorMat.color;
    //            }
    //        }
    //    }
    //}

    public void SetWinText(string winnerName)
    {
        winText.gameObject.SetActive(true);
        winText.text = winnerName + " wins!";
    }

    //public void SetInactive(int playerID)
    //{
    //    foreach(PlayerController p in GameManager.instance.players)
    //    {
    //        if(p.id == playerID)
    //        {
    //            p.isSpectator = true;
    //        }
    //    }
    //}
}

[System.Serializable]
public class PlayerUIContainer
{
    public GameObject obj;
    public TextMeshProUGUI nameText;
    public Material colorDenoter;
}
