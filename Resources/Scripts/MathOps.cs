using UnityEngine;

// Math Operations class contains useful general Maths operations not included natively in C# or UnityEngine
public static class MathOps
{
    //Finds the Modulo of 2 given floats, more useful than % which is just remainder...
    public static float Modulo(float a, float b)
    {
        return a - (b * Mathf.Floor(a / b));
    }
}
