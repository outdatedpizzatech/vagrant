using System.Collections.Generic;

public interface IInteractible
{
    public abstract List<NPCResponse> ReceiveInteraction(Enums.Direction direction);
}