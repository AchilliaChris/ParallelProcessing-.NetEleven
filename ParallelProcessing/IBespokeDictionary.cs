using System.Collections.Concurrent;

namespace ParallelProcessing
{
    public interface IBespokeDictionary
    {
        ConcurrentDictionary<string, Widget> widgets { get; set; }

        void AddIndexedSingleVat(int x, double rate);
        void AddSingleVat(KeyValuePair<string, Widget> pair, double rate);
        Task AddSingleVatAsync(KeyValuePair<string, Widget> pair, double rate);
        int AddVat(double rate);
        void AddVatSpecific(double rate);
        bool MakeWidgets(int size);
        Task<int> ParallelForEachAsyncOptionsVat(ParallelOptions parallelOptions, double rate);
        Task<int> ParralelForEachOptionsVat(ParallelOptions parallelOptions, double rate);
        Task<int> ParralelForEachVat(double rate);
        Task<int> ParralelForOptionsVat(ParallelOptions parallelOptions, double rate);
        Task<int> ParralelForVat(double rate);
    }
}