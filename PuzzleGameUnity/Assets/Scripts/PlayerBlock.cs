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
    public bool moving = false;
    public bool frozen = false;
    //public int targetX;
    //public int targetY;

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

    /*public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Activate!");
        if (!moving)
        {
            if (eventData.button == PointerEventData.InputButton.Left) activate();
            else if (eventData.button == PointerEventData.InputButton.Right) deactivate();
        }
    }*/

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
