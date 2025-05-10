using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Oculus.Platform.Models;
using UnityEngine;
using Unity.Netcode;

public class pieceSnapping : MonoBehaviour
{
    public GameObject piece, anchor;
    private Vector3 alignedForward, alignedUp;
    private GameObject[] solvedPieces;

    void Start(){
        solvedPieces = GameObject.FindGameObjectsWithTag("Piece");
        anchor = GameObject.Find("Anchor");
    }

    private static Vector3 NearestWorldAxis(Vector3 v){
        if (Mathf.Abs(v.x) < Mathf.Abs(v.y)){
            v.x = 0;
            if (Mathf.Abs(v.y) < Mathf.Abs(v.z)) v.y = 0;
            else v.z = 0;
        }
        else{
            v.y = 0;
            if (Mathf.Abs(v.x) < Mathf.Abs(v.z)) v.x = 0;
            else v.z = 0;
        }
        return v;
    }
    
    public void onDrop(){
        alignedForward = NearestWorldAxis(piece.transform.forward);
        alignedUp = NearestWorldAxis(piece.transform.up);
        piece.transform.rotation = Quaternion.LookRotation(alignedForward, alignedUp);
        float piecex = piece.transform.position.x, piecey = piece.transform.position.y, piecez = piece.transform.position.z;
        float anchorx = anchor.transform.position.x, anchory = anchor.transform.position.y, anchorz = anchor.transform.position.z;
        bool clash = false;

        if(piecex>anchorx) piecex = anchorx;
        else if (piecex<anchorx-0.8f)piecex = anchorx-0.8f;
        else piecex = anchorx+Mathf.Round((piecex-anchorx)*5)/5;
        
        if(piecey<anchory) piecey = anchory;
        else if(piecey>anchory+0.6f) piecey = anchory+0.6f;
        else piecey = anchory+Mathf.Round((piecey-anchory)*5)/5;

        if(piecez<anchorz) piecez = anchorz;
        else if (piecez>anchorz+0.8f)piecez = anchorz+0.8f;
        else piecez = anchorz+Mathf.Round((piecez-anchorz)*5)/5;
        
        Vector3 finalPos = new Vector3(piecex, piecey, piecez);
        
        //makes sure position does not already have a piece
        foreach(GameObject g in solvedPieces){
            if (g.transform.position==finalPos && g!=piece){
                clash = true;
            }
        }
        if(clash==false) piece.transform.position = new Vector3(piecex, piecey, piecez);   
    }
}
