using System.Collections;
using System.Collections.Generic;

public static class Utility // C#������static�����ⲿ��
{
    public static T[] ShuffleArray<T>(T[] array, int seed) // ������������T��һ������
    {
        System.Random prng = new System.Random(seed); // ����seed�ǿ��Թ̶�����˳�� 
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
