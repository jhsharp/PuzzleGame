/****
 * Created by: Jacob Sharp
 * Date Created: January 30, 2022
 * 
 * Last Edited by: Jacob Sharp
 * Date Last Edited: March 2, 2022
 * 
 * Description: Base class for all player blocks
 ****/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerBlock : Block//, IPointerClickHandler
{
    public bool moving = false; // indicates whether the block is currently in motion
    public bool frozen = false; // can be used to stop the block from activating/deactivating

    [HideInInspector] public int tempOffset = 0; // variable used when connecting blocks over long distances to avoid pushing them into walls

    internal void Start()
    {
        base.Start();
    }

    internal void Update()
    {
        base.Update();
        if (moving) fixedPosition = false;
        else fixedPosition = true;
    }

    public void OnMouseOver()
    {
        if (!moving && !frozen)
        {
            if (Input.GetMouseButtonDown(0)) activate();
            else if (Input.GetMouseButtonDown(1)) deactivate();
        }
    }

    virtual public void activate()
    {

    }

    virtual public void deactivate()
    {

    }
}
