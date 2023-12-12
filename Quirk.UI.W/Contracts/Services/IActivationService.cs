namespace Quirk.UI.W.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
