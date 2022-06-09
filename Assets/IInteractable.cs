using System.Collections.Generic;

public interface IInteractable
{
    public abstract List<MessageEnvelope> ReceiveInteraction(Enums.Direction direction);
}