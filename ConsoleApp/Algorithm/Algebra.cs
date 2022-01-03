namespace Algorithm;

public class Algebra
{
    /// <summary>
    /// Returns true if the <paramref name="point"/> is in the triangle created with <paramref name="X"/>, <paramref name="Y"/> and <paramref name="Z"/>. Points are double array with length of 2
    /// </summary>
    /// <param name="X">First point of the triangle</param>
    /// <param name="Y">Second point if the triangle</param>
    /// <param name="Z">Third point if the triangle</param>
    /// <param name="point">The point where we will check if it is inside the triangle</param>
    /// <returns><see cref="bool"/></returns>
    public static bool PointWithinTriangle(double[] X, double[] Y, double[] Z, double[] point)
    {
        var triangle = new double[3][] { X, Y, Z };
        for (int i = 0; i < 3; i++)
        {
            var A = triangle[i];
            var B = triangle[(i + 1) % 3];
            var C = triangle[(i + 2) % 3];
            double M = (A[1] - B[1]) / (A[0] - B[0]);
            if ((C[1] - ((C[0] - A[0]) * M) > A[1]) != (point[1] - ((point[0] - A[0]) * M) > A[1]))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Returns true if the <paramref name="point"/> is in the triangular pyramid created with <paramref name="A"/>, <paramref name="B"/>, <paramref name="C"/> and <paramref name="D"/>. Points are double array with length of 3
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <param name="C"></param>
    /// <param name="D"></param>
    /// <param name="point"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool PointWithinTetrahedron(double[] A, double[] B, double[] C, double[] D, double[] point)
    {
        static double[] CrossProduct(double[] X, double[] Y)
        {
            return new double[] { X[1] * Y[2] - X[2] * Y[1], X[2] * Y[0] - X[0] * Y[2], X[0] * Y[1] - X[1] * Y[0] };
        }

        static double DotProduct(double[] X, double[] Y)
        {
            return X[0] * Y[0] + X[1] * Y[1] + X[2] * Y[2];
        }

        static double[] Subtract(double[] X, double[] Y)
        {
            return new double[] { X[0] - Y[0], X[1] - Y[1], X[2] - Y[2] };
        }

        static int Sign(double x)
        {
            return ((x == 0) ? 0 : ((x > 0) ? 1 : -1));
        }

        static bool SameArea(double[] X, double[] Y, double[] Z, double[] T, double[] point)
        {
            var normal = CrossProduct(Subtract(Y, X), Subtract(Z, X));
            var dotT = DotProduct(normal, Subtract(T, X));
            var dotP = DotProduct(normal, Subtract(point, X));
            return Sign(dotP) == Sign(dotT);
        }

        return SameArea(A, B, C, D, point) && SameArea(B, C, D, A, point) && SameArea(C, D, A, B, point) && SameArea(D, A, B, C, point);
    }

    /// <summary>
    /// Returns true if <paramref name="value"/> has exactly 3 divisors
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool ExactlyThree(int value)
    {
        int divided = 0;
        for (int i = 2; i <= value / 2; i++)
            if (value % i == 0)
                divided++;

        return divided == 1;
    }

    /// <summary>
    /// Returns the given <paramref name="value"/> is a valid leap year
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsLeapYear(int value)
    {
        while (value % 100 == 0)
            value /= 100;

        return value % 4 == 0;
    }
}
