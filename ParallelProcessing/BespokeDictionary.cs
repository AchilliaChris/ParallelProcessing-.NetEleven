using System.Collections.Concurrent;

namespace ParallelProcessing
{
    public class BespokeDictionary : IBespokeDictionary
    {
        public ConcurrentDictionary<string, Widget> widgets { get; set; }
        private const int length = 10;

        public bool MakeWidgets(int size)
        {
            widgets = new ConcurrentDictionary<string, Widget>();
            for (int i = 0; i < size; i++)

            {
                string key = "X" + i.ToString().PadLeft(length, '0');
                widgets.TryAdd(key, new Widget() { Id = i, Name = RandomString(2), Price = random.NextDouble() * 100 });

            }
            return true;
        }

        public int AddVat(Double rate)
        {
            Double multiplier = rate + 1;
            int count = 0;
            foreach (var item in widgets)
            {
                Widget newVal = new Widget() { Id = item.Value.Id, Name = item.Value.Name, Price = item.Value.Price * multiplier };

                if (widgets.TryUpdate(item.Key, newVal, item.Value))
                    count++;
            }
            return count;
        }

        public async Task<int> ParralelForVat(Double rate)
        {
            int size = widgets.Count();
            Parallel.For(0, size, _ => AddIndexedSingleVat(_, rate));
            return size;
        }

        public async Task<int> ParralelForOptionsVat(ParallelOptions parallelOptions, Double rate)
        {
            int size = widgets.Count();
            Parallel.For(0, size, parallelOptions, _ => AddIndexedSingleVat(_, rate));
            return size;
        }

        public void AddIndexedSingleVat(int x, Double rate)
        {
            bool changed = false;
            Double multiplier = rate + 1;
            string key = "X" + x.ToString().PadLeft(length, '0');
            Widget Value;
            if (widgets.TryGetValue(key, out Value))
            {
                Widget newVal = new Widget() { Id = Value.Id, Name = Value.Name, Price = Value.Price * multiplier };

                if (widgets.TryUpdate(key, newVal, Value))
                    changed = true;
            }
        }

        public async Task<int> ParralelForEachVat(Double rate)
        {
            Double multiplier = rate + 1;
            int count = widgets.Count();
            Parallel.ForEach<KeyValuePair<string, Widget>>(widgets, _ => AddSingleVat(_, rate));
            return count;
        }

        public async Task<int> ParralelForEachOptionsVat(ParallelOptions parallelOptions, Double rate)
        {
            Double multiplier = rate + 1;
            int count = widgets.Count();
            Parallel.ForEach<KeyValuePair<string, Widget>>(widgets, parallelOptions, _ => AddSingleVat(_, rate));
            return count;
        }
        public async Task<int> ParallelForEachAsyncOptionsVat(ParallelOptions parallelOptions, Double rate)
        {
            Double multiplier = rate + 1;
            int count = widgets.Count();
            await Parallel.ForEachAsync<KeyValuePair<string, Widget>>(widgets, parallelOptions, async (pair, ct) => AddSingleVat(pair, rate));
            return count;
        }
        public void AddSingleVat(KeyValuePair<string, Widget> pair, Double rate)
        {
            Double multiplier = rate + 1;
            bool changed = false;
            Widget newVal = new Widget() { Id = pair.Value.Id, Name = pair.Value.Name, Price = pair.Value.Price * multiplier };

            if (widgets.TryUpdate(pair.Key, newVal, pair.Value))
                changed = true;
        }
        public async Task AddSingleVatAsync(KeyValuePair<string, Widget> pair, Double rate)
        {
            Double multiplier = rate + 1;
            bool changed = false;
            Widget newVal = new Widget() { Id = pair.Value.Id, Name = pair.Value.Name, Price = pair.Value.Price * multiplier };

            if (widgets.TryUpdate(pair.Key, newVal, pair.Value))
                changed = true;
        }
        public void AddVatSpecific(Double rate)
        {
            int size = widgets.Count();
            Double multiplier = rate + 1;
            for (int i = 0; i < size; ++i)

            {
                string key = "X" + i.ToString().PadLeft(length, '0');
                Widget val, newVal;
                if (widgets.TryGetValue(key, out val))
                {
                    newVal = new Widget() { Id = val.Id, Name = val.Name, Price = val.Price * multiplier };
                    widgets.TryUpdate(key, newVal, val);
                }
            }

        }
        private Random random = new Random();

        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
