using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class WorldNameChecker
{
    public static bool IsNameValid(string name) {
        Regex regex = new Regex("^[a-zA-Z0-9_]+( [a-zA-Z0-9_]+)*$");
        return regex.IsMatch(name);
    }

    public static bool IsSeedValid(string seed) {
        Regex regex = new Regex("^[a-zA-Z0-9_]+( [a-zA-Z0-9_]+)*$");
        return regex.IsMatch(seed);
    }
}
