using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public CombatController healthToTrack;
    [SerializeField] private Image barFill;
    [SerializeField] private Vector3 trackingOffset;

    // Update is called once per frame
    void Update()
    {
        //if theres a combat controller to track the health of...
        if(healthToTrack)
        {
            //keep the bar updated and in the offset that was set
            barFill.fillAmount = healthToTrack.Health / healthToTrack.CharacterData.MaxHealth;
            gameObject.transform.position = healthToTrack.gameObject.transform.position + trackingOffset;
            //if the health is depleted then delete this object
            if(healthToTrack.Health <= 0)
            {
                DestroyBar();
            }
        }
    }

    //function for instancing to set a target to the bar
    public void SetTarget(CombatController theHealth)
    {
        healthToTrack = theHealth;
    }

    //destroy bar object
    public void DestroyBar()
    {
        Destroy(gameObject);
    }
}
