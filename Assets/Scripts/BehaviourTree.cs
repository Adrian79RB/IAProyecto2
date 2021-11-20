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

    TaskCollection rootTask;
    List<nodeTask> treeConstructor;
    int maxChildren = 10;


    public BehaviourTree()
    {
        rootTask = new Sequence();
        treeConstructor = new List<nodeTask>();
        nodeTask currentTask;

        //Primer hijo del nodo raíz
        currentTask = new nodeTask(new Sequence(), rootTask);
        treeConstructor[0] = currentTask;

        for (int i = 1; i < maxChildren; i++)
        {
            switch(i){
                case 1:
                    currentTask = new nodeTask(new ShopCondition(), currentTask.task); break;
                case 2:
                    currentTask = new nodeTask(new GoShopping(), treeConstructor[0].task); break;
                case 3:
                    currentTask = new nodeTask(new Selector(), rootTask); break;
                case 4:
                    currentTask = new nodeTask(new Sequence(), currentTask.task); break;
                case 5:
                    currentTask = new nodeTask(new AttackCondition(), currentTask.task); break;
                case 6:
                    currentTask = new nodeTask(new Attack(), treeConstructor[4].task); break;
                case 7:
                    currentTask = new nodeTask(new Sequence(), treeConstructor[3].task); break;
                case 8:
                    currentTask = new nodeTask(new Move(), currentTask.task); break;
                case 9:
                    currentTask = new nodeTask(new Attack(), treeConstructor[7].task); break;
            }

            treeConstructor.Add(currentTask);
        }

        buildBehaviourTree();
    }

    public void buildBehaviourTree()
    {
        for(int i = 0; i < treeConstructor.Count; i++)
        {
            treeConstructor[i].father.children.Add(treeConstructor[i].task);
        }
    }

    public int StartBehaviour(Agent agent)
    {
        return rootTask.Run(agent);
    }
}
