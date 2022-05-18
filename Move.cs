using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Move : MonoBehaviour
{
    private float startX;
    private float startZ;
    private float endX;
    private float endZ;
    private float degreePer;
    //this makes the piece move by going in order of the functions
    [PunRPC]
    public void MovePiece(float myendx, float myendz)
    {
        GameManager.gm.FlySound.Play(0);
        startX = this.transform.position.x;
        startZ = this.transform.position.z;
        endX = myendx;
        endZ = myendz;
        StartCoroutine(Moveup());
    }
    
    IEnumerator Moveup()
    {
        int x = 0;
        while(x < 30)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + .05666f, this.transform.position.z);
            yield return new WaitForSeconds(.01f);
            x++;
        }
    
        StartCoroutine(Rotate());
    }
    IEnumerator Rotate()
    {
        int x = 0;
        float movex = (endX - startX);
        float movez = (endZ - startZ);
        degreePer = (Mathf.Rad2Deg*Mathf.Atan(movex/movez))/30;
        Transform childTransform = transform.GetChild(0);
        //blue team
        if(transform.rotation.y == 1)
        {
            if(movex <= 0 && movez < 0)
            {
                degreePer += -(180f/30f);
            }
            if(movex > 0 && movez <= 0)
            {
                degreePer += (180f/30f);
            }
            if((movex==0) || (movez == 0))
            {
                if(movex < 0 && movez == 0)
                {
                    degreePer = -(90f/30f);
                }
                if(movex == 0 && movez < 0)
                {
                    degreePer = -(180f/30f);
                }
                if(movex > 0 && movez == 0)
                {
                    degreePer = (90f/30f);
                }
            }
        }
        //red team
        if(transform.rotation.y == 0)
        {
            
            if(movex < 0 && movez > 0)
            {
                degreePer += (180f/30f);
            }
            if(movex > 0 && movez > 0)
            {
                degreePer += -(180f/30f);
            }
            if((movex==0) || (movez == 0))
            {
                if(movex < 0 && movez == 0)
                {
                    degreePer = (90f/30f);
                }
                if(movex == 0 && movez > 0)
                {
                    degreePer = (180f/30f);
                }
                if(movex > 0 && movez == 0)
                {
                    degreePer = -(90f/30f);
                }
            }
        }
        while(x < 30)
        {
            if(!((movex == 0) && (movez == 0)))
            {
                childTransform.Rotate(0f, degreePer, 0f, Space.Self);
            }
            yield return new WaitForSeconds(.01f);
            x++;
        }
        StartCoroutine(Moveposition());
    }
    IEnumerator Moveposition()
    {
        int x = 0;
        float movexPer = (endX - startX)/30;
        float movezPer = (endZ - startZ)/30;
        while(x < 30)
        {
            this.transform.position = new Vector3(this.transform.position.x + movexPer, this.transform.position.y, this.transform.position.z + movezPer);
            yield return new WaitForSeconds(.01f);
            x++;
        }
        StartCoroutine(RotateBack());
    }
    IEnumerator RotateBack()
    {
        int x = 0;
        float movex = (endX - startX);
        float movez = (endZ - startZ);
        degreePer = (Mathf.Rad2Deg*Mathf.Atan(movex/movez))/30;
        Transform childTransform = transform.GetChild(0);
        //blue team
        if(transform.rotation.y == 1)
        {
            if(movex <= 0 && movez < 0)
            {
                degreePer += -(180f/30f);
            }
            if(movex > 0 && movez <= 0)
            {
                degreePer += (180f/30f);
            }
            if((movex==0) || (movez == 0))
            {
                if(movex < 0 && movez == 0)
                {
                    degreePer = -(90f/30f);
                }
                if(movex == 0 && movez < 0)
                {
                    degreePer = -(180f/30f);
                }
                if(movex > 0 && movez == 0)
                {
                    degreePer = (90f/30f);
                }
            }
        }
        //red team
        if(transform.rotation.y == 0)
        {
            
            if(movex < 0 && movez > 0)
            {
                degreePer += (180f/30f);
            }
            if(movex > 0 && movez > 0)
            {
                degreePer += -(180f/30f);
            }
            if((movex==0) || (movez == 0))
            {
                if(movex < 0 && movez == 0)
                {
                    degreePer = (90f/30f);
                }
                if(movex == 0 && movez > 0)
                {
                    degreePer = (180f/30f);
                }
                if(movex > 0 && movez == 0)
                {
                    degreePer = -(90f/30f);
                }
            }
        }
        
        while(x < 30)
        {
            if(!((movex == 0) && (movez == 0)))
            {
                childTransform.Rotate(0f, -degreePer, 0f, Space.Self);
            }
            
            yield return new WaitForSeconds(.01f);
            x++;
        }
        StartCoroutine(Movedown());
    }
    IEnumerator Movedown()
    {
        int x = 0;
        while(x < 30)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - .05666f, this.transform.position.z);
            yield return new WaitForSeconds(.01f);
            x++;
        }
        StartCoroutine(fixMove());
    }
    //make sure that the piece is at an even integer position
    IEnumerator fixMove()
    {
        
        GameManager.gm.myTurn = GameManager.gm.lastTurn;
        yield return new WaitForSeconds(.1f);
        this.transform.position = new Vector3(Mathf.Round(this.transform.position.x), 0, Mathf.Round(this.transform.position.z));
        //Debug.Log("after fix: " + this.transform.position.x + " " + this.transform.position.z);
        
    }
    
    
}
