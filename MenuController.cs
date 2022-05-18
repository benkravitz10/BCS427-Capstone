using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;


public class MenuController : MonoBehaviourPunCallbacks
{
    public GameObject OptionsMenu;
    public GameObject Rules;
    public GameObject Buttons;
    public GameObject loading;
    private string lobbyName = "test";
    private string sceneName = "Game";


    public void Regular()
    {
        Buttons.SetActive(false);
        loading.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();

    }
    public void Obstacle()
    {
        Buttons.SetActive(false);
        loading.SetActive(true);
        lobbyName = "test2";
        sceneName = "Game2";
        PhotonNetwork.ConnectUsingSettings();
    }
    public void HowToPlay()
    {
        OptionsMenu.SetActive(false);
        Rules.SetActive(true);

    }
    public void GoBack()
    {
        OptionsMenu.SetActive(true);
        Rules.SetActive(false);
    }
    
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinOrCreateRoom(lobbyName, new RoomOptions(), TypedLobby.Default);
    }
    
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}