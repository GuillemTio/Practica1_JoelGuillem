using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTargetProve : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "HumanTarget")
        {
            print("ENTER");
        }
    }

    void OnTriggerExit (Collider other)
    {
        if (other.gameObject.tag == "HumanTarget")
        {
            print("EXIT");
        }
    }
}
