using System;
using System.Collections.Generic;

/// <summary>
/// Rappresenta un nodo dell'albero del BSP (Binary Space Partitioning)
/// E' un rettangolo che a sua volta può essere suddiviso in 2 parti:
/// (sinistra/destra o sotto/sopra)
/// </summary>
public class BSPPartition
{
    #region DATI GEOMETRICI
    /// <summary>
    /// Coordinata X del bordo sinistro della partizione
    /// </summary>
    public int X { get; }

    /// <summary>
    /// Coordinata Y del bordo inferiore della partizione
    /// </summary>
    public int Y { get; }

    /// <summary>
    /// Lunghezza della partizione in celle
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Altezza della partizione in celle
    /// </summary>
    public int Height { get; }
    #endregion

    #region STRUTTURA AD ALBERO
    /// <summary>
    /// Figlio della divisione che può essere sinistra o sopra
    /// </summary>
    public BSPPartition First { get; private set; }
    /// <summary>
    /// Figlio della divisione che può essere destra o sotto
    /// </summary>
    public BSPPartition Second { get; private set; }
    #endregion

    #region STANZA

    // per scelta architetturale usiamo una tupla
    public (int x, int y, int width, int height)? Room { get; private set; }

    #endregion

    #region COSTRUTTORE
    public BSPPartition(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
    #endregion

    #region PUBLIC API
    bool IsLeaf => First == null && Second == null;

    /// <summary>
    /// Tenta di dividere questa partizione in due figli
    /// </summary>
    /// <param name="random">Generatore casuale di direzione</param>
    /// <param name="minWidth">Larghezza minima che ogni figlio deve avere</param>
    /// <param name="minHeight">Altezza minima che ogni figlio deve avere</param>
    /// <returns>True se il taglio è riuscito, false se la partizione era troppo piccola</returns>
    public bool TrySplit(Random random, int minWidth, int minHeight)
    {
        if (!IsLeaf)
            return false;

        bool splitHorizontally = ChooseSplitOrientation(random);

        if (splitHorizontally)
        {
            return TrySplitHorizontally(random, minHeight);
        }
        else
        {
            return TrySplitVertically(random, minWidth);
        }
    }

    /// <summary>
    /// Memorizza la stanza scavata all'interno di questa foglia
    /// Viene chiamato dal generatore dopo aver calcolato le coordinate della stanza
    /// </summary>
    public void AssignRoom(int x, int y, int width, int height)
    {
        Room = (x, y, width, height);
    }

    /// <summary>
    /// Metodo che percorre tutto il sottoalbero alla ricerca delle foglie,
    /// ovvero tutte le stanze che non hanno ulteriori suddivisioni.
    /// Ogni foglia rapprezenta un potenziale spazio per una stanza
    /// </summary>
    public List<BSPPartition> GetLeaves()
    {
        var leaves = new List<BSPPartition>();
        CollectLeaves(leaves);
        return leaves;
    }

    /// <summary>
    /// Metodo ricorsivo per ricercare tutte le foglie dell'albero
    /// </summary>
    /// <param name="leaves"></param>
    public void CollectLeaves(List<BSPPartition> leaves)
    {
        if (IsLeaf)
        {
            leaves.Add(this);
            return;
        }

        First?.CollectLeaves(leaves);
        Second?.CollectLeaves(leaves);
    }

    public (int x, int y)? GetRoomCenter()
    {
        // foglia con stanza assegnata: restituiamo il suo centro
        if (Room.HasValue)
        {
            var r = Room.Value;
            return (r.x + r.width / 2, r.y + r.height / 2);
        }

        // foglia senza stanza (stanza non generata per mancanza di spazio)
        if (IsLeaf)
            return null;

        // nodo interno: proviamo entrambi i figli e restituiamo il punto medio
        // se entrambi disponibili, o quello del figlio disponibile in caso contrario
        var centerFirst = First?.GetRoomCenter();
        var centerSecond = Second?.GetRoomCenter();

        if (centerFirst.HasValue && centerSecond.HasValue)
            return ((centerFirst.Value.x + centerSecond.Value.x) / 2,
                    (centerFirst.Value.y + centerSecond.Value.y) / 2);

        return centerFirst ?? centerSecond;
    }
    #endregion

    #region PRIVATE API

    private bool ChooseSplitOrientation(Random random)
    {
        float ratio = (float)Width / Height;

        if (ratio > 1.25f) return false; // partizione molto larga => taglio verticale
        if (ratio < 0.75f) return true; // partizione molto alta => taglio orizzontale

        return random.Next(2) == 0; // partizione molto simile nelle dimenzioni => taglio casuale
    }

    private bool TrySplitVertically(Random random, int minWidth)
    {
        int minCut = minWidth;
        int maxCut = Width - minWidth;

        if (minCut > maxCut) return false; // partizione troppo piccola per essere tagliata

        int cut = random.Next(minCut, maxCut);

        First = new BSPPartition(X, Y, cut, Height);
        Second = new BSPPartition(X + cut, Y, Width - cut, Height);
        return true;
    }

    private bool TrySplitHorizontally(Random random, int minHeight)
    {
        int minCut = minHeight;
        int maxCut = Height - minHeight;

        if (minCut > maxCut) return false; // partizione troppo piccola per essere tagliata

        int cut = random.Next(minCut, maxCut);

        First = new BSPPartition(X, Y, Width, cut);
        Second = new BSPPartition(X, Y + cut, Width, Height - cut);
        return true;
    }

    #endregion
}
