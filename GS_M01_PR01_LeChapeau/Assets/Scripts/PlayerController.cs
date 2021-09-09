using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector]
    public int id;

    [Header("Info")]
    public float moveSpeed;
    public float jumpForce;
    public TNTController tnt;

   // [HideInInspector]
    //public float currHatTime;

    [Header("Components")]
    public Rigidbody rb;
    public Player photonPlayer;
    
    [Header("Spectator")]
    public bool isSpectator = false;


    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;

        GameManager.instance.players[id - 1] = this;
        GameManager.instance.activePlayers.Add(this);

        GetComponent<Renderer>().material = GameUI.instance.playerContainers[id - 1].colorDenoter;

        if(id == 1)
            GameManager.instance.GiveTNT(id, true);

        if (!photonView.IsMine)
            rb.isKinematic = true;

    }

    private void Update()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            if(GameUI.instance.timeSlider.value <= 0 && !GameManager.instance.gameEnded)
            {
                if (GameManager.instance.activePlayers.Count != 1)
                {
                    GameManager.instance.photonView.RPC("EliminatePlayer", RpcTarget.All, GameManager.instance.playerWithTNT);
                    //GameManager.instance.photonView.RPC("GiveTNT", RpcTarget.AllViaServer, GameManager.instance.activePlayers[0].id, false);
                }                
            }
        }

        if(photonView.IsMine)
        {
            Move();

            if (Input.GetKeyDown(KeyCode.Space))
                TryJump();

            //if (hatObject.activeInHierarchy)
                //currHatTime += Time.deltaTime;
        }
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal") * moveSpeed;
        float z = Input.GetAxis("Vertical") * moveSpeed;

        rb.velocity = new Vector3(x, rb.velocity.y, z);
    }

    void TryJump ()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if(Physics.Raycast(ray, 0.7f))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void SetTNT(bool hasTNT)
    {
        GetComponent<TNTController>().enabled = hasTNT;


        if (GetComponent<TNTController>().enabled == false)
        {
            GetComponent<Renderer>().material = GameUI.instance.playerContainers[id - 1].colorDenoter;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine)
            return;

        if(collision.gameObject.CompareTag("Player"))
        {
            if(GameManager.instance.GetPlayer(collision.gameObject).id == GameManager.instance.playerWithTNT)
            {
                if(GameManager.instance.CanGetTNT())
                {
                    GameManager.instance.photonView.RPC("GiveTNT", RpcTarget.All, id, false);
                }
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(currHatTime);
        }
        else if (stream.IsReading)
        {
            //currHatTime = (float)stream.ReceiveNext();
        }
    }
}

