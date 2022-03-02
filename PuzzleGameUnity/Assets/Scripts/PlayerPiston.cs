/****
 * Created by: Jacob Sharp
 * Date Created: February 28, 2022
 * 
 * Last Edited by: Jacob Sharp
 * Date Last Edited: March 2, 2022
 * 
 * Description: Manages the extension and retraction of piston blocks
 ****/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPiston : PlayerBlock
{
    public int pistonDirection;

    [HideInInspector] public int extension = 0;
    public int extensionMax = 3;

    public GameObject armParent;
    public GameObject[] armSegments;

    private PlayerCore core;
    private Vector3 pistonDirectionVector;

    private float moveSpeed;
    private Vector3 targetPos;

    private bool extending = false;
    private bool retracting = false;
    private bool armMoving = false;

    void Start()
    {
        base.Start();

        if (pistonDirection == 0) pistonDirectionVector = up;
        else if (pistonDirection == 1) pistonDirectionVector = right;
        else if (pistonDirection == 2) pistonDirectionVector = down;
        else if (pistonDirection == 3) pistonDirectionVector = left;

        core = FindObjectOfType<PlayerCore>();

        moveSpeed = core.pistonMoveSpeed;
    }

    void Update()
    {
        base.Update();
        if (extending)
        {
            pistonExtend();
            if (Mathf.Abs(armParent.transform.localPosition.x - targetPos.x) < (moveSpeed * Time.deltaTime) && Mathf.Abs(armParent.transform.localPosition.x - targetPos.x) < (moveSpeed * Time.deltaTime))
            {
                armParent.transform.localPosition = targetPos;
                core.freezeBlocks(false);
                extending = false;
            }
        }
        if (retracting)
        {
            pistonRetract();
            if (Mathf.Abs(armParent.transform.localPosition.x - targetPos.x) < moveSpeed && Mathf.Abs(armParent.transform.localPosition.x - targetPos.x) < moveSpeed)
            {
                armParent.transform.localPosition = targetPos;
                core.freezeBlocks(false);
                retracting = false;
            }
        }
        if (armMoving)
        {
            pistonMoveArm();
            if (!moving) armMoving = false;
        }
    }

    public override void activate()
    {
        base.activate();
        if (extension >= extensionMax) return;
        if (core.playerBlocks.Contains(this))
        {
            armSegments[extension].SetActive(true);
            extension++;
            Block collisionBlock = manager.checkBlock(x + (int)pistonDirectionVector.x * (extension), y + (int)pistonDirectionVector.y * (extension));
            if (collisionBlock != null)
            {
                if (core.playerBlocks.Contains(collisionBlock) && collisionBlock != this) // if the direction of the piston contains a player controlled block, don't extend the piston
                {
                    extension--;
                    armSegments[extension].SetActive(false);
                }
                else // otherwise attempt to push off of the wall
                {
                    if (core.pistonMoveStart(-pistonDirectionVector)) pistonMoveArmStart();
                    ;
                }
            }
            else // if there is no block in the way of the pison, extend the arm
            {
                pistonExtendStart();
            }
        }
    }

    public override void deactivate()
    {
        base.deactivate();
        if (core.playerBlocks.Contains(this) && extension > 0)
        {
            extension--;
            armSegments[extension].SetActive(false);
            pistonRetractStart();
        }
    }

    public void pistonExtendStart()
    {
        targetPos = armParent.transform.localPosition + pistonDirectionVector * manager.blockSize;
        core.freezeBlocks(true);
        extending = true;
    }
    public void pistonExtend()
    {
        armParent.transform.localPosition += pistonDirectionVector * moveSpeed * Time.deltaTime;
    }

    public void pistonRetractStart()
    {
        targetPos = armParent.transform.localPosition - pistonDirectionVector * manager.blockSize;
        core.freezeBlocks(true);
        retracting = true;
    }

    public void pistonRetract()
    {
        armParent.transform.localPosition -= pistonDirectionVector * moveSpeed * Time.deltaTime;
    }

    public void pistonMoveArmStart()
    {
        targetPos = armParent.transform.position;
        armMoving = true;
    }

    public void pistonMoveArm()
    {
        armParent.transform.position = targetPos;
    }
}
