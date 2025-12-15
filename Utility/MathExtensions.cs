namespace HMV_Player.Utility;

public static class MathExtensions {
    public static float InverseLerp(float a, float b, float value) {
        if (a == b) {
            return 1f;
        }

        float t = (value - a) / (b - a);
        return t;
    }
    
    public static float InverseLerp(double a, double b, double value) {
        if (a == b) {
            return 1f;
        }

        double t = (value - a) / (b - a);
        return (float) t;
    }
}