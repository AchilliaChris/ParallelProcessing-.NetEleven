using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace ParallelProcessing
{
    class Program
    {

        private const int length = 10;
        private const int size = 30000000;

        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) => { })
            .ConfigureServices((context, services) =>
            {
                // Use Startup to configure services
                var startup = new Startup(context.Configuration);
                startup.ConfigureServices(services);
            })
            .Build();

            // Resolve IBespokeDictionary from DI and run
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dictionary = services.GetRequiredService<IBespokeDictionary>();
                await Run(args, dictionary);
            }

            // Optionally run the host if needed
            await host.RunAsync();
        }

        static async Task Run(string[] args, IBespokeDictionary dictionary)
        {
            // Ensure concrete type is BespokeDictionary for internal access to widgets property
            var concrete = dictionary as BespokeDictionary ?? throw new InvalidOperationException("BespokeDictionary not registered as IBespokeDictionary");

            ParallelOptions parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
            Widget val, postVat, parval, parpostVat, parOptionsval, parpostOptionsVat, postSingleVat, postForeachVat, postForeachAsyncVat, postOptionsSingleVat, postOptionsForeachVat, parpostOptionsAsyncVat;
            string key = "X0000000067";
            Double rate = 0.2;
            DateTime StartTime, FinishTime;
            StartTime = DateTime.Now;
            Console.WriteLine("Starting the process at {0:T}", StartTime.ToString());
            Console.WriteLine();
            Console.WriteLine("Single thread make widgets.");
            var stopWatch = Stopwatch.StartNew();
            await MeasureAsync("Widgets made.", () => Task.Run(() => concrete.MakeWidgets(size)));

            if (concrete.widgets.TryGetValue(key, out val))
                Console.WriteLine("Key: " + key + " Price: " + val.Price);
            else
                Console.WriteLine("Key not found: " + key);

            Console.WriteLine();
            Console.WriteLine("Single thread Add VAT.");
            int count = await MeasureAsync("Vat added using dictionary.AddVat().", () => Task.Run(() => concrete.AddVat(rate)));
            Console.WriteLine("{0}, {1}", "Vat added using dictionary.AddVat(). No of widgets: ", count); // preserved original extra output style
            if (concrete.widgets.TryGetValue(key, out postVat))
                Console.WriteLine("Key: " + key + " Price: " + postVat.Price);
            else
                Console.WriteLine("Key not found after AddVat: " + key);

            Console.WriteLine();
            Console.WriteLine("Adding more vat using Parallel For on single widgets.");
            Measure("Vat added using Parallel For on single widgets dictionary.AddIndexedSingleVat().",
            () => Parallel.For(0, size, _ => concrete.AddIndexedSingleVat(_, rate)));
            if (concrete.widgets.TryGetValue(key, out postSingleVat))
                Console.WriteLine("Key: " + key + " Price: " + postSingleVat.Price);
            else
                Console.WriteLine("Key not found after Parallel.For: " + key);


            Console.WriteLine();
            Console.WriteLine("Adding more vat using Parallel ForEach on single widgets.");
            Measure("Vat added using Parallel ForEach on single widgets dictionary.AddSingleVat().",
            () => Parallel.ForEach<KeyValuePair<string, Widget>>(concrete.widgets, _ => concrete.AddSingleVat(_, rate)));
            if (concrete.widgets.TryGetValue(key, out postForeachVat))
                Console.WriteLine("Key: " + key + " Price: " + postForeachVat.Price);
            else
                Console.WriteLine("Key not found after Parallel.ForEach: " + key);

            Console.WriteLine();
            // Parallel ForEach is clearly faster (great for CPU bound work) but Parallel ForEachAsync is more robust for async operations involving db and api calls.
            Console.WriteLine("Adding more vat using Parallel ForEachAsync on single widgets.");
            await MeasureAsync("Vat added using Parallel ForEachAsync on single widgets dictionary.AddSingleVat().",
            () => Parallel.ForEachAsync<KeyValuePair<string, Widget>>(concrete.widgets, async (pair, ct) => concrete.AddSingleVatAsync(pair, rate)));
            if (concrete.widgets.TryGetValue(key, out postForeachAsyncVat))
                Console.WriteLine("Key: " + key + " Price: " + postForeachAsyncVat.Price);
            else
                Console.WriteLine("Key not found after Parallel.ForEachAsync: " + key);

            Console.WriteLine();
            Console.WriteLine("Adding more vat using Parallel For with Parallel Options on single widgets.");
            Measure("Vat added using Parallel For on single widgets dictionary.AddIndexedSingleVat().",
            () => Parallel.For(0, size, parallelOptions, _ => concrete.AddIndexedSingleVat(_, rate)));
            if (concrete.widgets.TryGetValue(key, out postOptionsSingleVat))
                Console.WriteLine("Key: " + key + " Price: " + postOptionsSingleVat.Price);
            else
                Console.WriteLine("Key not found after Parallel.For: " + key);


            Console.WriteLine();
            Console.WriteLine("Adding more vat using Parallel ForEach on single widgets.");
            Measure("Vat added using Parallel ForEach on single widgets dictionary.AddSingleVat().",
            () => Parallel.ForEach<KeyValuePair<string, Widget>>(concrete.widgets, parallelOptions, _ => concrete.AddSingleVat(_, rate)));
            if (concrete.widgets.TryGetValue(key, out postOptionsForeachVat))
                Console.WriteLine("Key: " + key + " Price: " + postOptionsForeachVat.Price);
            else
                Console.WriteLine("Key not found after Parallel.ForEach: " + key);


            Console.WriteLine();
            count = await MeasureAsync("Vat added using dictionary.ParallelForVat().", () => Task.Run(() => concrete.ParralelForVat(rate)));
            Console.WriteLine("{0}, {1}", "Vat added using dictionary.ParallelForVat(). No of widgets: ", count);
            if (concrete.widgets.TryGetValue(key, out parval))
                Console.WriteLine("Key: " + key + " Price: " + parval.Price);
            else
                Console.WriteLine("Key not found after ParralelForVat: " + key);

            Console.WriteLine();
            count = await MeasureAsync("Vat added using dictionary.ParallelForEachVat().", () => Task.Run(() => concrete.ParralelForEachVat(rate)));
            Console.WriteLine("{0}, {1}", "Vat added using dictionary.ParallelForEachVat(). No of widgets: ", count);
            if (concrete.widgets.TryGetValue(key, out parpostVat))
                Console.WriteLine("Key: " + key + " Price: " + parpostVat.Price);
            else
                Console.WriteLine("Key not found after ParralelForEachVat: " + key);

            Console.WriteLine();
            count = await MeasureAsync("Vat added using dictionary.ParallelForOptionsVat().", () => Task.Run(() => concrete.ParralelForOptionsVat(parallelOptions, rate)));
            Console.WriteLine("{0}, {1}", "Vat added using dictionary.ParallelForOptionsVat(). No of widgets: ", count);
            if (concrete.widgets.TryGetValue(key, out parOptionsval))
                Console.WriteLine("Key: " + key + " Price: " + parOptionsval.Price);
            else
                Console.WriteLine("Key not found after ParralelForOptionsVat: " + key);

            Console.WriteLine();
            count = await MeasureAsync("Vat added using dictionary.ParallelForEachOptionsVat().", () => Task.Run(() => concrete.ParralelForEachOptionsVat(parallelOptions, rate)));
            Console.WriteLine("{0}, {1}", "Vat added using dictionary.ParallelForEachOptionsVat(). No of widgets: ", count);
            if (concrete.widgets.TryGetValue(key, out parpostOptionsVat))
                Console.WriteLine("Key: " + key + " Price: " + parpostOptionsVat.Price);
            else
                Console.WriteLine("Key not found after ParralelForEachOptionsVat: " + key);

            Console.WriteLine();
            // Parallel ForEach is clearly faster (great for CPU bound work) but Parallel ForEachAsync is more robust for async operations involving db and api calls.
            count = await MeasureAsync("Vat added using dictionary.ParallelForEachAsyncOptionsVat().", () => Task.Run(() => concrete.ParallelForEachAsyncOptionsVat(parallelOptions, rate)));
            Console.WriteLine("{0}, {1}", "Vat added using dictionary.ParallelForEachAsyncOptionsVat(). No of widgets: ", count);
            if (concrete.widgets.TryGetValue(key, out parpostOptionsAsyncVat))
                Console.WriteLine("Key: " + key + " Price: " + parpostOptionsAsyncVat.Price);
            else
                Console.WriteLine("Key not found after ParralelForEachOptionsVat: " + key);
            stopWatch.Stop();
            Console.WriteLine("The total elapsed time is: " + stopWatch.Elapsed);
            Console.WriteLine();
            FinishTime = DateTime.Now;
            TimeSpan timeSpan = FinishTime - StartTime;
            Console.WriteLine("The total time taken for the process is {0}", timeSpan.ToString());
            Console.WriteLine();
            Console.WriteLine("Sorry to spoil your fun, but that's all.");

            Console.ReadLine();
        }

        static T Measure<T>(string message, Func<T> func)
        {
            var sw = Stopwatch.StartNew();
            var result = func();
            sw.Stop();
            Console.WriteLine("{0}, {1}", sw.Elapsed, message);
            return result;
        }

        static async Task<T> MeasureAsync<T>(string message, Func<Task<T>> func)
        {
            var sw = Stopwatch.StartNew();
            var result = await func();
            sw.Stop();
            Console.WriteLine("{0}, {1}", sw.Elapsed, message);
            return result;
        }
        static async Task MeasureAsync(string message, Func<Task> func)
        {
            var sw = Stopwatch.StartNew();
            await func();
            sw.Stop();
            Console.WriteLine("{0}, {1}", sw.Elapsed, message);
            return;
        }
    }
}
