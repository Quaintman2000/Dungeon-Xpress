using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public void Shoot()
    {
        //when used this will shoot the spikes up if 0 is higher than its current position, meaning if they are down/reloaded.
        if (this.transform.localPosition.y < 0f)
        {
            StartCoroutine(_Shoot());
        }
    }

    IEnumerator _Shoot()
    {

        //waits .1 second after the trigger has happened
        yield return new WaitForSeconds(.1f);
        //shoots the spike up 2
        this.transform.localPosition += (Vector3.up * 2f);
    }
    public void Retract()
    {
        //if the spikes postion is more than or equal to zero this means they are up and out and must be retracted.
        if (this.transform.localPosition.y >= 0f)
        {
            //retracts the spikes
            StartCoroutine(_Retract());
        }
    }

    IEnumerator _Retract()
    {
        //waits .1 seconds after triggered

        yield return new WaitForSeconds(.1f);
        // brings the spikes down 2 back into reloaded postion.
        this.transform.localPosition -= (Vector3.up * 2f);
    }
}
