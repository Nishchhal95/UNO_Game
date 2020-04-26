using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class _GameManager : MonoBehaviour
{
    public static _GameManager Instance = null;

    public Transform myPlayerHolder;
    public Transform[] otherPlayerHolders;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(this);
        }
    }
}
