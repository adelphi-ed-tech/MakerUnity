using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowTrigger : MonoBehaviour
{
    public Room room;
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            foreach (Transform t in room.Ceiling.transform)
            {
                Light light = t.GetComponent<Light>();
                if (light != null)
                {
                    light.shadows = room.mood.castShadows ? LightShadows.Hard : LightShadows.None;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach (Transform t in room.Ceiling.transform)
            {
                Light light = t.GetComponent<Light>();
                if (light != null)
                {
                    light.shadows = LightShadows.None;
                }
            }
        }
    }
}
