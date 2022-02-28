/****
 * Created by: Jacob Sharp
 * Date Created: February 28, 2022
 * 
 * Last Edited by: Jacob Sharp
 * Date Last Edited: February 28, 2022
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

    public GameObject[] armSegments;

    private PlayerCore core;
    private Vector3 pistonDirectionVector;

    private bool extending = false;
    private bool retracting = false;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        if (pistonDirection == 0) pistonDirectionVector = up;
        else if (pistonDirection == 1) pistonDirectionVector = right;
        else if (pistonDirection == 2) pistonDirectionVector = down;
        else if (pistonDirection == 3) pistonDirectionVector = left;
        core = FindObjectOfType<PlayerCore>();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (extending)
        {

        }
        if (retracting)
        {

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
            if (collisionBlock != null && !core.playerBlocks.Contains(collisionBlock))
            {
                core.pistonMoveStart(-pistonDirectionVector);
            }
            else
            {
                pistonExtend();
            }
        }
    }

    public override void deactivate()
    {
        base.deactivate();
        if (core.playerBlocks.Contains(this) && extension > 0)
        {
            armSegments[extension].SetActive(false);
            extension--;
            pistonRetract();
        }
    }

    public void pistonExtend()
    {

    }

    public void pistonRetract()
    {

    }
}
