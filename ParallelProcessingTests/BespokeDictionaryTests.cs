using System.Collections.Concurrent;
using ParallelProcessing;
using Xunit;

namespace ParallelProcessingTests
{
    public class BespokeDictionaryTests
    {

        [Fact]
        public void MakeWidgets_Creates_Correct_Number()
        {
            var dict = new BespokeDictionary();
            var result = dict.MakeWidgets(100);
            Assert.True(result);
            Assert.Equal(100, dict.widgets.Count);
            Assert.True(dict.widgets.ContainsKey("X0000000099"));
        }

        [Fact]
        public void MakeWidgets_Zero_Size()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(0);
            Assert.Empty(dict.widgets);
        }

        [Fact]
        public void MakeWidgets_Negative_Size_Produces_Zero()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(-5);
            Assert.NotNull(dict.widgets);
            Assert.Empty(dict.widgets);
        }

        [Fact]
        public void AddVat_Updates_Prices()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(10);
            var firstKey = dict.widgets.Keys.First();
            var before = dict.widgets[firstKey].Price;
            int changed = dict.AddVat(0.1);
            Assert.Equal(10, changed);
            var after = dict.widgets[firstKey].Price;
            Assert.Equal(Math.Round(before * 1.1, 10), Math.Round(after, 10));
        }

        [Fact]
        public void AddVat_ZeroRate_NoChange()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(5);
            var key = dict.widgets.Keys.First();
            var before = dict.widgets[key].Price;
            int changed = dict.AddVat(0.0);
            Assert.Equal(5, changed);
            var after = dict.widgets[key].Price;
            Assert.Equal(Math.Round(before, 10), Math.Round(after, 10));
        }

        [Fact]
        public void AddVat_NegativeRate_Decreases_Prices()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(6);
            var key = dict.widgets.Keys.First();
            var before = dict.widgets[key].Price;
            int changed = dict.AddVat(-0.5);
            Assert.Equal(6, changed);
            var after = dict.widgets[key].Price;
            Assert.Equal(Math.Round(before * 0.5, 10), Math.Round(after, 10));
        }

        [Fact]
        public void AddVat_On_Empty_Dictionary_Returns_Zero()
        {
            var dict = new BespokeDictionary();
            dict.widgets = new ConcurrentDictionary<string, Widget>();
            int changed = dict.AddVat(0.3);
            Assert.Equal(0, changed);
        }

        [Fact]
        public void AddIndexedSingleVat_Works_For_Valid_Index()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(5);
            var key = "X0000000002";
            var before = dict.widgets[key].Price;
            dict.AddIndexedSingleVat(2, 0.5);
            var after = dict.widgets[key].Price;
            Assert.Equal(Math.Round(before * 1.5, 10), Math.Round(after, 10));
        }

        [Fact]
        public void AddIndexedSingleVat_No_Throw_For_Invalid_Index()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(3);
            // should not throw
            dict.AddIndexedSingleVat(10, 0.2);
        }

        [Fact]
        public void AddIndexedSingleVat_No_Throw_For_Negative_Index()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(3);
            var ex = Record.Exception(() => dict.AddIndexedSingleVat(-1, 0.2));
            Assert.Null(ex);
        }

        [Fact]
        public async Task ParralelForVat_Returns_Size()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(20);
            int returned = await Task.Run(() => dict.ParralelForVat(0.1));
            Assert.Equal(20, returned);
        }

        [Fact]
        public async Task ParralelForVat_Updates_Prices()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(8);
            var key = dict.widgets.Keys.First();
            var before = dict.widgets[key].Price;
            await Task.Run(() => dict.ParralelForVat(0.2));
            var after = dict.widgets[key].Price;
            Assert.Equal(Math.Round(before * 1.2, 10), Math.Round(after, 10));
        }

        [Fact]
        public async Task ParralelForVat_On_Empty_Dictionary_Returns_Zero()
        {
            var dict = new BespokeDictionary();
            dict.widgets = new ConcurrentDictionary<string, Widget>();
            int returned = await dict.ParralelForVat(0.1);
            Assert.Equal(0, returned);
        }

        [Fact]
        public async Task ParralelForOptionsVat_Returns_Size()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(12);
            var options = new ParallelOptions { MaxDegreeOfParallelism = 2 };
            int returned = await dict.ParralelForOptionsVat(options, 0.1);
            Assert.Equal(12, returned);
        }

        [Fact]
        public async Task ParralelForEachVat_Returns_Size()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(15);
            int returned = await dict.ParralelForEachVat(0.2);
            Assert.Equal(15, returned);
        }

        [Fact]
        public async Task ParralelForEachVat_Updates_Prices()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(5);
            var key = dict.widgets.Keys.First();
            var before = dict.widgets[key].Price;
            await dict.ParralelForEachVat(0.3);
            var after = dict.widgets[key].Price;
            Assert.Equal(Math.Round(before * 1.3, 10), Math.Round(after, 10));
        }

        [Fact]
        public async Task ParralelForEachOptionsVat_Returns_Size()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(7);
            var options = new ParallelOptions { MaxDegreeOfParallelism = 3 };
            int returned = await dict.ParralelForEachOptionsVat(options, 0.2);
            Assert.Equal(7, returned);
        }

        [Fact]
        public void AddSingleVat_Updates_Pair()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(4);
            var pair = dict.widgets.First();
            var before = pair.Value.Price;
            dict.AddSingleVat(pair, 0.25);
            Assert.Equal(Math.Round(before * 1.25, 10), Math.Round(dict.widgets[pair.Key].Price, 10));
        }

        [Fact]
        public void AddSingleVat_No_Throw_For_Missing_Pair()
        {
            var dict = new BespokeDictionary();
            dict.widgets = new ConcurrentDictionary<string, Widget>();
            var missingPair = new KeyValuePair<string, Widget>("NoKey", new Widget { Id = 1, Name = "A", Price = 1.0 });
            var ex = Record.Exception(() => dict.AddSingleVat(missingPair, 0.1));
            Assert.Null(ex);
        }

        [Fact]
        public void AddSingleVat_Null_Value_Throws_NullReferenceException()
        {
            var dict = new BespokeDictionary();
            dict.widgets = new ConcurrentDictionary<string, Widget>();
            // insert a key with null value to simulate bad data
            dict.widgets.TryAdd("X0000000000", null);
            var pair = new KeyValuePair<string, Widget>("X0000000000", null);
            var ex = Record.Exception(() => dict.AddSingleVat(pair, 0.1));
            Assert.NotNull(ex);
            Assert.IsType<NullReferenceException>(ex);
        }

        [Fact]
        public void AddVatSpecific_Works()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(6);
            var before = dict.widgets["X0000000003"].Price;
            dict.AddVatSpecific(0.5);
            Assert.Equal(Math.Round(before * 1.5, 10), Math.Round(dict.widgets["X0000000003"].Price, 10));
        }

        [Fact]
        public void RandomString_Is_Correct_Length()
        {
            var dict = new BespokeDictionary();
            var method = dict.GetType().GetMethod("RandomString", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (string)method.Invoke(dict, new object[] { 5 });
            Assert.Equal(5, result?.Length);
        }
        [Fact]
        public async Task ParallelForEachAsyncOptionsVat_WithSingleWidget_ReturnsCount()
        {
            // Arrange
            var bespokeDictionary = new BespokeDictionary();
            bespokeDictionary.widgets = new ConcurrentDictionary<string, Widget>
            {
                ["key1"] = new Widget { Id = 1, Name = "Widget1", Price = 100.0 }
            };
            var parallelOptions = new ParallelOptions();
            double rate = 0.2; // 20% VAT

            // Act
            int result = await bespokeDictionary.ParallelForEachAsyncOptionsVat(parallelOptions, rate);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task ParallelForEachAsyncOptionsVat_WithMultipleWidgets_ReturnsCorrectCount()
        {
            // Arrange
            var bespokeDictionary = new BespokeDictionary();
            bespokeDictionary.widgets = new ConcurrentDictionary<string, Widget>
            {
                ["key1"] = new Widget { Id = 1, Name = "Widget1", Price = 100.0 },
                ["key2"] = new Widget { Id = 2, Name = "Widget2", Price = 50.0 },
                ["key3"] = new Widget { Id = 3, Name = "Widget3", Price = 75.0 }
            };
            var parallelOptions = new ParallelOptions();
            double rate = 0.2;

            // Act
            int result = await bespokeDictionary.ParallelForEachAsyncOptionsVat(parallelOptions, rate);

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
        public async Task ParallelForEachAsyncOptionsVat_WithEmptyWidgets_ReturnsZero()
        {
            // Arrange
            var bespokeDictionary = new BespokeDictionary();
            bespokeDictionary.widgets = new ConcurrentDictionary<string, Widget>();
            var parallelOptions = new ParallelOptions();
            double rate = 0.2;

            // Act
            int result = await bespokeDictionary.ParallelForEachAsyncOptionsVat(parallelOptions, rate);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task ParallelForEachAsyncOptionsVat_UpdatesWidgetPricesCorrectly()
        {
            // Arrange
            var bespokeDictionary = new BespokeDictionary();
            var originalPrice = 100.0;
            var originalWidget = new Widget { Id = 1, Name = "Widget1", Price = originalPrice };
            bespokeDictionary.widgets = new ConcurrentDictionary<string, Widget>
            {
                ["key1"] = originalWidget
            };
            var parallelOptions = new ParallelOptions();
            double rate = 0.2; // 20% VAT, so multiplier = 1.2

            // Act
            await bespokeDictionary.ParallelForEachAsyncOptionsVat(parallelOptions, rate);

            // Assert
            var updatedWidget = bespokeDictionary.widgets["key1"];
            Assert.Equal(originalPrice * 1.2, updatedWidget.Price, precision: 2);
            Assert.Equal("Widget1", updatedWidget.Name);
            Assert.Equal(1, updatedWidget.Id);
        }

        [Fact]
        public async Task ParallelForEachAsyncOptionsVat_WithDifferentRates_CalculatesMultiplierCorrectly()
        {
            // Arrange
            var bespokeDictionary = new BespokeDictionary();
            var originalPrice = 100.0;
            bespokeDictionary.widgets = new ConcurrentDictionary<string, Widget>
            {
                ["key1"] = new Widget { Id = 1, Name = "Widget1", Price = originalPrice }
            };
            var parallelOptions = new ParallelOptions();
            double rate = 0.5; // 50% VAT, multiplier = 1.5

            // Act
            await bespokeDictionary.ParallelForEachAsyncOptionsVat(parallelOptions, rate);

            // Assert
            var updatedWidget = bespokeDictionary.widgets["key1"];
            Assert.Equal(150.0, updatedWidget.Price, precision: 2);
        }

        [Fact]
        public async Task ParallelForEachAsyncOptionsVat_WithZeroRate_PreservesOriginalPrice()
        {
            // Arrange
            var bespokeDictionary = new BespokeDictionary();
            var originalPrice = 100.0;
            bespokeDictionary.widgets = new ConcurrentDictionary<string, Widget>
            {
                ["key1"] = new Widget { Id = 1, Name = "Widget1", Price = originalPrice }
            };
            var parallelOptions = new ParallelOptions();
            double rate = 0.0;

            // Act
            await bespokeDictionary.ParallelForEachAsyncOptionsVat(parallelOptions, rate);

            // Assert
            var updatedWidget = bespokeDictionary.widgets["key1"];
            Assert.Equal(originalPrice, updatedWidget.Price, precision: 2);
        }

        [Fact]
        public async Task ParallelForEachAsyncOptionsVat_WithParallelismLimit_StillUpdatesAllWidgets()
        {
            // Arrange
            var bespokeDictionary = new BespokeDictionary();
            bespokeDictionary.widgets = new ConcurrentDictionary<string, Widget>
            {
                ["key1"] = new Widget { Id = 1, Name = "Widget1", Price = 100.0 },
                ["key2"] = new Widget { Id = 2, Name = "Widget2", Price = 50.0 },
                ["key3"] = new Widget { Id = 3, Name = "Widget3", Price = 75.0 }
            };
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 1 };
            double rate = 0.2;

            // Act
            int result = await bespokeDictionary.ParallelForEachAsyncOptionsVat(parallelOptions, rate);

            // Assert
            Assert.Equal(3, result);
            Assert.Equal(120.0, bespokeDictionary.widgets["key1"].Price, precision: 2);
            Assert.Equal(60.0, bespokeDictionary.widgets["key2"].Price, precision: 2);
            Assert.Equal(90.0, bespokeDictionary.widgets["key3"].Price, precision: 2);
        }

        [Fact]
        public async Task ParallelForEachAsyncOptionsVat_WithNegativeRate_CalculatesCorrectly()
        {
            // Arrange
            var bespokeDictionary = new BespokeDictionary();
            var originalPrice = 100.0;
            bespokeDictionary.widgets = new ConcurrentDictionary<string, Widget>
            {
                ["key1"] = new Widget { Id = 1, Name = "Widget1", Price = originalPrice }
            };
            var parallelOptions = new ParallelOptions();
            double rate = -0.2; // -20% discount, multiplier = 0.8

            // Act
            await bespokeDictionary.ParallelForEachAsyncOptionsVat(parallelOptions, rate);

            // Assert
            var updatedWidget = bespokeDictionary.widgets["key1"];
            Assert.Equal(80.0, updatedWidget.Price, precision: 2);
        }

        [Fact]
        public async Task AddSingleVatAsync_WithSingleWidget_UpdatesPrice()
        {
            // Arrange
            var bespokeDictionary = new BespokeDictionary();
            var originalPrice = 100.0;
            var originalWidget = new Widget { Id = 1, Name = "Widget1", Price = originalPrice };
            bespokeDictionary.widgets = new ConcurrentDictionary<string, Widget>
            {
                ["key1"] = originalWidget
            };
            var pair = new KeyValuePair<string, Widget>("key1", originalWidget);
            double rate = 0.2; // 20% VAT, multiplier = 1.2

            // Act
            await bespokeDictionary.AddSingleVatAsync(pair, rate);

            // Assert
            var updatedWidget = bespokeDictionary.widgets["key1"];
            Assert.Equal(originalPrice * 1.2, updatedWidget.Price, precision: 2);
            Assert.Equal("Widget1", updatedWidget.Name);
            Assert.Equal(1, updatedWidget.Id);
        }

        [Fact]
        public async Task AddSingleVatAsync_WithDifferentRates_CalculatesMultiplierCorrectly()
        {
            // Arrange
            var bespokeDictionary = new BespokeDictionary();
            var originalPrice = 100.0;
            var widget = new Widget { Id = 1, Name = "Widget1", Price = originalPrice };
            bespokeDictionary.widgets = new ConcurrentDictionary<string, Widget>
            {
                ["key1"] = widget
            };
            var pair = new KeyValuePair<string, Widget>("key1", widget);
            double rate = 0.5; // 50% VAT, multiplier = 1.5

            // Act
            await bespokeDictionary.AddSingleVatAsync(pair, rate);

            // Assert
            var updatedWidget = bespokeDictionary.widgets["key1"];
            Assert.Equal(150.0, updatedWidget.Price, precision: 2);
        }

        [Fact]
        public async Task AddSingleVatAsync_WithZeroRate_PreservesOriginalPrice()
        {
            // Arrange
            var bespokeDictionary = new BespokeDictionary();
            var originalPrice = 100.0;
            var widget = new Widget { Id = 1, Name = "Widget1", Price = originalPrice };
            bespokeDictionary.widgets = new ConcurrentDictionary<string, Widget>
            {
                ["key1"] = widget
            };
            var pair = new KeyValuePair<string, Widget>("key1", widget);
            double rate = 0.0;

            // Act
            await bespokeDictionary.AddSingleVatAsync(pair, rate);

            // Assert
            var updatedWidget = bespokeDictionary.widgets["key1"];
            Assert.Equal(originalPrice, updatedWidget.Price, precision: 2);
        }

        [Fact]
        public async Task AddSingleVatAsync_WithNegativeRate_CalculatesCorrectly()
        {
            // Arrange
            var bespokeDictionary = new BespokeDictionary();
            var originalPrice = 100.0;
            var widget = new Widget { Id = 1, Name = "Widget1", Price = originalPrice };
            bespokeDictionary.widgets = new ConcurrentDictionary<string, Widget>
            {
                ["key1"] = widget
            };
            var pair = new KeyValuePair<string, Widget>("key1", widget);
            double rate = -0.2; // -20% discount, multiplier = 0.8

            // Act
            await bespokeDictionary.AddSingleVatAsync(pair, rate);

            // Assert
            var updatedWidget = bespokeDictionary.widgets["key1"];
            Assert.Equal(80.0, updatedWidget.Price, precision: 2);
        }

        [Fact]
        public async Task AddSingleVatAsync_PreservesWidgetIdAndName()
        {
            // Arrange
            var bespokeDictionary = new BespokeDictionary();
            var widget = new Widget { Id = 42, Name = "TestWidget", Price = 100.0 };
            bespokeDictionary.widgets = new ConcurrentDictionary<string, Widget>
            {
                ["key1"] = widget
            };
            var pair = new KeyValuePair<string, Widget>("key1", widget);
            double rate = 0.2;

            // Act
            await bespokeDictionary.AddSingleVatAsync(pair, rate);

            // Assert
            var updatedWidget = bespokeDictionary.widgets["key1"];
            Assert.Equal(42, updatedWidget.Id);
            Assert.Equal("TestWidget", updatedWidget.Name);
        }

        [Fact]
        public async Task AddSingleVatAsync_WithZeroPrice_UpdatesCorrectly()
        {
            // Arrange
            var bespokeDictionary = new BespokeDictionary();
            var widget = new Widget { Id = 1, Name = "Widget1", Price = 0.0 };
            bespokeDictionary.widgets = new ConcurrentDictionary<string, Widget>
            {
                ["key1"] = widget
            };
            var pair = new KeyValuePair<string, Widget>("key1", widget);
            double rate = 0.2;

            // Act
            await bespokeDictionary.AddSingleVatAsync(pair, rate);

            // Assert
            var updatedWidget = bespokeDictionary.widgets["key1"];
            Assert.Equal(0.0, updatedWidget.Price, precision: 2);
        }

        [Fact]
        public async Task AddSingleVatAsync_WithLargeRate_CalculatesCorrectly()
        {
            // Arrange
            var bespokeDictionary = new BespokeDictionary();
            var originalPrice = 100.0;
            var widget = new Widget { Id = 1, Name = "Widget1", Price = originalPrice };
            bespokeDictionary.widgets = new ConcurrentDictionary<string, Widget>
            {
                ["key1"] = widget
            };
            var pair = new KeyValuePair<string, Widget>("key1", widget);
            double rate = 10.0; // 1000% VAT, multiplier = 11.0

            // Act
            await bespokeDictionary.AddSingleVatAsync(pair, rate);

            // Assert
            var updatedWidget = bespokeDictionary.widgets["key1"];
            Assert.Equal(1100.0, updatedWidget.Price, precision: 2);
        }

        [Fact]
        public async Task ParallelForEachAsyncOptionsVat_WithCancellation_StillProcesses()
        {
            // Arrange
            var bespokeDictionary = new BespokeDictionary();
            bespokeDictionary.widgets = new ConcurrentDictionary<string, Widget>
            {
                ["key1"] = new Widget { Id = 1, Name = "Widget1", Price = 100.0 },
                ["key2"] = new Widget { Id = 2, Name = "Widget2", Price = 50.0 }
            };
            var cts = new CancellationTokenSource();
            var parallelOptions = new ParallelOptions { CancellationToken = cts.Token };
            double rate = 0.2;

            // Act
            int result = await bespokeDictionary.ParallelForEachAsyncOptionsVat(parallelOptions, rate);

            // Assert
            Assert.Equal(2, result);
            // At least some widgets should have been processed
            var key1Price = bespokeDictionary.widgets["key1"].Price;
            var key2Price = bespokeDictionary.widgets["key2"].Price;
            Assert.True(key1Price > 0);
            Assert.True(key2Price > 0);
        }

        [Fact]
        public async Task AddSingleVatAsync_WithVerySmallRate_CalculatesCorrectly()
        {
            // Arrange
            var bespokeDictionary = new BespokeDictionary();
            var originalPrice = 100.0;
            var widget = new Widget { Id = 1, Name = "Widget1", Price = originalPrice };
            bespokeDictionary.widgets = new ConcurrentDictionary<string, Widget>
            {
                ["key1"] = widget
            };
            var pair = new KeyValuePair<string, Widget>("key1", widget);
            double rate = 0.001; // 0.1% VAT, multiplier = 1.001

            // Act
            await bespokeDictionary.AddSingleVatAsync(pair, rate);

            // Assert
            var updatedWidget = bespokeDictionary.widgets["key1"];
            Assert.Equal(100.1, updatedWidget.Price, precision: 1);
        }

        [Fact]
        public async Task ParallelForEachAsyncOptionsVat_MultipleCalls_CumulativelyUpdatePrices()
        {
            // Arrange
            var bespokeDictionary = new BespokeDictionary();
            var widget = new Widget { Id = 1, Name = "Widget1", Price = 100.0 };
            bespokeDictionary.widgets = new ConcurrentDictionary<string, Widget>
            {
                ["key1"] = widget
            };
            var parallelOptions = new ParallelOptions();
            double rate = 0.1; // 10% VAT

            // Act - First call
            await bespokeDictionary.ParallelForEachAsyncOptionsVat(parallelOptions, rate);
            var priceAfterFirstCall = bespokeDictionary.widgets["key1"].Price;

            // Act - Second call
            await bespokeDictionary.ParallelForEachAsyncOptionsVat(parallelOptions, rate);
            var priceAfterSecondCall = bespokeDictionary.widgets["key1"].Price;

            // Assert
            Assert.Equal(110.0, priceAfterFirstCall, precision: 2);
            Assert.Equal(121.0, priceAfterSecondCall, precision: 2);
        }
    }
}
