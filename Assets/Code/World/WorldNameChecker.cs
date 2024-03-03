using System.Text.RegularExpressions;

public class WorldNameChecker
{
    public static bool IsNameValid(string name) {
        Regex regex = new Regex("^[a-zA-Z0-9_]+( [a-zA-Z0-9_]+)*$");
        return regex.IsMatch(name) && name.Length <= WorldStorageProperties.worldNameLimit + 1;
    }

    public static bool IsSeedValid(string seed) {
        Regex regex = new Regex("^[a-zA-Z0-9_]+( [a-zA-Z0-9_]+)*$");
        return regex.IsMatch(seed) && seed.Length <= WorldStorageProperties.worldSeedLimit + 1;
    }
}
