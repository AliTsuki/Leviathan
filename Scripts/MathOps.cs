using UnityEngine;

// Useful general maths operations not included natively in C# or UnityEngine
public static class MathOps
{
    //Finds the Modulo of 2 given floats, behaves slightly different than % operator
    public static float Modulo(float a, float b)
    {
        return a - (b * Mathf.Floor(a / b));
    }
}
