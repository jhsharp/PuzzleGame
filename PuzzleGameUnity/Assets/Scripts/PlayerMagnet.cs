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
        Debug.Log("Activate!");
        base.activate();
        if (core.playerBlocks.Contains(this)) core.magnetMoveStart(magnetDirectionVector);
    }
}
