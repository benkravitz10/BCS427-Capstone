using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public enum turn {blue1, blue2, red1, red2, none};
    public turn myTurn;
    public turn lastTurn;
    Vector3 SpawnPoint = new Vector3(0,0,0);
    public Text timer;
    public Text blueTurn;
    public Text redTurn;
    public Text gameOver;
    private bool started = false;
    private float myTime;
    public float maxTime;
    public GameObject bluePlayerPrefab;
    public GameObject redPlayerPrefab;
    public GameObject spectatePrefab;
    public GameObject GameCanvas;
    public GameObject mainCamera;
    //to tell if there is already a blue player
    private bool bluePlayer = false;
    //to tell if there is already a red player
    private bool redPlayer = false;
    public AudioSource shootSound;
    public AudioSource FlySound;
    
    void Awake()
    {
        if(gm != null && gm != this)
        {
            gameObject.SetActive(false);
        }
        else{
            gm = this;
        }
        
        GameCanvas.SetActive(true);
    }
    private void FixedUpdate()
    {
        if(myTime > 0)
        {
            myTime -= .02f;
            timer.text = "Timer: " + Mathf.Round(myTime);
        }
        if(started&& myTime <= 0)
        {
            NextTurn();
            myTurn = lastTurn;
            myTime = maxTime;
        }


        //see if game is done
        GameObject[] team;
        int counter = 0;
        team = GameObject.FindGameObjectsWithTag("BlueTeam");
        foreach (GameObject ally in team)
        {
            counter ++;
        }
        if(counter < 2)
        {
            EndGame("Blue");
        }
        counter = 0;
        team = GameObject.FindGameObjectsWithTag("RedTeam");
        foreach (GameObject ally in team)
        {
            counter ++;
        }
        if(counter < 2)
        {
            EndGame("Red");
        }
    }
    void Start()
    {
        //gm = this.gameObject.GetComponent<GameManager>();
        myTurn = turn.blue1;
        lastTurn = turn.blue1;
    }

//when a player joins as blue player
    public void BluePlayer()
    {
        if(!bluePlayer)
        {
            GameObject obj = PhotonNetwork.Instantiate(bluePlayerPrefab.name, SpawnPoint, Quaternion.identity);
        }
        else{GameObject obj = PhotonNetwork.Instantiate(spectatePrefab.name, SpawnPoint, Quaternion.identity);}
        GameCanvas.SetActive(false);
        mainCamera.SetActive(false);
    }
    //when a player joins as red player
    public void RedPlayer()
    {
        if(!redPlayer)
        {
            GameObject obj = PhotonNetwork.Instantiate(redPlayerPrefab.name, SpawnPoint, Quaternion.identity);
        }
        else{GameObject obj = PhotonNetwork.Instantiate(spectatePrefab.name, SpawnPoint, Quaternion.identity);}
        GameCanvas.SetActive(false);
        mainCamera.SetActive(false);
    }
    //punrpc allows it to be called from everywhere and works on both host and other players' game managers
    [PunRPC]
    public void NextTurn()
    {
        if(!started)
        {
            //Debug.Log("should enable");
            blueTurn.gameObject.SetActive(true);
            timer.gameObject.SetActive(true);
            myTime = maxTime;
            started = true;
        }
        //if(pView.IsMine)
        {
        switch(lastTurn)
        {
            case turn.blue1:
                myTime = maxTime;
                myTurn = turn.none;
                lastTurn = turn.blue2;
                break;
            case turn.blue2:
                myTime = maxTime;
                myTurn = turn.none;
                lastTurn = turn.red1;
                blueTurn.gameObject.SetActive(false);
                redTurn.gameObject.SetActive(true);
                break;
            case turn.red1:
                myTime = maxTime;
                myTurn = turn.none;
                lastTurn = turn.red2;
                break;
            case turn.red2:
                myTime = maxTime;
                myTurn = turn.none;
                lastTurn = turn.blue1;
                redTurn.gameObject.SetActive(false);
                blueTurn.gameObject.SetActive(true);
                break;
        }
        }
    }
    void EndGame(string winner)
    {
        timer.gameObject.SetActive(false);
        blueTurn.gameObject.SetActive(false);
        redTurn.gameObject.SetActive(false);
        GameObject[] team;
        team = GameObject.FindGameObjectsWithTag("BlueTeam");
        foreach (GameObject ally in team)
        {
            Destroy(ally);
        }
        team = GameObject.FindGameObjectsWithTag("RedTeam");
        foreach (GameObject ally in team)
        {
            Destroy(ally);
        }
        team = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obstacle in team)
        {
            Destroy(obstacle);
        }
        gameOver.gameObject.SetActive(true);
        gameOver.text = "Game Over\n" + winner + "Player Wins";
        StartCoroutine(restart());
    }
    IEnumerator restart()
    {
        yield return new WaitForSeconds(6f);
        PhotonNetwork.LoadLevel("MainMenu");
    }
}
