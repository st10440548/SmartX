using SmartX.Api.Models;

namespace SmartX.Api.Services;

public class DeploymentValidationService
{
    public bool ValidateNode(DeviceNode node)
    {
        if (!node.IsConfigured)
        {
            return false;
        }

        foreach (var child in node.Children)
        {
            if (!ValidateNode(child))
            {
                return false;
            }
        }

        return true;
    }
}
