namespace GoTool.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
