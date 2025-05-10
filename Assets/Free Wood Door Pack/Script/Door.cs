using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
namespace DoorScript
{
	[RequireComponent(typeof(AudioSource))]


	public class Door : NetworkBehaviour {
		public AudioSource asource;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
		public AudioClip openDoor;
		public NetworkObject doorActive;
		public Transform teleportTarget;
		public GameObject player;
		UnityEvent doorInteracted = new UnityEvent();
		// Use this for initialization
		void Start () {
			asource = GetComponent<AudioSource> ();
			doorInteracted.AddListener(destroyDoor);
		}

		void OnTriggerEnter(Collider other){
			if(other.tag == "GameController")
			{
				asource.clip = openDoor;
				asource.Play ();
				DoorDestroyerServerRpc();
				DoorDestroyerNotServerRpc();
				player.transform.position = teleportTarget.transform.position;
			}
		}
		
		public void destroyDoor(){
			Destroy(doorActive.gameObject);
		}

		[Rpc(SendTo.Server)]
		void DoorDestroyerServerRpc(){
			doorInteracted.Invoke();
		}
		[Rpc(SendTo.NotServer)]
		void DoorDestroyerNotServerRpc(){
			doorInteracted.Invoke();
		}
	}
}