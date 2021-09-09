using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    public bool gameEnded = false;
    public float timeToWin;
    public float invincDur;
    private float tntPickupTime;

    [Header("Players")]
    public string playerPrefabLocation;
    public Transform[] spawnPoints;
    public PlayerController[] players;
    public List<PlayerController> activePlayers;
    public int playerWithTNT;
    private int playersInGame;

    public static GameManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
            gameObject.SetActive(false);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            GoBackToMenu();
        }
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;

        if (playersInGame == PhotonNetwork.PlayerList.Length)
            SpawnPlayer();
    }

    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);

        PlayerController playerScript = playerObj.GetComponent<PlayerController>();

        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);        
    }

    public PlayerController GetPlayer (int playerID)
    {
        return players.First(x => x.id == playerID);
    }
    public PlayerController GetPlayer (GameObject playerObj)
    {
        return players.First(x => x.gameObject == playerObj);
    }

    [PunRPC]
    public void GiveTNT(int playerID, bool initialGive)
    {
        if (!initialGive)
        {
            GetPlayer(playerWithTNT).SetTNT(false);
        }

        playerWithTNT = playerID;
        GetPlayer(playerID).SetTNT(true);
        tntPickupTime = Time.time;
    }

    public bool CanGetTNT()
    {
        if (Time.time > tntPickupTime + invincDur)
            return true;
        else
            return false;
    }

    [PunRPC]
    void WinGame(int playerID)
    {
        gameEnded = true;
        PlayerController player = GetPlayer(playerID);
        GameUI.instance.SetWinText(player.photonPlayer.NickName);

        Invoke("GoBackToMenu", 3.0f);
    }

    [PunRPC]
    void EliminatePlayer(int playerID)
    {
        PlayerController player = GetPlayer(playerID);
        activePlayers.Remove(player);

        activePlayers.OrderBy(x => x.id);
        player.transform.position = new Vector3(50f, -50f, 50f);
        player.rb.isKinematic = true;
        player.GetComponent<TNTController>().enabled = false;

        if (activePlayers.Count == 1)
        {
            gameEnded = true;
            photonView.RPC("WinGame", RpcTarget.All, activePlayers[0].id);
            return;
        }

        
        GameUI.instance.timeSlider.value = GameManager.instance.timeToWin;

        photonView.RPC("GiveTNT", RpcTarget.All, activePlayers[0].id, false);

        print(activePlayers.Count);
        
    }

    void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
    }

}
