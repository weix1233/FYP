using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using TMPro;
using Unity.VisualScripting.FullSerializer;

public class SolutionChecker : NetworkBehaviour{
    private GameObject[] solutionPieces;
    private GameObject[] solvedPieces;
    private GameObject[] winMessages;
    private GameObject[] pedestals;
    private GameObject[] prefabs;
    public GameObject[] tutMessage;
    
    public NetworkVariable<int> numberPieces = new NetworkVariable<int>();
    public NetworkVariable<int> correctPieces = new NetworkVariable<int>();

    void Start(){
        winMessages = GameObject.FindGameObjectsWithTag("WinMessage");
        pedestals = GameObject.FindGameObjectsWithTag("Pedestal");
        prefabs = GameObject.FindGameObjectsWithTag("Prefab");
        solvedPieces = GameObject.FindGameObjectsWithTag("Piece");
        solutionPieces = GameObject.FindGameObjectsWithTag("Solution");
        numberPieces.Value = solvedPieces.Length;
        correctPieces.Value = 0;
    }

    public override void OnNetworkSpawn(){
        if (IsServer) { // Ensure only the server listens for value changes
        correctPieces.OnValueChanged += (prev, curr) =>
        {  
            if (correctPieces.Value == numberPieces.Value){
                Debug.Log("Server: Sending winMessageClientRpc()");
                winMessageClientRpc();
                winMessage(); // The host should call this directly
            }
        };
    }
    }
    public void checkSolution(){
        solvedPieces = GameObject.FindGameObjectsWithTag("Piece");
        solutionPieces = GameObject.FindGameObjectsWithTag("Solution");
        numberPieces.Value = solvedPieces.Length;
        Debug.Log(solutionPieces.Length);
        correctPieces.Value = 0;
        foreach(GameObject g in solvedPieces){
            foreach(GameObject h in solutionPieces){
                if(g.GetComponent<LocalPosition>().localx==h.GetComponent<LocalPosition>().localx && 
                g.GetComponent<LocalPosition>().localy==h.GetComponent<LocalPosition>().localy &&
                g.GetComponent<LocalPosition>().localz==h.GetComponent<LocalPosition>().localz &&
                g.GetComponent<Renderer>().sharedMaterial==h.GetComponent<Renderer>().sharedMaterial &&
                g.transform.rotation == h.transform.rotation){
                    correctPieces.Value++;
                    break;
                }
            }
        }
    }
    void winMessage(){
        Debug.Log($"Client {NetworkManager.Singleton.LocalClientId}: Running winMessage()");
        prefabs = GameObject.FindGameObjectsWithTag("Prefab");
        solvedPieces = GameObject.FindGameObjectsWithTag("Piece");
        solutionPieces = GameObject.FindGameObjectsWithTag("Solution");
        winMessages = GameObject.FindGameObjectsWithTag("WinMessage");
        pedestals = GameObject.FindGameObjectsWithTag("Pedestal");
		foreach(GameObject message in winMessages){
            message.GetComponent<TextMeshProUGUI>().enabled=true;
        }
        foreach(GameObject g in pedestals){
            g.GetComponent<Renderer>().enabled=true;
        }
        foreach(GameObject piece in solvedPieces){
            Destroy(piece);
        }
        foreach(GameObject solutionPiece in solutionPieces){
            Destroy(solutionPiece);
        }
        foreach(GameObject prefab in prefabs){
            Destroy(prefab);
        }
        foreach(GameObject message in tutMessage){
            message.GetComponent<TextMeshProUGUI>().enabled=false;
        }
	}

	[ClientRpc]
    void winMessageClientRpc(){
        Debug.Log($"Client {NetworkManager.Singleton.LocalClientId}: Received winMessageClientRpc()");
        winMessages = GameObject.FindGameObjectsWithTag("WinMessage");
        pedestals = GameObject.FindGameObjectsWithTag("Pedestal");
        foreach(GameObject message in winMessages){
            message.GetComponent<TextMeshProUGUI>().enabled=true;
        }
        foreach(GameObject g in pedestals){
            g.GetComponent<Renderer>().enabled=true;
        }
        if(!IsHost){
            winMessage();
        }
	}
}
