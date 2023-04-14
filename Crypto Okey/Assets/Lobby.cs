using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class Lobby
{

    private List<ulong> players = new List<ulong>();

    public int maxPlayers {get; private set;} = 4;

    private ulong loobyId;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetCurrentPlayerNumber()
    {
        return players.Count;
    }

    [ServerRpc]
    public void ConnectToLobyServerRpc(ulong clientID)
    {
        players.Add(clientID);
    }

    [ServerRpc]
    public void LeaveLobbyServerRpc(ulong clientID)
    {
        players.Remove(clientID);
    }


    [ServerRpc]
    public void SetLobbyIdServerRpc(ulong clientId)
    {
        loobyId = clientId;
    }

    [ServerRpc]
    public void AllPlayersId()
    {
        for (int i = 0; i < players.Count; i++)
        {
            Debug.Log("Oyuncu Id " + players[i]);
        }
    }
}
