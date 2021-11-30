using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TaskStates
{
    public static int FRESH = 0;
    public static int RUNNING = 1;
    public static int SUCCEEDED = 2;
    public static int FAILED = 3;
    
}


public abstract class TaskCollection
{
    public int state = TaskStates.FRESH;

    public List<TaskCollection> children;

    public TaskCollection()
    {
        children = new List<TaskCollection>();
    }

    public abstract int Run(Agent agent);
}

public class Selector : TaskCollection
{
    public override int Run(Agent agent)
    {
        state = TaskStates.RUNNING;

        for (int i = 0; i < children.Count; i++)
        {
            if (children[i].Run(agent) == TaskStates.SUCCEEDED)
            {
                state = TaskStates.FAILED;
                return state;
            }
        }

        state = TaskStates.FAILED;
        return state;
    }
}

public class Sequence : TaskCollection
{
    public override int Run(Agent agent)
    {
        state = TaskStates.RUNNING;
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i].Run(agent) == TaskStates.FAILED)
            {
                state = TaskStates.FAILED;
                return state;
            }
        }
        state = TaskStates.SUCCEEDED;
        return state;
    }
}

public abstract class Decorator : TaskCollection
{
    public TaskCollection child;
}

public class UntilFail_EndTurn : Decorator
{
    public override int Run(Agent agent)
    {
        child = children[0];
        state = TaskStates.RUNNING;
        while (true)
        {
            if (child.Run(agent) == TaskStates.FAILED)
                break;
        }
        agent.EndAgentTurn();
        return TaskStates.SUCCEEDED;
    }
}

public class ShopCondition : TaskCollection
{
    public override int Run(Agent agent)
    {
        state = TaskStates.RUNNING;

        if (agent.gameManager.player2Gold >= 40)
        {
            state = TaskStates.SUCCEEDED;
        }
        else
        {
            state = TaskStates.FAILED;
        }

        return state;
    }
}

public class GoShopping : TaskCollection
{
    public override int Run(Agent agent)
    {
        state = TaskStates.RUNNING;
        if (agent.tryToShop())
            state = TaskStates.SUCCEEDED;
        else
            state = TaskStates.FAILED;

        return state;
    }
}

public class Move : TaskCollection
{
    public override int Run(Agent agent)
    {
        state = TaskStates.RUNNING;
        if (agent.tryToMove())
            state = TaskStates.SUCCEEDED;
        else
            state = TaskStates.FAILED;

        return state;
    }
}

public class AttackCondition : TaskCollection
{
    public override int Run(Agent agent)
    {
        state = TaskStates.RUNNING;
        if (agent.checkEnemiesContact())
            state = TaskStates.SUCCEEDED;
        else
            state = TaskStates.FAILED;

        return state;
    }
}

public class Attack : TaskCollection
{
    public override int Run(Agent agent)
    {
        state = TaskStates.RUNNING;
        if (agent.tryToAttack())
            state = TaskStates.SUCCEEDED;
        else
            state = TaskStates.FAILED;

        return state;
    }
}




