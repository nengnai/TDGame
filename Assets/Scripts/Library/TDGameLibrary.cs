// 

using UnityEngine;

namespace TDGameLibrary
{
    public static class TDUtility
    {
        public static float BoolToFloat(bool bBool) => bBool ? 1.0f : 0.0f;
        

        public static float FInterpTo(float Current, float Target, float DeltaTime, float InterpSpeed)
        {
            if (InterpSpeed <= 0f) return Target;
            float Dist = Target - Current;
            if (Dist * Dist < Mathf.Epsilon) return Target;
            return Current + Dist * Mathf.Clamp01(DeltaTime * InterpSpeed);
        }

        public static float Remap(float InValue, float InMin, float InMax, float OutMin, float OutMax)
        {
            if (Mathf.Approximately(InMin, InMax))
                {return (OutMin + OutMax) * 0.5f;}
            
            return OutMin + (OutMax - OutMin) * ((InValue - InMin) / (InMax - InMin));
        }
    }
}