﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.IO;

namespace Prometheus.Tests
{
    [TestClass]
    public sealed class CollectorRegistryTests
    {
        [TestMethod]
        public void ExportAsText_ExportsExpectedData()
        {
            var registry = Metrics.NewCustomRegistry();
            var factory = Metrics.WithCustomRegistry(registry);

            const string canary = "sb64v77";
            const double canaryValue = 64835.83;

            var gauge = factory.CreateGauge(canary, "");
            gauge.Set(canaryValue);

            var stream = new MemoryStream();
            registry.CollectAndExportAsText(stream);

            stream.Position = 0;
            var text = new StreamReader(stream).ReadToEnd();

            StringAssert.Contains(text, canary);
            StringAssert.Contains(text, canaryValue.ToString(CultureInfo.InvariantCulture));
        }
    }
}
