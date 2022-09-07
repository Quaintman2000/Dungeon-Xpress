using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IInteractable
{
    public void Interact(InteractionController controller);
    public void SetFocused(bool isFocused);

    
}
