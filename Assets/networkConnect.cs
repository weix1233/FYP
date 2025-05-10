using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class networkConnect : MonoBehaviour
{
    public int maxConnection = 2;
    public UnityTransport transport;
    public GameObject wall;
    public GameObject panel;
    private Lobby currentLobby;

    private async void Awake()
    {
        if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
        {
            await UnityServices.InitializeAsync();
        }
        string debugProfile = "TestUser_" + UnityEngine.Random.Range(1000, 9999);
        AuthenticationService.Instance.SwitchProfile(debugProfile);
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
    }
    void Start()
    {
        ListLobbies();
    }
    public async void ListLobbies()
    {
        try{
            if (!AuthenticationService.Instance.IsSignedIn){
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Signed in with PlayerID: " + AuthenticationService.Instance.PlayerId);
            }
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            Debug.Log($"Lobbies found: {queryResponse.Results.Count}");
            foreach (var lobby in queryResponse.Results){
                Debug.Log($"Lobby Name: {lobby.Name}, ID: {lobby.Id}, Players: {lobby.Players.Count}");
            }
        }
        catch (LobbyServiceException e){
            Debug.LogError($"Failed to list lobbies: {e.Message}");
        }
    }
    public async void Create(){
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
        string newJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);
        CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
        {
            IsPrivate = false,  // Ensure the lobby is public
            Data = new Dictionary<string, DataObject>{
                { "JOIN_CODE", new DataObject(DataObject.VisibilityOptions.Public, newJoinCode) }
            }
        };
        currentLobby = await Lobbies.Instance.CreateLobbyAsync("Lobby Name", maxConnection,lobbyOptions);
        NetworkManager.Singleton.StartHost();
        panel.SetActive(false);
        Debug.Log("Lobby Created! ID: " + currentLobby.Id);
    }

    public async void Join(){
        QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
        if(queryResponse.Results.Count==0){
            Debug.Log("No lobbies to join.");
        }
        else{
            currentLobby = await Lobbies.Instance.QuickJoinLobbyAsync();
            string relayJoinCode = currentLobby.Data["JOIN_CODE"].Value;
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);
            NetworkManager.Singleton.StartClient();
            panel.SetActive(false);
            Destroy(wall);
            this.enabled=false;
        }
    }
    
    void Update(){
        try{
            if(NetworkManager.Singleton.ConnectedClients.Count>=2){
                Destroy(wall);
                this.enabled=false;
            }
        }
        catch(NotServerException){
        }
    }
}
