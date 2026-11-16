using UnityEngine;

public static class Formulas 
{
    //drag force = .5* p*v^2*Cd*A
    // p = density
    //v = velocity
    // Cd = drag coefficent
    //A = Cross sectional area
    public static float Drag(Vector3 velocity)
    {
        float p = 1.225f; //air density kg/m^3
        float v = velocity.magnitude;
        float Cd = .47f; // .47 = drag coefficient of a sphere
        float A = 1.0f;

        return .5f * p * (v * v) * Cd * A;

    }
}
