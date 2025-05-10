using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using TMPro;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using System.Net.NetworkInformation;
using System.Linq;
using System;
using Unity.VisualScripting;

public class nextLevel : NetworkBehaviour
{
    private GameObject[] pedestals;
    private GameObject[] winMessages;
    private GameObject[] endMessages;
    private GameObject solver;
    private GameObject assistant;
    public GameObject[] levels;
    public GameObject[] levelPieces;
    public NetworkVariable<int> randomIndex = new NetworkVariable<int>(-1);

    void Start(){
        winMessages = GameObject.FindGameObjectsWithTag("WinMessage");
        endMessages = GameObject.FindGameObjectsWithTag("EndMessage");
        pedestals = GameObject.FindGameObjectsWithTag("Pedestal");
        solver = GameObject.Find("Solver Room");
        assistant = GameObject.Find("Solution Space");
    }

    public override void OnNetworkSpawn(){
        randomIndex.OnValueChanged += (prev, curr) =>
        {
            removeUIClientRpc();
            if (IsHost){
                SpawnNextLevelServerRpc(curr);
            }
        };
    }

    public void PlayerButtonPressed(){
        if (IsHost){
            /*
            int count = levels.Length; 
            if( count == 0) {
            // Handle somehow this event
                endGameClientRpc();
            }
            else{
                randomIndex.Value = UnityEngine.Random.Range(0, count - 1); // choose random element
            }*/

            //for testing
            if(randomIndex.Value==19){
                endGameClientRpc();
            }
            else{
                randomIndex.Value++;
            }
        }
    }

    [ClientRpc]
    void endGameClientRpc(){
        foreach (GameObject g in endMessages){
            g.GetComponent<TextMeshProUGUI>().enabled = true;
        }
    }

    [ClientRpc]
    void removeUIClientRpc(){
        foreach (GameObject g in pedestals){
            g.GetComponent<Renderer>().enabled = false;
        }
        foreach (GameObject message in winMessages){
            message.GetComponent<TextMeshProUGUI>().enabled = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnNextLevelServerRpc(int index){
        GameObject prefabLevel = levels[index];
        GameObject prefabLevelPieces = levelPieces[index];
        
        // Instantiate new level and pieces
        GameObject level = Instantiate(prefabLevel, assistant.transform);
        GameObject piecesObject = Instantiate(prefabLevelPieces, solver.transform);

        // Ensure they have NetworkObject components
        NetworkObject levelNetworkObject = level.GetComponent<NetworkObject>();
        NetworkObject piecesNetworkObject = piecesObject.GetComponent<NetworkObject>();
        levelNetworkObject.Spawn();
        int counter = 0;
        
        if (piecesNetworkObject != null){
            foreach (Transform childTransform in piecesObject.GetComponentsInChildren<Transform>()){
                // Instantiate and spawn the child object
                GameObject childGameObj = childTransform.gameObject;
                NetworkObject childNetworkObject = childGameObj.GetComponent<NetworkObject>();
                if (childNetworkObject != null && !childNetworkObject.IsSpawned) {
                    childNetworkObject.Spawn();
                    Debug.LogFormat("Spawned {0}", counter);
                    counter++;
                }
            }
        }
    }
}
