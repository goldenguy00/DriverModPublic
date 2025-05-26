namespace RobDriver.SkillStates.Driver.SupplyDrop.Nerfed
{
    public class AimCrapDrop : AimSupplyDrop
    {
        protected override CancelSupplyDrop GetCancelState() => new CancelCrapDrop();
        protected override FireSupplyDrop GetFireState() => new FireCrapDrop();
    }
}