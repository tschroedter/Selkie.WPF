using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Media;
using Castle.Core.Logging;
using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Common;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Converters.Interfaces;

namespace Selkie.WPF.Converters.Tests.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class RacetrackPathToFiguresConverterTests
    {
        [SetUp]
        public void Setup()
        {
            m_Logger = Substitute.For <ILogger>();

            m_StartSegment = Substitute.For <ITurnCircleArcSegment>();
            m_MiddleSegment = Substitute.For <ILine>();
            m_EndSegment = Substitute.For <ITurnCircleArcSegment>();

            m_Path = Substitute.For <IPath>();
            var segments = new List <IPolylineSegment>
                           {
                               m_StartSegment,
                               m_MiddleSegment,
                               m_EndSegment
                           };

            m_Path.Segments.Returns(segments);

            m_UTurnPath = Substitute.For <IPath>();
            var segmentsUTurn = new List <IPolylineSegment>
                                {
                                    m_StartSegment,
                                    Substitute.For <ITurnCircleArcSegment>(),
                                    m_EndSegment
                                };

            m_UTurnPath.Segments.Returns(segmentsUTurn);

            m_NormalFigureCollection = new PathFigureCollection();
            m_NormalConverter = Substitute.For <IRacetrackPathTurnToFiguresConverter>();
            m_NormalConverter.FiguresCollection.Returns(m_NormalFigureCollection);

            m_UTurnFigureCollection = new PathFigureCollection();
            m_UTurnConverter = Substitute.For <IRacetrackPathUTurnToFiguresConverter>();
            m_UTurnConverter.FiguresCollection.Returns(m_UTurnFigureCollection);

            m_Converter = new RacetrackPathToFiguresConverter(m_NormalConverter,
                                                              m_UTurnConverter)
                          {
                              Logger = m_Logger
                          };
        }

        private RacetrackPathToFiguresConverter m_Converter;
        private ITurnCircleArcSegment m_StartSegment;
        private ILine m_MiddleSegment;
        private ITurnCircleArcSegment m_EndSegment;
        private IPath m_Path;
        private IRacetrackPathTurnToFiguresConverter m_NormalConverter;
        private IRacetrackPathUTurnToFiguresConverter m_UTurnConverter;
        private IPath m_UTurnPath;
        private PathFigureCollection m_NormalFigureCollection;
        private PathFigureCollection m_UTurnFigureCollection;
        private ILogger m_Logger;

        [Test]
        public void ConvertForUnknownPathTest()
        {
            m_Converter.Path = Path.Unknown;

            m_Converter.Convert();

            Assert.AreEqual(0,
                            m_Converter.FiguresCollection.Count());
        }

        [Test]
        public void ConvertForWrongNumberOfSegmentsTest()
        {
            var segments = new List <IPolylineSegment>
                           {
                               m_StartSegment,
                               m_MiddleSegment
                           };
            var path = Substitute.For <IPath>();
            path.Segments.Returns(segments);

            m_Converter.Path = path;

            m_Converter.Convert();

            Assert.AreEqual(0,
                            m_Converter.FiguresCollection.Count());
        }

        [Test]
        public void ConvertLogsForUnknownPathTest()
        {
            var path = Substitute.For <IPath>();
            var segments = new List <IPolylineSegment>
                           {
                               m_StartSegment,
                               Substitute.For <IArcSegment>(),
                               m_EndSegment
                           };

            path.Segments.Returns(segments);

            m_Converter.Path = path;

            m_Converter.Convert();

            m_Logger.Received().Error(Arg.Any <string>());

            Assert.AreEqual(0,
                            m_Converter.FiguresCollection.Count,
                            "Count");
        }

        [Test]
        public void ConvertReturnsFiguresForUTurnTest()
        {
            m_Converter.Path = m_UTurnPath;

            m_Converter.Convert();

            Assert.AreEqual(m_UTurnFigureCollection,
                            m_Converter.FiguresCollection);
        }

        [Test]
        public void ConvertReturnsFiguresTest()
        {
            m_Converter.Path = m_Path;

            m_Converter.Convert();

            Assert.AreEqual(m_NormalFigureCollection,
                            m_Converter.FiguresCollection);
        }

        [Test]
        public void ConvertTurnPathTest()
        {
            PathFigureCollection actual = m_Converter.ConvertTurnPath(m_Path);

            m_NormalConverter.Received().Convert();
            Assert.AreEqual(m_Path,
                            m_NormalConverter.Path,
                            "Path");
            Assert.AreEqual(m_UTurnFigureCollection,
                            actual,
                            "Collection");
        }

        [Test]
        public void ConvertUTurnPathTest()
        {
            PathFigureCollection actual = m_Converter.ConvertUTurnPath(m_UTurnPath);

            m_UTurnConverter.Received().Convert();
            Assert.AreEqual(m_UTurnPath,
                            m_UTurnConverter.Path,
                            "Path");
            Assert.AreEqual(m_UTurnFigureCollection,
                            actual,
                            "Collection");
        }

        [Test]
        public void FiguresDefaultTest()
        {
            Assert.IsNotNull(m_Converter.FiguresCollection);
            Assert.AreEqual(0,
                            m_Converter.FiguresCollection.Count,
                            "Count");
        }

        [Test]
        public void IsNormalTurnReturnFalseForForOtherTest()
        {
            Assert.False(m_Converter.IsNormalTurn(Substitute.For <ITurnCircleArcSegment>()));
        }

        [Test]
        public void IsNormalTurnReturnTrueForForLineTest()
        {
            Assert.True(m_Converter.IsNormalTurn(Substitute.For <ILine>()));
        }

        [Test]
        public void IsUTurnReturnFalseForForOtherTest()
        {
            Assert.False(m_Converter.IsUTurn(Substitute.For <ILine>()));
        }

        [Test]
        public void IsUTurnReturnTrueForForArcSegmentTest()
        {
            Assert.True(m_Converter.IsUTurn(Substitute.For <ITurnCircleArcSegment>()));
        }

        [Test]
        public void PathDefaultTest()
        {
            Assert.AreEqual(Path.Unknown,
                            m_Converter.Path);
        }

        [Test]
        public void PathRoundtripTest()
        {
            var other = Substitute.For <IPath>();

            m_Converter.Path = other;

            Assert.AreEqual(other,
                            m_Converter.Path);
        }
    }
}