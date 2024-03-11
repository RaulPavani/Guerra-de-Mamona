using System.Threading.Tasks;
using UnityEngine;

public abstract class Command
{
    private bool isExecuting = false;
    public bool IsExecuting => isExecuting;

    protected EntityBase selectedEntity;
    protected EntityBase targetEntity;

    public async void Execute()
    {
        isExecuting = true;
        await AsyncExecuter();
        isExecuting = false;
    }

    protected abstract Task AsyncExecuter();

    public virtual void UndoCommand()
    {

    }

    //Visual
    public EntityBase GetSelectedCharacter()
    {
        return selectedEntity;
    }

    public EntityBase GetTargetCharacter()
    {
        return targetEntity;
    }

    public Sprite GetPortraitSelectedCharacter()
    {
        return selectedEntity.GetPortrait();
    }

    public Sprite GetPortraitTargetCharacter()
    {
        return targetEntity?.GetPortrait();
    }
}
