using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Common;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Converters.Interfaces;
using ArcSegment = System.Windows.Media.ArcSegment;
using Point = System.Windows.Point;

namespace Selkie.WPF.Converters.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class RacetrackPathTurnToFiguresConverterTests
    {
        [SetUp]
        public void Setup()
        {
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
            var uTurnSegments = new List <IPolylineSegment>
                                {
                                    m_StartSegment,
                                    Substitute.For <ITurnCircleArcSegment>(),
                                    m_EndSegment
                                };

            m_UTurnPath.Segments.Returns(uTurnSegments);

            m_ArcSegmentOne = new ArcSegment(new Point(10.0,
                                                       10.0),
                                             new Size(10.0,
                                                      10.0),
                                             45.0,
                                             false,
                                             SweepDirection.Clockwise,
                                             false);

            m_LineSegment = new LineSegment(new Point(20.0,
                                                      20.0),
                                            false);

            m_ArcSegmentTwo = new ArcSegment(new Point(30.0,
                                                       30.0),
                                             new Size(30.0,
                                                      30.0),
                                             90.0,
                                             false,
                                             SweepDirection.Counterclockwise,
                                             false);

            m_ArcSegmentThree = new ArcSegment(new Point(40.0,
                                                         40.0),
                                               new Size(40.0,
                                                        40.0),
                                               135.0,
                                               false,
                                               SweepDirection.Counterclockwise,
                                               false);


            m_Point = new Point(10.0,
                                10.0);

            m_Helper = Substitute.For <IPathSegmentHelper>();
            m_Helper.SegmentToLineSegment(Line.Unknown).ReturnsForAnyArgs(m_LineSegment);
            m_Helper.SegmentToArcSegment(TurnCircleArcSegment.Unknown).ReturnsForAnyArgs(m_ArcSegmentOne,
                                                                                         m_ArcSegmentTwo,
                                                                                         m_ArcSegmentThree);
            m_Helper.PointRelativeToOrigin(null).ReturnsForAnyArgs(m_Point);

            m_Converter = new RacetrackPathTurnToFiguresConverter(m_Helper);
        }

        private IPathSegmentHelper m_Helper;
        private RacetrackPathTurnToFiguresConverter m_Converter;
        private ArcSegment m_ArcSegmentOne;
        private ArcSegment m_ArcSegmentTwo;
        private LineSegment m_LineSegment;
        private Point m_Point;
        private ITurnCircleArcSegment m_StartSegment;
        private ILine m_MiddleSegment;
        private ITurnCircleArcSegment m_EndSegment;
        private IPath m_Path;
        private ArcSegment m_ArcSegmentThree;
        private IPath m_UTurnPath;

        [Test]
        public void ConvertCollectionCountTest()
        {
            m_Converter.Path = m_Path;

            m_Converter.Convert();

            PathFigureCollection figuresCollection = m_Converter.FiguresCollection;

            Assert.AreEqual(1,
                            figuresCollection.Count);
        }

        [Test]
        public void ConvertFiguresTest()
        {
            m_Converter.Path = m_Path;

            m_Converter.Convert();

            PathFigure pathFigure = m_Converter.FiguresCollection.First();

            PathSegment[] actual = pathFigure.Segments.ToArray();

            Assert.AreEqual(m_ArcSegmentOne,
                            actual [ 0 ],
                            "[0]");
            Assert.AreEqual(m_LineSegment,
                            actual [ 1 ],
                            "[1]");
            Assert.AreEqual(m_ArcSegmentTwo,
                            actual [ 2 ],
                            "[2]");
        }

        [Test]
        public void ConvertForUTurnCollectionCountTest()
        {
            m_Converter.Path = m_UTurnPath;

            m_Converter.Convert();

            PathFigureCollection figuresCollection = m_Converter.FiguresCollection;

            Assert.AreEqual(1,
                            figuresCollection.Count);
        }

        [Test]
        public void ConvertForUTurnFiguresTest()
        {
            m_Converter.Path = m_UTurnPath;

            m_Converter.Convert();

            PathFigure pathFigure = m_Converter.FiguresCollection.First();

            PathSegment[] actual = pathFigure.Segments.ToArray();

            Assert.AreEqual(m_ArcSegmentOne,
                            actual [ 0 ],
                            "[0]");
            Assert.AreEqual(m_ArcSegmentTwo,
                            actual [ 1 ],
                            "[1]");
            Assert.AreEqual(m_ArcSegmentThree,
                            actual [ 2 ],
                            "[2]");
        }

        [Test]
        public void ConvertSegmentForArcSegmentTest()
        {
            var segment = Substitute.For <IArcSegment>();

            Assert.Throws <ArgumentException>(() => m_Converter.ConvertSegment(segment));
        }

        [Test]
        public void ConvertSegmentForLineSegmentTest()
        {
            var segment = Substitute.For <ILine>();

            PathSegment actual = m_Converter.ConvertSegment(segment);

            Assert.AreEqual(m_LineSegment,
                            actual);
        }

        [Test]
        public void ConvertSegmentForTurnCircleArcSegmentTest()
        {
            var segment = Substitute.For <ITurnCircleArcSegment>();

            PathSegment actual = m_Converter.ConvertSegment(segment);

            Assert.AreEqual(m_ArcSegmentOne,
                            actual);
        }

        [Test]
        public void CreateFiguresCollectionCountTest()
        {
            PathFigureCollection collection = m_Converter.CreateFigures(m_Path);

            Assert.AreEqual(1,
                            collection.Count);
        }

        [Test]
        public void CreateFiguresFiguresCountTest()
        {
            PathFigure figures = m_Converter.CreateFigures(m_Path).First();

            Assert.AreEqual(3,
                            figures.Segments.Count);
        }

        [Test]
        public void CreateFiguresSegmentsTest()
        {
            PathFigure pathFigure = m_Converter.CreateFigures(m_Path).First();

            PathSegment[] actual = pathFigure.Segments.ToArray();

            Assert.AreEqual(m_ArcSegmentOne,
                            actual [ 0 ],
                            "[0]");
            Assert.AreEqual(m_LineSegment,
                            actual [ 1 ],
                            "[1]");
            Assert.AreEqual(m_ArcSegmentTwo,
                            actual [ 2 ],
                            "[2]");
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
        public void PathDefaultTest()
        {
            Assert.AreEqual(Path.Unknown,
                            m_Converter.Path);
        }

        [Test]
        public void PathRoundtripTest()
        {
            m_Converter.Path = m_Path;

            Assert.AreEqual(m_Path,
                            m_Converter.Path);
        }
    }
}