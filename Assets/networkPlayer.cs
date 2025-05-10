using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class networkPlayer : NetworkBehaviour
{
    public Transform root;
    public Transform leftHand;
    public Transform rightHand;

    public Renderer[] meshToDisable;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(IsOwner)
        {
            foreach (var item in meshToDisable)
            {
                item.enabled=false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsOwner)
        {
            root.position = VRReferenceRig.Singleton.root.position;
            root.rotation = VRReferenceRig.Singleton.root.rotation;
            leftHand.position = VRReferenceRig.Singleton.leftHand.position;
            leftHand.rotation = VRReferenceRig.Singleton.leftHand.rotation;
            rightHand.position = VRReferenceRig.Singleton.rightHand.position;
            rightHand.rotation = VRReferenceRig.Singleton.rightHand.rotation;
        }
    }

}
