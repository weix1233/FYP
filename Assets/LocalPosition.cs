using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPosition : MonoBehaviour
{
    public GameObject anchor;
    public int localx, localy, localz;
    private float piecex, piecey, piecez, anchorx, anchory, anchorz;
    // Start is called before the first frame update
    void Start()
    {
        if(this.tag=="Solution") anchor = GameObject.Find("SolutionAnchor");
        else anchor = GameObject.Find("Anchor");
        anchorx = anchor.transform.position.x;
        anchory = anchor.transform.position.y;
        anchorz = anchor.transform.position.z;
        calcPos();
    }

    public void calcPos(){
        piecex = this.transform.position.x;
        piecey = this.transform.position.y;
        piecez = this.transform.position.z;
        localx = (int)Mathf.Round((anchorx-piecex)*5);
        localy = (int)Mathf.Round((piecey-anchory)*5);
        localz = (int)Mathf.Round((piecez-anchorz)*5);
    }
}
