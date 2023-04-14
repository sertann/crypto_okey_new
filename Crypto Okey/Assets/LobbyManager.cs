using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    private List<Lobby> lobbies = new List<Lobby>();
    [SerializeField] Button playBtn;
    ServerRpcParams serverRpcParam;

    public override void OnNetworkSpawn()
    {
        serverRpcParam = new ServerRpcParams();
    }

    [ServerRpc(RequireOwnership = false)]
    void CreateLobbyServerRpc(ulong clientId)
    {
        Lobby lobby = new Lobby();
        lobby.ConnectToLobyServerRpc(clientId);
        lobby.SetLobbyIdServerRpc(clientId);
        lobbies.Add(lobby);
        Debug.Log("Lobby Created");
    }

    private void Update()
    {
        if (!IsServer) return;
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("lobby sayisi = " + lobbies.Count.ToString());

            int playerCount = 0;
            for (int i = 0; i < lobbies.Count; i++)
            {
                Debug.Log(i + ". lobi oyuncularý");
                lobbies[i].AllPlayersId();
                playerCount += lobbies[i].GetCurrentPlayerNumber();
            }
            Debug.Log("oyuncu sayisi = " + playerCount.ToString());

            
        }
        
    }

    public void SearchForLobbies()
    {
        SearchForLobbiesServerRpc(serverRpcParam);
    }


    [ServerRpc(RequireOwnership = false)]
    void SearchForLobbiesServerRpc(ServerRpcParams serverRpc)
    {
        if (!IsServer) return;
        ulong sender = serverRpc.Receive.SenderClientId;
        Debug.Log("Searching");
        if (lobbies.Count <= 0)
        {
            CreateLobbyServerRpc(sender);
            return;
        }

        for (int i = 0; i < lobbies.Count; i++)
        {
            if (lobbies[i].GetCurrentPlayerNumber() < lobbies[i].maxPlayers) 
            {
                lobbies[i].ConnectToLobyServerRpc(sender);
                Debug.Log(sender + "joined the session");
                return;
            }
        }

        CreateLobbyServerRpc(sender);
        
    }
}
