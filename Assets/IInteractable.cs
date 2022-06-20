using System.Collections.Generic;

public interface IInteractable
{
    public abstract InteractionEvent ReceiveInteraction(Enums.Direction direction);
    public abstract InteractionEvent ReceiveInteraction(object promptId);
}