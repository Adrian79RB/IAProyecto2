using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TaskCollection
{
    public List<TaskCollection> children
    {
        get
        {
            return children;
        }

        set
        {
            children = value;
        }
    }

    public void setChildren(List<TaskCollection> newChildren) 
    {
        foreach (var child in newChildren)
            children.Add(child);
    }

    public abstract bool Run();
}

public class Selector : TaskCollection
{
    public override bool Run()
    {
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i].Run())
                return true;
        }

        return false;
    }
}

public class Sequence : TaskCollection
{
    public override bool Run()
    {
        for (int i = 0; i < children.Count; i++)
        {
            if (!children[i].Run())
                return false;
        }

        return true;
    }
}

public abstract class Decorator : TaskCollection
{
    public abstract TaskCollection child { get; set; }
}

public class UntilFail : Decorator
{
    private TaskCollection _child;
    public override TaskCollection child
    {
        get
        {
            return _child;
        }

        set
        {
            _child = value;
        }
    }

    public override bool Run()
    {
        while (true)
        {
            if (!_child.Run())
                break;
        }

        return true;
    }
}


