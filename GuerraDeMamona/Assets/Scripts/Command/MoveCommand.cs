using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class MoveCommand : Command
{
    private Vector3 targetPosition;
    private Vector3 startPosition;

    public MoveCommand(EntityBase entityToMove, Vector3 startPosition, Vector3 targetPosition)
    {
        this.selectedEntity = entityToMove;
        this.targetPosition = targetPosition;
        this.startPosition = startPosition;

        selectedEntity.transform.DOLocalMove(targetPosition, selectedEntity.GetMoveDuration(targetPosition));
    }

    protected override async Task AsyncExecuter()
    {
        //await selectedEntity.transform.DOLocalMove(targetPosition, selectedEntity.GetMoveDuration(targetPosition)).AsyncWaitForCompletion();
        //await Task.Delay(1000);
        await Task.Delay(100);
    }

    public override void UndoCommand()
    {
        selectedEntity.transform.DOLocalMove(startPosition, selectedEntity.GetMoveDuration(startPosition));
    }

    public Vector3 GetTargetPosition()
    {
        return targetPosition;
    }

    public Vector3 GetStartPosition()
    {
        return startPosition;
    }
}
