using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameBoardPiece : MonoBehaviour
{
    //this names each board piece to its location
    private float myX;
    private float myY;
    void Awake()
    {
        myX = Mathf.Round(this.transform.position.x+2.098554f);
        myY = Mathf.Round(this.transform.position.z+3.351851f);
        this.gameObject.name = (myX) + " " + (myY);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
