using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{
    private ulong currentClientID;

    public static PlayerInfo instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            return;
        }

        Destroy(gameObject);
    }

    public void SetClientId()
    {
        currentClientID = NetworkManager.Singleton.LocalClientId;
    }

    public ulong GetClientId()
    {
        return currentClientID;
    }


}
