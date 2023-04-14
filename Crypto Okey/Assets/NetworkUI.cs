using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUI : NetworkBehaviour
{
    [SerializeField] Button clientBtn;
    [SerializeField] Button serverBtn;
    [SerializeField] Button hostBtn;

    private void Awake()
    {
        serverBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
            Debug.Log("Startad as server");
        });
        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            SendMassageServerRpc();
        });
        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });
    }


    [ServerRpc]
    void SendMassageServerRpc()
    {
        Debug.Log("A Client Has Joined");
        Debug.Log(NetworkManager.Singleton.ConnectedClientsIds);
    }
}
