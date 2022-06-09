using System.Collections.Generic;

public interface IInteractible
{
    public abstract List<MessageEnvelope> ReceiveInteraction(Enums.Direction direction);
}