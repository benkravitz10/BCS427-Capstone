using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class player : MonoBehaviour
{
    public PhotonView photonView;
    public GameObject PlayerCamera;
    
    private void Awake()
    {
        if(photonView.IsMine)
        {
            PlayerCamera.SetActive(true);
        }
    }
}
