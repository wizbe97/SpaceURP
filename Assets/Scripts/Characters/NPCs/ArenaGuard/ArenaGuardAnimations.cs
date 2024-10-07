using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaGuardAnimations : NPCAnimationState
{
    protected override void Start()
    {
        base.Start();  // Call base class Start method
    }

    private void Update()
    {
        UpdateNPCAnimationState();
    }

}
