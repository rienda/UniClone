using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RemotePlayerOff : NetworkBehaviour {

	[SerializeField]
	private Behaviour[] components;

	[SerializeField]
	private GameObject[] gameObjects;

	void Start()
	{
		if (!isLocalPlayer)
		{
			for (int i = 0; i < components.Length; i++)
			{
				components[i].enabled = false;
			}
			for (int i = 0; i < gameObjects.Length; i++)
			{
				gameObjects[i].SetActive(false);
			}
		}
	}
}
