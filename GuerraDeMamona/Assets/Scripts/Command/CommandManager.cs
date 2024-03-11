using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommandManager : Singleton<CommandManager>
{
    //Commands
    private Command activeCommand;
    private Queue<Command> commandQueue = new Queue<Command>();
    public Action OnQueueEndEvent;

    private const int maxActionsPerTurn = 5;

    private VisualCommandsController visualController;

    [Header("Entitys")]
    [SerializeField] private EntityBase selectedEntity;

    private void Start()
    {
        visualController = GetComponent<VisualCommandsController>();
    }

    public void EnqueueCommand(Command command)
    {
        if (commandQueue.Count < maxActionsPerTurn)
        {
            commandQueue.Enqueue(command);
            visualController.CreateVisualCard(command);
        }
    }

    public void UndoCommand(Command command)
    {
        command.UndoCommand();

        commandQueue = new Queue<Command>(commandQueue.Where(c => c != command));
        visualController.RemoveVisualCard(command);
    }

    public void RunQueue()
    {
        StartCoroutine(RunQueueRoutine());
    }

    IEnumerator RunQueueRoutine()
    {
        while (commandQueue.Count > 0) { 
            activeCommand = commandQueue.Dequeue();
            activeCommand.Execute();
            visualController.RemoveVisualCard(activeCommand);
            yield return new WaitUntil(() => activeCommand.IsExecuting == false);
        }

        OnQueueEndEvent?.Invoke();
    }
}
