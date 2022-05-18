using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PieceMovement : MonoBehaviour
{
    
    private int playerLayer;
    private int boardLayer;
    private GameObject lastHit;
    private GameObject lastMoved;
    private GameObject shipHit;
    private GameObject shipShoot;
    private int moveToX;
    private int moveToY;
    void Start()
    {
        playerLayer = LayerMask.GetMask("Pieces");
        boardLayer = LayerMask.GetMask("Board");
    }
    void Update()
    {
        //click down
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, playerLayer))
            {
                //if its blue turn and a blue piece is hit AND its either the first turn or (the hit piece isnt the last moved piece)
                if((GameManager.gm.myTurn == GameManager.turn.blue1 || GameManager.gm.myTurn == GameManager.turn.blue2) && hit.transform.name.Contains("b")&&
                    (GameManager.gm.myTurn == GameManager.turn.blue1||GameObject.Find(hit.transform.name) != lastMoved))
                {
                    
                    //Debug.Log(GameManager.gm.myTurn + "   " + hit.transform.name);
                    lastHit = GameObject.Find(hit.transform.name);
                    checkPossibleLocations();
                }
                //if its red turn and a red piece is hit AND its either the first turn or (the hit piece isnt the last moved piece)
                if((GameManager.gm.myTurn == GameManager.turn.red1 || GameManager.gm.myTurn == GameManager.turn.red2) && hit.transform.name.Contains("r")&&
                    (GameManager.gm.myTurn == GameManager.turn.red1||GameObject.Find(hit.transform.name) != lastMoved))
                {
                    lastHit = GameObject.Find(hit.transform.name);
                    checkPossibleLocations();
                }
                
            }
        }
        //release click
        if (Input.GetMouseButtonUp(0))
        {
            if(lastHit != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, boardLayer))
                {
                    string[] coords;
                    coords = hit.transform.name.Split(' ');
                    if(GameObject.Find(float.Parse(coords[0]) + " " + float.Parse(coords[1])).GetComponent<Renderer>().material.color == Color.green||
                            GameObject.Find(float.Parse(coords[0]) + " " + float.Parse(coords[1])).GetComponent<Renderer>().material.color == Color.black)
                    {
                        lastHit.GetComponent<PhotonView>().RPC("MovePiece", RpcTarget.AllBuffered, float.Parse(coords[0]), float.Parse(coords[1]));
                        //lastHit.GetComponent<Move>().MovePiece(float.Parse(coords[0]), float.Parse(coords[1]));
                        GameManager.gm.GetComponent<PhotonView>().RPC("NextTurn", RpcTarget.AllBuffered);
                        //this is what holds the last moved piece to make it so that you cant move the same piece twice in a turn
                        lastMoved = lastHit;
                        
                        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        //see if killing other piece
                        if (Physics.Raycast(ray, out hit, 100, playerLayer))
                        {
                            if(hit.transform.name.Contains("rShip") && lastHit.transform.name.Contains("bShip"))
                            {
                                destroyShip(GameObject.Find(hit.transform.name));
                            }
                            else if(hit.transform.name.Contains("bShip") && lastHit.transform.name.Contains("rShip"))
                            {
                                destroyShip(GameObject.Find(hit.transform.name));
                            }
                        }
                        
                    }
                    
                }
            }
            shipShoot = null;
            lastHit = null;
            foreach(Transform child in GameObject.Find("Board").transform)
            {
                child.gameObject.GetComponent<Renderer>().material.color = Color.white;
            }
        }
    }
    //checks what ship is hit and gets the pissible spaces for each piece
    void checkPossibleLocations()
    {
        
        if(lastHit.transform.name.Contains("1"))
        {
            ship1();
        }
        else if(lastHit.transform.name.Contains("2"))
        {
            ship2();
        }
        else if(lastHit.transform.name.Contains("3"))
        {
            ship3();
        }
        else if(lastHit.transform.name.Contains("4"))
        {
            ship4();
        }
        else if(lastHit.transform.name.Contains("5"))
        {
            ship5();
        }
        else if(lastHit.transform.name.Contains("6"))
        {
            ship6();
        }

        
    }
    //given a location, checks if there is anything in the way
    bool isValidSpot(Vector3 location)
    {
        //obstacles have to be on player layer
        Collider[] hitColliders = Physics.OverlapSphere(location, .44f, playerLayer);
        foreach (var hitCollider in hitColliders)
        {
            //see if moving and piece hit are on the same team
            if(hitCollider.transform.name.Contains("bShip"))
            {
                if(lastHit.transform.name.Contains("bShip"))
                {
                    return false;
                }
                else
                {
                    shipHit = hitCollider.gameObject;
                    shipShoot = hitCollider.gameObject;
                }
            }
            //same as above
            else if(hitCollider.transform.name.Contains("rShip"))
            {
                if(lastHit.transform.name.Contains("rShip"))
                {
                    return false;
                }
                else
                {
                    shipHit = hitCollider.gameObject;
                    shipShoot = hitCollider.gameObject;
                }
            }
            //see if there is an obstacle in the location trying to be moved to
            else if(hitCollider.transform.name.Contains("Obstacle"))
            {
                return false;
            }
        }
        return true;
    }
    void destroyShip(GameObject ship)
    {
        //this has to be this way and not the commented out way becuase if not, only the player that attacked would see the other piece die
        ship.GetComponent<PhotonView>().RPC("die", RpcTarget.AllBuffered);
        //ship.GetComponent<Destroy>().die();
    }

    


///////////////////////////////////////////////////////////////Ship Movement Checks//////////////////////////////////////////////////////////////////
//each ship calculates the locations that would be available to it and checks them for if the spots are valid. If they are they are either made green or black depending on if there is an enemy there
//anywhere within 2 rings of ship
void ship1()
    {
        float shipx = Mathf.Round(lastHit.transform.position.x);
        float shipz = Mathf.Round(lastHit.transform.position.z);
        for(float i = -2f; i <= 2f; i++)
        {
            for(float j = -2f; j<=2f; j++)
            {
                if(6<shipx+i||shipx+i<-6 || 6<shipz+j || shipz +j<-6)
                {
                    continue;
                }
                if(i==0&&j==0)
                {
                    continue;
                }
                if(isValidSpot(new Vector3((shipx+i)-2.099f, .1f, (shipz+j) -3.364f))){
                 if(shipHit != null){
                    GameObject.Find((shipx+i) + " " + (shipz + j)).GetComponent<Renderer>().material.color = Color.black;
                    shipHit = null;}
                else{GameObject.Find((shipx+i) + " " + (shipz + j)).GetComponent<Renderer>().material.color = Color.green;}}
            }
        }
    }



//straight lines
    void ship2()
    {
        float shipx = Mathf.Round(lastHit.transform.position.x);
        float shipz = Mathf.Round(lastHit.transform.position.z);
        for(float i = -1f; i >= -3f; i--)
            {
                if(6<shipz+i || shipz + i<-6)
                {
                    continue;
                }
                if(i==0)
                {
                    continue;
                }
                if(isValidSpot(new Vector3((shipx)-2.099f, .1f, (shipz+i) -3.364f))){
                    if(shipHit != null){
                        GameObject.Find((shipx) + " " + (shipz + i)).GetComponent<Renderer>().material.color = Color.black;
                        shipHit = null;}
                    else{
                        //Debug.Log((shipx) + " " + (shipz + i));
                        GameObject.Find((shipx) + " " + (shipz + i)).GetComponent<Renderer>().material.color = Color.green;}}
                        else{break;}
            }
        for(float i = 1f; i <= 3f; i++)
            {
                if(6<shipz+i || shipz + i<-6)
                {
                    continue;
                }
                if(i==0)
                {
                    continue;
                }
                if(isValidSpot(new Vector3((shipx)-2.099f, .1f, (shipz+i) -3.364f))){
                    if(shipHit != null){
                        GameObject.Find((shipx) + " " + (shipz + i)).GetComponent<Renderer>().material.color = Color.black;
                        shipHit = null;}
                    else{
                        //Debug.Log((shipx) + " " + (shipz + i));
                        GameObject.Find((shipx) + " " + (shipz + i)).GetComponent<Renderer>().material.color = Color.green;}}
                        else{break;}
            }


            for(float i = -1f; i >= -3f; i--)
            {
                if(6<shipx+i || shipx + i<-6)
                {
                    continue;
                }
                if(i==0)
                {
                    continue;
                }
                if(isValidSpot(new Vector3((shipx+i)-2.099f, .1f, (shipz) -3.364f))){
                    if(shipHit != null){
                        GameObject.Find((shipx+i) + " " + (shipz)).GetComponent<Renderer>().material.color = Color.black;
                        shipHit = null;}
                    else{GameObject.Find((shipx+i) + " " + (shipz)).GetComponent<Renderer>().material.color = Color.green;}}
                    else{break;}
            }
            for(float i = 1f; i <= 3f; i++)
            {
                if(6<shipx+i || shipx + i<-6)
                {
                    continue;
                }
                if(i==0)
                {
                    continue;
                }
                if(isValidSpot(new Vector3((shipx+i)-2.099f, .1f, (shipz) -3.364f))){
                    if(shipHit != null){
                        GameObject.Find((shipx+i) + " " + (shipz)).GetComponent<Renderer>().material.color = Color.black;
                        shipHit = null;}
                    else{GameObject.Find((shipx+i) + " " + (shipz)).GetComponent<Renderer>().material.color = Color.green;}}
                    else{break;}
            }
    }
    //1 ring 2 rings away from ship
    void ship3()
    {
        //top and bottom
        float shipx = Mathf.Round(lastHit.transform.position.x);
        float shipz = Mathf.Round(lastHit.transform.position.z);
        for(float i = -2f; i <= 2f; i++)
        {
            if(6<shipx+i||shipx+i<-6 || 6<shipz+2 || shipz +2<-6)
            {
                continue;
            }
            if(isValidSpot(new Vector3((shipx+i)-2.099f, .1f, (shipz+2f) -3.364f))){
                 if(shipHit != null){
                    GameObject.Find((shipx+i) + " " + (shipz + 2f)).GetComponent<Renderer>().material.color = Color.black;
                    shipHit = null;}
                else{
                    GameObject.Find((shipx+i) + " " + (shipz + 2f)).GetComponent<Renderer>().material.color = Color.green;}}
        }
        for(float i = -2f; i <= 2f; i++)
        {
        if(6<shipx+i||shipx+i<-6 || 6<shipz-2 || shipz -2<-6)
        {
            continue;
        }
        if(isValidSpot(new Vector3((shipx+i)-2.099f, .1f, (shipz-2f) -3.364f))){
            if(shipHit != null){
                GameObject.Find((shipx+i) + " " + (shipz-2f)).GetComponent<Renderer>().material.color = Color.black;
                shipHit = null;}
            else{GameObject.Find((shipx+i) + " " + (shipz-2f)).GetComponent<Renderer>().material.color = Color.green;}}
        }
        //sides
        for(float i = -1f; i <= 1f; i++)
        {
        if(6<shipx-2||shipx-2<-6 || 6<shipz+i || shipz+i<-6)
        {
            continue;
        }
        if(isValidSpot(new Vector3((shipx-2f)-2.099f, .1f, (shipz+i) -3.364f))){
            if(shipHit != null){
                GameObject.Find((shipx-2f) + " " + (shipz+i)).GetComponent<Renderer>().material.color = Color.black;
                shipHit = null;}
            else{GameObject.Find((shipx-2f) + " " + (shipz+i)).GetComponent<Renderer>().material.color = Color.green;}}
        }
        for(float i = -1f; i <= 1f; i++)
        {
        if(6<shipx+2||shipx+2<-6 || 6<shipz+i || shipz+i<-6)
        {
            continue;
        }
        if(isValidSpot(new Vector3((shipx+2f)-2.099f, .1f, (shipz+i) -3.364f))){
            if(shipHit != null){
                GameObject.Find((shipx+2f) + " " + (shipz+i)).GetComponent<Renderer>().material.color = Color.black;
                shipHit = null;}
            else{GameObject.Find((shipx+2f) + " " + (shipz+i)).GetComponent<Renderer>().material.color = Color.green;}}
        }
        
    }


    //diagonal lines
    void ship4()
    {
        float shipx = Mathf.Round(lastHit.transform.position.x);
        float shipz = Mathf.Round(lastHit.transform.position.z);
        for(float i = -1f; i >= -3f; i--)
        {
            if(6<shipx+i||shipx+i<-6 || 6<shipz+i || shipz +i<-6)
            {
                continue;
            }
            if(i==0)
            {
                continue;
            }
            if(isValidSpot(new Vector3((shipx+i)-2.099f, .1f, (shipz+i) -3.364f))){
                if(shipHit != null){
                    GameObject.Find((shipx+i) + " " + (shipz+i)).GetComponent<Renderer>().material.color = Color.black;
                    shipHit = null;}
                else{
                    GameObject.Find((shipx+i) + " " + (shipz+i)).GetComponent<Renderer>().material.color = Color.green;}}
            else{break;}
        }
        for(float i = 1f; i <= 3f; i++)
        {
            if(6<shipx+i||shipx+i<-6 || 6<shipz+i || shipz +i<-6)
            {
                continue;
            }
            if(i==0)
            {
                continue;
            }
            if(isValidSpot(new Vector3((shipx+i)-2.099f, .1f, (shipz+i) -3.364f))){
                if(shipHit != null){
                    GameObject.Find((shipx+i) + " " + (shipz+i)).GetComponent<Renderer>().material.color = Color.black;
                    shipHit = null;}
                else{
                    GameObject.Find((shipx+i) + " " + (shipz+i)).GetComponent<Renderer>().material.color = Color.green;}}
            else{break;}
        }
        for(float i = -1f; i >= -3f; i--)
        {
            if(6<shipx+i||shipx+i<-6 || 6<shipz-i || shipz -i<-6)
            {
                continue;
            }
            if(i==0)
            {
                continue;
            }
            if(isValidSpot(new Vector3((shipx+i)-2.099f, .1f, (shipz-i) -3.364f))){
            if(shipHit != null){
                GameObject.Find((shipx+i) + " " + (shipz-i)).GetComponent<Renderer>().material.color = Color.black;
                shipHit = null;}
            else{GameObject.Find((shipx+i) + " " + (shipz-i)).GetComponent<Renderer>().material.color = Color.green;}}
            else{break;}
        }
        for(float i = 1f; i <= 3f; i++)
        {
            if(6<shipx+i||shipx+i<-6 || 6<shipz-i || shipz -i<-6)
            {
                continue;
            }
            if(i==0)
            {
                continue;
            }
            if(isValidSpot(new Vector3((shipx+i)-2.099f, .1f, (shipz-i) -3.364f))){
            if(shipHit != null){
                GameObject.Find((shipx+i) + " " + (shipz-i)).GetComponent<Renderer>().material.color = Color.black;
                shipHit = null;}
            else{GameObject.Find((shipx+i) + " " + (shipz-i)).GetComponent<Renderer>().material.color = Color.green;}}
            else{break;}
        }
    }
    //can move 4 in z and 1 in x directions
    void ship5()
    {
        float shipx = Mathf.Round(lastHit.transform.position.x);
        float shipz = Mathf.Round(lastHit.transform.position.z);
        for(float i = 1f; i <= 4f; i++)
            {
                if(6<shipz+i || shipz + i<-6)
                {
                    continue;
                }
                if(i==0)
                {
                    continue;
                }
                if(isValidSpot(new Vector3((shipx)-2.099f, .1f, (shipz+i) -3.364f))){
                    if(shipHit != null){
                        GameObject.Find((shipx) + " " + (shipz + i)).GetComponent<Renderer>().material.color = Color.black;
                        shipHit = null;}
                    else{GameObject.Find((shipx) + " " + (shipz + i)).GetComponent<Renderer>().material.color = Color.green;}}
                    else{break;}
            }
        for(float i = -1f; i >= -4f; i--)
            {
                if(6<shipz+i || shipz + i<-6)
                {
                    continue;
                }
                if(i==0)
                {
                    continue;
                }
                if(isValidSpot(new Vector3((shipx)-2.099f, .1f, (shipz+i) -3.364f))){
                    if(shipHit != null){
                        GameObject.Find((shipx) + " " + (shipz + i)).GetComponent<Renderer>().material.color = Color.black;
                        shipHit = null;}
                    else{GameObject.Find((shipx) + " " + (shipz + i)).GetComponent<Renderer>().material.color = Color.green;}}
                    else{break;}
            }
        if(!(shipx+1>6))
            {
                if(isValidSpot(new Vector3((shipx+1)-2.099f, .1f, (shipz) -3.364f))){
                    if(shipHit != null){
                        GameObject.Find((shipx+1) + " " + (shipz)).GetComponent<Renderer>().material.color = Color.black;
                        shipHit = null;}
                    else{GameObject.Find((shipx+1) + " " + (shipz)).GetComponent<Renderer>().material.color = Color.green;}}
            }
        if(!(shipx-1<-6))
            {
                if(isValidSpot(new Vector3((shipx+1)-2.099f, .1f, (shipz) -3.364f))){
                    if(shipHit != null){
                        GameObject.Find((shipx-1) + " " + (shipz)).GetComponent<Renderer>().material.color = Color.black;
                        shipHit = null;}
                    else{GameObject.Find((shipx-1) + " " + (shipz)).GetComponent<Renderer>().material.color = Color.green;}}
            }
    }


//can go adjacent to any ally
    void ship6()
    {
        float shipx = Mathf.Round(lastHit.transform.position.x);
        float shipz = Mathf.Round(lastHit.transform.position.z);
        GameObject[] team;
        if(lastHit.tag == "RedTeam")
        {
            team = GameObject.FindGameObjectsWithTag("RedTeam");
            foreach (GameObject ally in team)
            {
                
                if(!(ally.transform.position.x-1<-6))
                {
                    
                    if(isValidSpot(new Vector3(Mathf.Round((ally.transform.position.x-1))-2.099f, .1f, Mathf.Round((ally.transform.position.z)) -3.364f))){
                        if(shipHit != null){
                            GameObject.Find(Mathf.Round((ally.transform.position.x-1)) + " " + Mathf.Round((ally.transform.position.z))).GetComponent<Renderer>().material.color = Color.black;
                            shipHit = null;}
                        else{GameObject.Find(Mathf.Round((ally.transform.position.x-1)) + " " + Mathf.Round((ally.transform.position.z))).GetComponent<Renderer>().material.color = Color.green;}}
                }
                if(!(ally.transform.position.x+1>6))
                {
                    
                    if(isValidSpot(new Vector3(Mathf.Round((ally.transform.position.x+1))-2.099f, .1f, Mathf.Round((ally.transform.position.z)) -3.364f))){
                        if(shipHit != null){
                            GameObject.Find(Mathf.Round((ally.transform.position.x+1)) + " " + Mathf.Round((ally.transform.position.z))).GetComponent<Renderer>().material.color = Color.black;
                            shipHit = null;}
                        else{GameObject.Find(Mathf.Round((ally.transform.position.x+1)) + " " + Mathf.Round((ally.transform.position.z))).GetComponent<Renderer>().material.color = Color.green;}}
                }
                if(!(ally.transform.position.z-1<-6))
                {
                    
                    if(isValidSpot(new Vector3(Mathf.Round((ally.transform.position.x))-2.099f, .1f, Mathf.Round((ally.transform.position.z-1)) -3.364f))){
                        if(shipHit != null){
                            GameObject.Find(Mathf.Round((ally.transform.position.x)) + " " + Mathf.Round((ally.transform.position.z-1))).GetComponent<Renderer>().material.color = Color.black;
                            shipHit = null;}
                        else{GameObject.Find(Mathf.Round((ally.transform.position.x)) + " " + Mathf.Round((ally.transform.position.z-1))).GetComponent<Renderer>().material.color = Color.green;}}
                }
                if(!(ally.transform.position.z+1>6))
                {
                    
                    if(isValidSpot(new Vector3(Mathf.Round((ally.transform.position.x))-2.099f, .1f, Mathf.Round((ally.transform.position.z+1)) -3.364f))){
                        if(shipHit != null){
                            GameObject.Find(Mathf.Round((ally.transform.position.x)) + " " + Mathf.Round((ally.transform.position.z+1))).GetComponent<Renderer>().material.color = Color.black;
                            shipHit = null;}
                        else{GameObject.Find(Mathf.Round((ally.transform.position.x)) + " " + Mathf.Round((ally.transform.position.z+1))).GetComponent<Renderer>().material.color = Color.green;}}
                }
            }
        }
        else if(lastHit.tag == "BlueTeam")
        {
            team = GameObject.FindGameObjectsWithTag("BlueTeam");
            foreach (GameObject ally in team)
            {
                if(!(ally.transform.position.x-1<-6))
                {
                    
                    if(isValidSpot(new Vector3(Mathf.Round((ally.transform.position.x-1))-2.099f, .1f, Mathf.Round((ally.transform.position.z)) -3.364f))){
                        if(shipHit != null){
                            GameObject.Find(Mathf.Round((ally.transform.position.x-1)) + " " + Mathf.Round((ally.transform.position.z))).GetComponent<Renderer>().material.color = Color.black;
                            shipHit = null;}
                        else{GameObject.Find(Mathf.Round((ally.transform.position.x-1)) + " " + Mathf.Round((ally.transform.position.z))).GetComponent<Renderer>().material.color = Color.green;}}
                }
                if(!(ally.transform.position.x+1>6))
                {
                    
                    if(isValidSpot(new Vector3(Mathf.Round((ally.transform.position.x+1))-2.099f, .1f, Mathf.Round((ally.transform.position.z)) -3.364f))){
                        if(shipHit != null){
                            GameObject.Find(Mathf.Round((ally.transform.position.x+1)) + " " + Mathf.Round((ally.transform.position.z))).GetComponent<Renderer>().material.color = Color.black;
                            shipHit = null;}
                        else{GameObject.Find(Mathf.Round((ally.transform.position.x+1)) + " " + Mathf.Round((ally.transform.position.z))).GetComponent<Renderer>().material.color = Color.green;}}
                }
                if(!(ally.transform.position.z-1<-6))
                {
                    if(isValidSpot(new Vector3(Mathf.Round((ally.transform.position.x))-2.099f, .1f, Mathf.Round((ally.transform.position.z-1)) -3.365f))){
                        if(shipHit != null){
                            GameObject.Find(Mathf.Round((ally.transform.position.x)) + " " + Mathf.Round((ally.transform.position.z-1))).GetComponent<Renderer>().material.color = Color.black;
                            shipHit = null;}
                        else{GameObject.Find(Mathf.Round((ally.transform.position.x)) + " " + Mathf.Round((ally.transform.position.z-1))).GetComponent<Renderer>().material.color = Color.green;}}
                }
                if(!(ally.transform.position.z+1>6))
                {
                    if(isValidSpot(new Vector3(Mathf.Round((ally.transform.position.x))-2.099f, .1f, Mathf.Round((ally.transform.position.z+1)) -3.364f))){
                        if(shipHit != null){
                            GameObject.Find(Mathf.Round((ally.transform.position.x)) + " " + Mathf.Round((ally.transform.position.z+1))).GetComponent<Renderer>().material.color = Color.black;
                            shipHit = null;}
                        else{GameObject.Find(Mathf.Round((ally.transform.position.x)) + " " + Mathf.Round((ally.transform.position.z+1))).GetComponent<Renderer>().material.color = Color.green;}}
                }
            }
        }
        
    }
}
