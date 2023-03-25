using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class RaycastData
{
    public Vector3 ImpactPoint;
    public CombatController HitCombatant;
    public CombatController SenderCombatant;
    public HitResult Result;
    // Constructor
    public RaycastData(RaycastHit raycastHit, CombatController sender)
    {
        // Reference to sender of raycast.
        SenderCombatant = sender;
        // If we hit something...
        if (raycastHit.collider != null)
        {
            // Record impact point in world space.
            ImpactPoint = raycastHit.point;
            // If we hit something with a combat controller...
            if (raycastHit.collider.TryGetComponent<CombatController>(out HitCombatant))
            {
                // If the the combatant is ourselve, hit result = self. Else, Result = other.
                if (HitCombatant == SenderCombatant)
                    Result = HitResult.Self;
                else
                    Result = HitResult.Other;
            }
            else
            {
                Result = HitResult.Ground;
            }
        }
        else
        {
            Result = HitResult.Nothing;
        }
    }
}

public enum HitResult
{
    Self, Other, Ground, Nothing
}