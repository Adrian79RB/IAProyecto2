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
    int maxChildren = 11;


    public BehaviourTree()
    {
        rootTask = new UntilFail_EndTurn();
        treeConstructor = new List<nodeTask>();
        nodeTask currentTask;



        //Primer hijo del nodo raíz
        currentTask = new nodeTask(new Sequence(), rootTask);
        treeConstructor[0] = currentTask;

        for (int i = 1; i < maxChildren; i++)
        {
            switch(i){
                case 1:
                    currentTask = new nodeTask(new Sequence(), currentTask.task); break;
                case 2:
                    currentTask = new nodeTask(new ShopCondition(), currentTask.task); break;
                case 3:
                    currentTask = new nodeTask(new GoShopping(), treeConstructor[1].task); break;
                case 4:
                    currentTask = new nodeTask(new Selector(), treeConstructor[0].task); break;
                case 5:
                    currentTask = new nodeTask(new Sequence(), currentTask.task); break;
                case 6:
                    currentTask = new nodeTask(new AttackCondition(), currentTask.task); break;
                case 7:
                    currentTask = new nodeTask(new Attack(), treeConstructor[5].task); break;
                case 8:
                    currentTask = new nodeTask(new Sequence(), treeConstructor[4].task); break;
                case 9:
                    currentTask = new nodeTask(new Move(), currentTask.task); break;
                case 10:
                    currentTask = new nodeTask(new Attack(), treeConstructor[8].task); break;
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
