using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSpikes : MonoBehaviour
{
    //Makes a list to detect all game items using the combat controller script
    private List<CombatController> listControllers = new List<CombatController>();

    private List<Spike> listSpikes = new List<Spike>();

    // references a coroutine to trigger the spikes to rise
    Coroutine SpikeTriggerRoutine;
    // refernces a bool to check if spikes a reloaded/down.
    private bool spikesReloaded;

    // Damage variable allowing the spikes to hurt player
    [SerializeField]
    private float spikeDamage;

    private void Start()
    {
        // on start makes sure the SpikeTriggerRoutine is off and not autorunning.
        SpikeTriggerRoutine = null;
        // makes sure the spikes are down and not up
        spikesReloaded = true;
        //clears the lists 
        listControllers.Clear();
        listSpikes.Clear();

        //makes an array with the Spike object and its children, this would be useful for multiple individual spikes
        Spike[] arr = this.gameObject.GetComponentsInChildren<Spike>();
        foreach (Spike s in arr)
        {
            // adds any extra spikes to its own list
            listSpikes.Add(s);
        }
    }

    IEnumerator TriggerSpikes()
    {
        //sets spikes reloaded to false when triggered and shoots the spikes up.
        spikesReloaded = false;

        foreach (Spike s in listSpikes)
        {
            s.Shoot();
        }

        //waits one second
        yield return new WaitForSeconds(1f);
        // then starts to retract the spikes
        foreach (Spike s in listSpikes)
        {
            s.Retract();
        }

        //waits one second
        yield return new WaitForSeconds(1f);

        // updates SpikeTriggerRoutine to null
        SpikeTriggerRoutine = null;
        //reloads the spikes
        spikesReloaded = true;

    }


    private void OnTriggerEnter(Collider other)
    {
        //when the trigger is entered it will check to see if the item that stepped on it has a combat controller
        CombatController control = other.gameObject.transform.root.gameObject.GetComponent<CombatController>();
        // if there is no controller nothing happens
        if (control != null)
        {
            //if there is a controller then it will add it to the list of controllers confirming this is a item that can step in the trap and be damaged.

            if (!listControllers.Contains(control))
            {
                listControllers.Add(control);
            }

            //if the listControllers is not 0 then and the health of the person is not 0 then it will trigger the spikes to rise.
            if (listControllers.Count != 0)
            {
                if (control.Health != 0)
                {
                    //if the spikes aren't already triggered and they are reloaded then it will shoot
                    if (SpikeTriggerRoutine == null && spikesReloaded)
                    {
                        SpikeTriggerRoutine = StartCoroutine(TriggerSpikes());
                        //damages player everytime spikes are triggered.
                        control.TakeDamage(spikeDamage);
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the thing that left the trigger box has a combat controller...
        if (other.TryGetComponent<CombatController>(out CombatController controller))
        {
            // Remove them from the list.
            listControllers.Remove(controller);
        }
    }
}
