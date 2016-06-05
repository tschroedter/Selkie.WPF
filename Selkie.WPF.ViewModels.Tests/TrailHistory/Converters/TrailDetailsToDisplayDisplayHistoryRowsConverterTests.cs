using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Selkie.Common.Interfaces;
using Selkie.WPF.Models.Interfaces;
using Selkie.WPF.ViewModels.Interfaces;
using Selkie.WPF.ViewModels.TrailHistory;
using Selkie.WPF.ViewModels.TrailHistory.Converters;

namespace Selkie.WPF.ViewModels.Tests.TrailHistory.Converters
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class TrailDetailsToDisplayDisplayHistoryRowsConverterTests
    {
        [SetUp]
        public void Setup()
        {
            m_Disposer = Substitute.For <IDisposer>();
            m_Factory = Substitute.For <IDisplayHistoryRowFactory>();

            m_Converter = new TrailDetailsToDisplayDisplayHistoryRowsConverter(m_Disposer,
                                                                               m_Factory);
        }

        [TearDown]
        public void Teardown()
        {
            m_Converter.Dispose();
        }

        private IDisplayHistoryRowFactory m_Factory;
        private TrailDetailsToDisplayDisplayHistoryRowsConverter m_Converter;
        private IDisposer m_Disposer;

        private static IEnumerable <ITrailDetails> CreateTrailDetails()
        {
            return new[]
                   {
                       Substitute.For <ITrailDetails>(),
                       Substitute.For <ITrailDetails>()
                   };
        }

        [Test]
        public void AddDisplayHistoryRowsAddsRowsTest()
        {
            m_Converter.Trails = CreateTrailDetails();

            m_Converter.AddDisplayHistoryRows();

            IEnumerable <IDisplayHistoryRow> actual = m_Converter.DisplayHistoryRows;

            Assert.AreEqual(2,
                            actual.Count());
        }

        [Test]
        public void ConstructorAddsToDisposerTest()
        {
            m_Disposer.Received().AddResource(m_Converter.ReleaseDisplayHistoryRows);
        }

        [Test]
        public void ConvertCallsAddDisplayHistoryRowsTest()
        {
            m_Converter.Trails = CreateTrailDetails();

            m_Converter.Convert();

            IEnumerable <IDisplayHistoryRow> actual = m_Converter.DisplayHistoryRows;

            Assert.AreEqual(2,
                            actual.Count());
        }

        [Test]
        public void ConvertCallsReleaseDisplayHistoryRowsTest()
        {
            m_Converter.Trails = CreateTrailDetails();
            m_Converter.Convert();

            m_Converter.Trails = new[]
                                 {
                                     Substitute.For <ITrailDetails>()
                                 };

            m_Converter.Convert();

            m_Factory.Received(2).Release(Arg.Any <IDisplayHistoryRow>());
        }

        [Test]
        public void CreateDisplayHistoryRowCallsCreateTest()
        {
            m_Converter.CreateDisplayHistoryRow(Substitute.For <ITrailDetails>());

            m_Factory.ReceivedWithAnyArgs().Create(0,
                                                   new int[]
                                                   {
                                                   },
                                                   0.0,
                                                   0.0,
                                                   0.0,
                                                   0.0,
                                                   0.0,
                                                   0.0,
                                                   "Type");
        }

        [Test]
        public void CreateDisplayHistoryRowReturnsRowTest()
        {
            IDisplayHistoryRow actual = m_Converter.CreateDisplayHistoryRow(Substitute.For <ITrailDetails>());

            Assert.NotNull(actual);
        }

        [Test]
        public void DefaultDisplayHistoryRowsTest()
        {
            Assert.NotNull(m_Converter.DisplayHistoryRows);
        }

        [Test]
        public void DefaultTrailsTest()
        {
            Assert.NotNull(m_Converter.Trails);
        }

        [Test]
        public void DisposeCallsDisposeTest()
        {
            var disposer = Substitute.For <IDisposer>();

            var converter = new TrailDetailsToDisplayDisplayHistoryRowsConverter(disposer,
                                                                                 m_Factory);

            converter.Dispose();

            disposer.Received().Dispose();
        }

        [Test]
        public void ReleaseDisplayHistoryRowsCallsReleaseTest()
        {
            m_Converter.Trails = CreateTrailDetails();
            m_Converter.Convert();

            m_Converter.ReleaseDisplayHistoryRows();

            m_Factory.Received(2).Release(Arg.Any <IDisplayHistoryRow>());
        }
    }
}