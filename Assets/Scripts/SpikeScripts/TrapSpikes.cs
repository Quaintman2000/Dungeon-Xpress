using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSpikes : MonoBehaviour
{
    //Makes a list to detect all game items using the combat controller script
    public List<CombatController> ListControllers = new List<CombatController>();

    public List<Spike> ListSpikes = new List<Spike>();

    // references a coroutine to trigger the spikes to rise
    Coroutine SpikeTriggerRoutine;
    // refernces a bool to check if spikes a reloaded/down.
    bool SpikesReloaded;

    // Damage variable allowing the spikes to hurt player
    public float SpikeDamage;

    private void Start()
    {
        // on start makes sure the SpikeTriggerRoutine is off and not autorunning.
        SpikeTriggerRoutine = null;
        // makes sure the spikes are down and not up
        SpikesReloaded = true;
        //clears the lists 
        ListControllers.Clear();
        ListSpikes.Clear();

        //makes an array with the Spike object and its children, this would be useful for multiple individual spikes
        Spike[] arr = this.gameObject.GetComponentsInChildren<Spike>();
        foreach (Spike s in arr)
        {
            // adds any extra spikes to its own list
            ListSpikes.Add(s);
        }


    }

    private void Update()
    {
        //if the listControllers is not 0 then and the health of the person is not 0 then it will trigger the spikes to rise.
        if (ListControllers.Count != 0)
        {
            foreach (CombatController control in ListControllers)
            {
                if (control.Health != 0)
                {
                    //if the spikes aren't already triggered and they are reloaded then it will shoot
                    if (SpikeTriggerRoutine == null && SpikesReloaded)
                    {
                        SpikeTriggerRoutine = StartCoroutine(TriggerSpikes());
                        //damages player everytime spikes are triggered.
                        control.TakeDamage(SpikeDamage);




                    }

                }
            }
        }
    }

    IEnumerator TriggerSpikes()
    {
        //sets spikes reloaded to false when triggered and shoots the spikes up.
        SpikesReloaded = false;

        foreach (Spike s in ListSpikes)
        {
            s.Shoot();
        }



        //waits one second
        yield return new WaitForSeconds(1f);
        // then starts to retract the spikes
        foreach (Spike s in ListSpikes)
        {
            s.Retract();
        }

        //waits one second
        yield return new WaitForSeconds(1f);

        // updates SpikeTriggerRoutine to null
        SpikeTriggerRoutine = null;
        //reloads the spikes
        SpikesReloaded = true;

        //removes player from control list
        //which stops the spikes from shooting up multiple times if the player is still standing on the spikes
        foreach (CombatController control in ListControllers)
            ListControllers.Remove(control);



    }
    public static bool IsTrap(GameObject obj)
    {
        //checks if trap spikes are null, if so it will return false if they are not null it will return true indicated that the item is on the level.
        if (obj.transform.root.gameObject.GetComponent<TrapSpikes>() == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //when the trigger is entered it will check to see if the item that stepped on it has a combat controller
        CombatController control = other.gameObject.transform.root.gameObject.GetComponent<CombatController>();
        // if there is no controller nothing happens
        if (control != null)
        {
            //if there is a controller then it will add it to the list of controllers confirming this is a item that can step in the trap and be damaged.

            if (!ListControllers.Contains(control))
            {
                ListControllers.Add(control);


            }


        }



    }
   
}
