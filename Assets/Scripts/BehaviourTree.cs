using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct nodeTask
{
    public TaskCollection task;
    public TaskCollection father;

    public nodeTask(TaskCollection newTask, TaskCollection newFather)
    {
        task = newTask;
        father = newFather;
    }
}

public class BehaviourTree
{
    public TaskCollection rootTask;

    private List<nodeTask> treeConstructor;

    public BehaviourTree()
    {
        rootTask = new Selector();
        treeConstructor = new List<nodeTask>();
        nodeTask currentTask;
        int index = 0;

        //Primer hijo del nodo raíz
        currentTask = new nodeTask(new Sequence(), rootTask);
        treeConstructor[index] = currentTask;
        index++;
    }

    public void buildBehaviourTree()
    {
        for(int i = 0; i < treeConstructor.Count; i++)
        {
            treeConstructor[i].father.children.Add(treeConstructor[i].task);
        }
    }
}
