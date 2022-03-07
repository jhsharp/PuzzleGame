/****
 * Created by: Jacob Sharp
 * Date Created: March 4, 2022
 * 
 * Last Edited by: Jacob Sharp
 * Date Last Edited: March 4, 2022
 * 
 * Description: Manages the opening of locks
 ****/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockBlock : Block
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    public void open() // open the lock and other locks connected to it
    {
        manager.clearBlock(this); // clear the lock in the manager grid

        // Open any adjacent locks
        if (manager.checkBlock(x + 1, y) != null && manager.checkBlock(x + 1, y).type == "lock") manager.checkBlock(x + 1, y).GetComponent<LockBlock>().open();
        if (manager.checkBlock(x - 1, y) != null && manager.checkBlock(x - 1, y).type == "lock") manager.checkBlock(x - 1, y).GetComponent<LockBlock>().open();
        if (manager.checkBlock(x, y + 1) != null && manager.checkBlock(x, y + 1).type == "lock") manager.checkBlock(x, y + 1).GetComponent<LockBlock>().open();
        if (manager.checkBlock(x, y - 1) != null && manager.checkBlock(x, y - 1).type == "lock") manager.checkBlock(x, y - 1).GetComponent<LockBlock>().open();

        Destroy(this.gameObject); // remove the lock from the scene
    }
}
