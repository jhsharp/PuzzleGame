/****
 * Created by: Jacob Sharp
 * Date Created: January 30, 2022
 * 
 * Last Edited by: Jacob Sharp
 * Date Last Edited: January 30, 2022
 * 
 * Description: Manages activation of magnet blocks
 ****/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagnet : PlayerBlock
{
    public int magnetDirection;

    private PlayerCore core;
    private Vector3 magnetDirectionVector;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        if (magnetDirection == 0) magnetDirectionVector = up;
        else if (magnetDirection == 1) magnetDirectionVector = right;
        else if (magnetDirection == 2) magnetDirectionVector = down;
        else if (magnetDirection == 3) magnetDirectionVector = left;
        core = FindObjectOfType<PlayerCore>();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    public override void activate()
    {
        base.activate();
        if (core.playerBlocks.Contains(this)) core.magnetMoveStart(magnetDirectionVector);
    }
}
