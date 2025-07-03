using Godot;
using Godot.Collections;

namespace TO.Commons.Extensions;

public static class VariantArrayExtensions
{
    /// <summary>
    /// 创建原数组的副本，并交换指定两个索引位置的值
    /// </summary>
    public static Array<Variant> Swap(this Array<Variant> array, int index1, int index2)
    {
        ArgumentNullException.ThrowIfNull(array);
        if (index1 < 0 || index1 >= array.Count || index2 < 0 || index2 >= array.Count)
            throw new ArgumentOutOfRangeException(nameof(array));

        var newArray = new Array<Variant>(array); // 创建副本

        (newArray[index1], newArray[index2]) = (newArray[index2], newArray[index1]);

        return newArray;
    }
}