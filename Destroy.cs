using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Destroy : MonoBehaviour
{
    //this is on every ship and is called to make things die
    public GameObject explosionPrefab;

    //punrpc allows it to be called from anywhere and allow both the host and user to have it
    //this is important
    [PunRPC]
    public void die()
    {
        StartCoroutine(death());
    }
    IEnumerator death()
    {
        yield return new WaitForSeconds(2.15f);
        Vector3 offset = new Vector3(-2.06f, 0.43f, -3.39f);
        Instantiate (explosionPrefab, transform.position + offset, transform.rotation);
        GameManager.gm.shootSound.Play(0);
        Destroy(gameObject);
    }
}
