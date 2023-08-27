using System.Collections;

public class EndAction : ActionThing
{
    public override string thingSubType
    {
        get => "End";
    }

    protected override IEnumerator RunAction()
    {
        user.OccupyCurrentNode();

        GameManager.NextCharacter();

        // The action is no longer running
        EndAction(false);

        yield return null;
    }
}
