using System.Collections.Generic;

public static class Encoder
{
    private static int[] primeNumbers = new int[101] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293, 307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397, 401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541, 547 };

    public static int Encode(int[] values)
    {
        int code = 1;

        foreach (int value in values)
        {
            if (0 <= value && value <= 100)
            {
                code *= primeNumbers[value];
            }
            else
            {
                throw new System.InvalidOperationException("Impossible d'encoder une valeur négative ou supérieur à 100.");
            }
        }

        return code;
    }

    public static int[] Decode(int code)
    {
        List<int> values = new List<int>();

        for (int i = 0; i < primeNumbers.Length; i++)
        {
            if (code % primeNumbers[i] == 0)
            {
                values.Add(i);
                code /= primeNumbers[i];

                if (code == 1)
                    return values.ToArray();
            }
        }

        throw new System.InvalidOperationException("Impossible de déchiffrer le code.");
    }
}
