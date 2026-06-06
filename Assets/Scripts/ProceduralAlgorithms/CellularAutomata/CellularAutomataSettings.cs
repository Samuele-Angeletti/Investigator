public class CellularAutomataSettings
{
    public int Width = 50;
    public int Height = 50;

    /// <summary>
    /// Percentuale di inizializzazione dei muri sulla griglia.
    /// se 100 => tutto muro
    /// se 0   => tutto pavimento
    /// Non usare estremità
    /// </summary>
    public int InitialWallChance = 45;

    /// <summary>
    /// Numero di iterazioni da eseguire per ottenere forme più lisce e omogenee
    /// 
    /// Consigliato > 4
    /// </summary>
    public int Steps = 5;

    /// <summary>
    /// Valore che indica quanti vicini attorno a me devono essere "muro" per definire muro anche me
    /// (principio di vicinato di Moore)
    /// >= 5 => mappe più aperte, grotte più grandi
    /// <= 4 => mappe più dense, tunnel più stretti
    /// </summary>
    public int BirthLimit = 4;

    /// <summary>
    /// Limite per creare il pavimento secondo lo stesso principio di BirthLimit
    /// </summary>
    public int DeathLimit = 3;

    /// <summary>
    /// Indica se i bordi della mappa devono essere sempre muro oppure no
    /// </summary>
    public bool SolidBorder = false;

    public int Seed = 45;

    public bool RandomSeed = false;
}
