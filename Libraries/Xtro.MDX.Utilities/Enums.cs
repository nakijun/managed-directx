namespace Xtro.MDX.Utilities
{
    public enum MatchType
    {
        IgnoreInput = 0,  // Use the closest valid value to a default 
        PreserveInput,    // Use input without change, but may cause no valid device to be found
        ClosestToInput    // Use the closest valid value to the input 
    };
}
