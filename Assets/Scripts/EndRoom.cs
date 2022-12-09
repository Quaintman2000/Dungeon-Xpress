using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndRoom : Room
{
    public InteractableDoor InteractableDoor => _interactableDoor;

    [SerializeField]
    private InteractableDoor _interactableDoor;
  
}
