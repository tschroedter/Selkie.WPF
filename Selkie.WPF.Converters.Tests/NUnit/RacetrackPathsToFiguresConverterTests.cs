using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Media;
using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Interfaces;
using Selkie.WPF.Converters.Interfaces;

namespace Selkie.WPF.Converters.Tests.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class RacetrackPathsToFiguresConverterTests
    {
        [SetUp]
        public void Setup()
        {
            m_ConverterFigures = Substitute.For <IRacetrackPathToFiguresConverter>();
            m_ConverterFigures.FiguresCollection.Returns(new PathFigureCollection());

            m_Converter = new RacetrackPathsToFiguresConverter(m_ConverterFigures);
        }

        private IRacetrackPathToFiguresConverter m_ConverterFigures;
        private RacetrackPathsToFiguresConverter m_Converter;

        [Test]
        public void ConvertAddsToFiguresTest()
        {
            var pathFigureCollection = new PathFigureCollection();
            m_ConverterFigures.FiguresCollection.Returns(pathFigureCollection);

            m_Converter.Paths = new[]
                                {
                                    Substitute.For <IPath>()
                                };
            m_Converter.Convert();

            PathFigureCollection actual = m_Converter.Figures.First();

            Assert.AreEqual(pathFigureCollection,
                            actual);
        }

        [Test]
        public void ConvertCallsConvertTest()
        {
            m_Converter.Paths = new[]
                                {
                                    Substitute.For <IPath>()
                                };
            m_Converter.Convert();

            m_ConverterFigures.Received().Convert();
        }

        [Test]
        public void ConvertSetsPathTest()
        {
            var path = Substitute.For <IPath>();
            var paths = new[]
                        {
                            path
                        };

            m_Converter.Paths = paths;

            m_Converter.Convert();

            Assert.AreEqual(path,
                            m_ConverterFigures.Path);
        }

        [Test]
        public void ConvertUpdatesFiguresTest()
        {
            var pathFigureCollection = new PathFigureCollection();
            m_ConverterFigures.FiguresCollection.Returns(pathFigureCollection);

            m_Converter.Paths = new[]
                                {
                                    Substitute.For <IPath>()
                                };
            m_Converter.Convert();

            IEnumerable <PathFigureCollection> actual = m_Converter.Figures;

            Assert.AreEqual(1,
                            actual.Count());
        }

        [Test]
        public void DefaultFiguresTest()
        {
            Assert.NotNull(m_Converter.Figures);
        }

        [Test]
        public void PathsRoundtripTest()
        {
            var paths = new[]
                        {
                            Substitute.For <IPath>()
                        };

            m_Converter.Paths = new IPath[]
                                {
                                };
            m_Converter.Paths = paths;

            Assert.AreEqual(paths,
                            m_Converter.Paths);
        }

        [Test]
        public void PathsTest()
        {
            var paths = new IPath[]
                        {
                        };

            m_Converter.Paths = paths;

            Assert.AreEqual(paths,
                            m_Converter.Paths);
        }
    }
}