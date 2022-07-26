using System.Collections;
using System.Collections.Generic;

// Fisher-Yates打乱算法（不断从帽子里摸一张牌）
public static class Utility // C#可以用static修饰外部类
{
    public static T[] ShuffleArray<T>(T[] array, int seed) // 尖括号内声明T是一个泛型
    {
        System.Random prng = new System.Random(seed); // 因为MapEditor会一直刷新，所以要用seed固定打乱顺序
        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = prng.Next(i, array.Length);
            T tempItem = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = tempItem;
        }
        return array;
    }
}
