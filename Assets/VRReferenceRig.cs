using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRReferenceRig : MonoBehaviour
{
    public static VRReferenceRig Singleton;
    public Transform root;
    public Transform leftHand;
    public Transform rightHand;
    // Start is called before the first frame update
    private void Awake(){
        Singleton = this;
    }
}
