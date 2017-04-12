using UnityEngine;
using System.Collections;
using Photon;

public class GameManager : PunBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void OnJoinedRoom()
    {
        GameObject spawnedPlayer = PhotonNetwork.Instantiate("Player 1", new Vector3(-151, 1.5f, -25), Quaternion.identity, 0);
    }
}
