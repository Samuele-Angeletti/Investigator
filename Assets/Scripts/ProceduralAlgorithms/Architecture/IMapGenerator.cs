public interface IMapGenerator
{
    /// <summary>
    /// Esegue la generazione e restituisce un <see cref="GenerationResult"/> contenente la mappa grezza come griglia di interi
    /// </summary>
    /// <returns></returns>
    GenerationResult Generate();
}