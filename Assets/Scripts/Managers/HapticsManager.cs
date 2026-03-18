
public class HapticsManager
{
    public static void LightHaptic()
    {
        Taptic.Light();
    }    
    
    public static void MediumHaptic()
    {
        Taptic.Medium();
    }

    public static void HeavyHaptic()
    {
        Taptic.Heavy();
    }

    public static void SelectionHaptic()
    {
        Taptic.Selection();
    }

    public static void SuccessHaptic()
    {
        Taptic.Success();
    }

    public static void WarningHaptic()
    {
        Taptic.Warning();
    }
}