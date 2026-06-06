using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Si occupa esclusivamente di costruire l'albero in BSP
/// </summary>
public class BSPTreeBuilder
{
    private readonly Random _random;
    private readonly int _minPartitionWidth;
    private readonly int _minPartitionHeight;

    public BSPTreeBuilder(Random random, int minPartitionWidth, int minPartitionHeight)
    {
        _random = random;
        _minPartitionWidth = minPartitionWidth;
        _minPartitionHeight = minPartitionHeight;
    }

    /// <summary>
    /// Costruisce l'intero BSP
    /// </summary>
    /// <param name="totalWidth"></param>
    /// <param name="totalHeight"></param>
    /// <returns></returns>
    public BSPPartition Build(int totalWidth, int totalHeight)
    {
        var root = new BSPPartition(0, 0, totalWidth, totalHeight);

        SplitRecursive(root);

        return root;
    }

    /// <summary>
    /// Tenta di dividere dalla root alle foglie
    /// </summary>
    /// <param name="root"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void SplitRecursive(BSPPartition root)
    {
        bool divided = root.TrySplit(_random, _minPartitionWidth, _minPartitionHeight);

        if (!divided)
            return;

        SplitRecursive(root.First);
        SplitRecursive(root.Second);
    }
}
