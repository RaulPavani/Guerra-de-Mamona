using System.Threading.Tasks;
using TMPro;

public class AttackCommand : Command
{
    public AttackCommand(EntityBase attacker, EntityBase defender)
    {
        this.selectedEntity = attacker;
        this.targetEntity = defender;
    }

    protected override async Task AsyncExecuter()
    {
        selectedEntity.Attack();
        targetEntity?.TakeDamage(2);
        await Task.Delay(1000);
    }
}