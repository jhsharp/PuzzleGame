using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerBlock : Block//, IPointerClickHandler
{
    public bool moving = false;
    public int targetX;
    public int targetY;

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
        if (!moving)
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
