using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using EasyNetQ;
using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Common;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Primitives;
using Selkie.WPF.Common;

namespace Selkie.WPF.Converters.Tests.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class PathToRacetracksConverterTests
    {
        [SetUp]
        public void Setup()
        {
            m_Racetracks = Substitute.For <IRacetracks>();

            m_SimplePath = new[]
                           {
                               0,
                               1,
                               2,
                               3
                           };
            m_Path = new[]
                     {
                         0,
                         2
                     };

            m_Logger = Substitute.For <ILogger>();
            m_Bus = Substitute.For <IBus>();
            m_Helper = new NodeIdHelper(Substitute.For <ILinesSourceManager>());
            m_Manager = Substitute.For <IRacetracksSourceManager>();
            m_Manager.Racetracks.Returns(m_Racetracks);

            SetupManager();

            m_Converter = new PathToRacetracksConverter(m_Logger,
                                                        m_Bus,
                                                        m_Helper,
                                                        m_Manager);
        }

        private IRacetracksSourceManager m_Manager;
        private PathToRacetracksConverter m_Converter;
        private ILogger m_Logger;
        private IPath m_Racetrack1;
        private IPath m_Racetrack2;
        private IPath[] m_Paths1And2;
        private IPath m_Racetrack3;
        private IPath m_Racetrack4;
        private IPath[] m_Paths3And4;
        private IPath[][] m_ForwardToReverse;
        private IPath m_Racetrack5;
        private IPath m_Racetrack6;
        private IPath[] m_Paths5And6;
        private IPath m_Racetrack7;
        private IPath m_Racetrack8;
        private IPath[] m_Paths7And8;
        private IPath m_Racetrack9;
        private IPath m_Racetrack10;
        private IPath[] m_Paths9And10;
        private IPath m_Racetrack11;
        private IPath m_Racetrack12;
        private IPath[] m_Paths11And12;
        private IPath m_Racetrack13;
        private IPath m_Racetrack14;
        private IPath[] m_Paths13And14;
        private IPath m_Racetrack15;
        private IPath m_Racetrack16;
        private IPath[] m_Paths15And16;
        private IPath[][] m_ForwardToForward;
        private IPath[][] m_ReverseToForward;
        private IPath[][] m_ReverseToReverse;
        private int[] m_SimplePath;
        private int[] m_Path;
        private NodeIdHelper m_Helper;
        private IBus m_Bus;
        private IRacetracks m_Racetracks;

        private void SetupManager()
        {
            m_Racetrack1 = CreatePath(1);
            m_Racetrack2 = CreatePath(2);
            m_Paths1And2 = new[]
                           {
                               m_Racetrack1,
                               m_Racetrack2
                           };

            m_Racetrack3 = CreatePath(3);
            m_Racetrack4 = CreatePath(4);
            m_Paths3And4 = new[]
                           {
                               m_Racetrack3,
                               m_Racetrack4
                           };

            m_ForwardToForward = new[]
                                 {
                                     m_Paths1And2,
                                     m_Paths3And4
                                 };
            m_Manager.Racetracks.ForwardToForward.Returns(m_ForwardToForward);

            m_Racetrack5 = CreatePath(5);
            m_Racetrack6 = CreatePath(6);
            m_Paths5And6 = new[]
                           {
                               m_Racetrack5,
                               m_Racetrack6
                           };

            m_Racetrack7 = CreatePath(7);
            m_Racetrack8 = CreatePath(8);
            m_Paths7And8 = new[]
                           {
                               m_Racetrack7,
                               m_Racetrack8
                           };

            m_ForwardToReverse = new[]
                                 {
                                     m_Paths5And6,
                                     m_Paths7And8
                                 };
            m_Manager.Racetracks.ForwardToReverse.Returns(m_ForwardToReverse);

            m_Racetrack9 = CreatePath(9);
            m_Racetrack10 = CreatePath(10);
            m_Paths9And10 = new[]
                            {
                                m_Racetrack9,
                                m_Racetrack10
                            };

            m_Racetrack11 = CreatePath(11);
            m_Racetrack12 = CreatePath(12);
            m_Paths11And12 = new[]
                             {
                                 m_Racetrack11,
                                 m_Racetrack12
                             };

            m_ReverseToForward = new[]
                                 {
                                     m_Paths9And10,
                                     m_Paths11And12
                                 };
            m_Manager.Racetracks.ReverseToForward.Returns(m_ReverseToForward);

            m_Racetrack13 = CreatePath(13);
            m_Racetrack14 = CreatePath(14);
            m_Paths13And14 = new[]
                             {
                                 m_Racetrack13,
                                 m_Racetrack14
                             };

            m_Racetrack15 = CreatePath(15);
            m_Racetrack16 = CreatePath(16);
            m_Paths15And16 = new[]
                             {
                                 m_Racetrack15,
                                 m_Racetrack16
                             };

            m_ReverseToReverse = new[]
                                 {
                                     m_Paths13And14,
                                     m_Paths15And16
                                 };
            m_Manager.Racetracks.ReverseToReverse.Returns(m_ReverseToReverse);
        }

        private IPath CreatePath(int i)
        {
            var path = Substitute.For <IPath>();
            path.Distance.Returns(new Distance(i));

            return path;
        }

        [Test]
        public void ConstructorSubscribesToColonyRacetracksChangedMessageTest()
        {
            m_Bus.Received().SubscribeAsync(m_Converter.GetType().FullName,
                                            Arg.Any <Func <ColonyRacetracksChangedMessage, Task>>());
        }

        [Test]
        public void ConvertReturnsRacetrackForPathZeroThreeTest()
        {
            m_Converter.Path = new[]
                               {
                                   0,
                                   3
                               };

            m_Converter.Convert();

            IPath[] actual = m_Converter.Paths.ToArray();

            Assert.AreEqual(1,
                            actual.Length,
                            "Length");
            Assert.AreEqual(m_Racetrack6,
                            actual [ 0 ],
                            "[0]");
        }

        [Test]
        public void ConvertReturnsRacetrackForPathZeroTwoTest()
        {
            m_Converter.Path = m_Path;

            m_Converter.Convert();

            IPath[] actual = m_Converter.Paths.ToArray();

            Assert.AreEqual(1,
                            actual.Length,
                            "Length");
            Assert.AreEqual(m_Racetrack2,
                            actual [ 0 ],
                            "[0]");
        }

        [Test]
        public void ConvertReturnsRacetracksForLongPathTest()
        {
            m_Converter.Path = m_SimplePath;

            m_Converter.Convert();

            IPath[] actual = m_Converter.Paths.ToArray();

            Assert.AreEqual(3,
                            actual.Length,
                            "Length");
            Assert.AreEqual(m_Racetrack5,
                            actual [ 0 ],
                            "[0]");
            Assert.AreEqual(m_Racetrack10,
                            actual [ 1 ],
                            "[1]");
            Assert.AreEqual(m_Racetrack8,
                            actual [ 2 ],
                            "[2]");
        }

        [Test]
        public void DefaultRacetracksTest()
        {
            Assert.AreEqual(m_Racetracks,
                            m_Converter.Racetracks);
        }

        [Test]
        public void GetPathForNodeForwardReturnsUnknownForFromLineIdToBig()
        {
            IPath actual = m_Converter.GetPathForNodeForward(true,
                                                             100,
                                                             0);

            Assert.AreEqual(Path.Unknown,
                            actual);
        }

        [Test]
        public void GetPathForNodeForwardReturnsUnknownForToLineIdToBig()
        {
            IPath actual = m_Converter.GetPathForNodeForward(true,
                                                             0,
                                                             100);

            Assert.AreEqual(Path.Unknown,
                            actual);
        }

        [Test]
        public void GetPathToNodeFowardReturnsUnknownForFromLineIdToBig()
        {
            IPath actual = m_Converter.GetPathToNodeFoward(true,
                                                           100,
                                                           0);

            Assert.AreEqual(Path.Unknown,
                            actual);
        }

        [Test]
        public void GetPathToNodeFowardReturnsUnknownForToLineIdToBig()
        {
            IPath actual = m_Converter.GetPathToNodeFoward(true,
                                                           0,
                                                           100);

            Assert.AreEqual(Path.Unknown,
                            actual);
        }

        [Test]
        public void GetRacetrackReturnsRacetrackForIndex0And0Test()
        {
            IPath actual = m_Converter.GetRacetrack(m_SimplePath,
                                                    0,
                                                    0);

            Assert.AreEqual(m_Racetrack1,
                            actual);
        }

        [Test]
        public void GetRacetrackReturnsRacetrackForIndex0And1Test()
        {
            IPath actual = m_Converter.GetRacetrack(m_SimplePath,
                                                    0,
                                                    1);

            Assert.AreEqual(m_Racetrack5,
                            actual);
        }

        [Test]
        public void GetRacetrackReturnsRacetrackForIndex0And2Test()
        {
            IPath actual = m_Converter.GetRacetrack(m_SimplePath,
                                                    0,
                                                    2);

            Assert.AreEqual(m_Racetrack2,
                            actual);
        }

        [Test]
        public void GetRacetrackReturnsRacetrackForIndex0And3Test()
        {
            IPath actual = m_Converter.GetRacetrack(m_SimplePath,
                                                    0,
                                                    3);

            Assert.AreEqual(m_Racetrack6,
                            actual);
        }

        [Test]
        public void GetRacetrackReturnsRacetrackForIndex1And0Test()
        {
            IPath actual = m_Converter.GetRacetrack(m_SimplePath,
                                                    1,
                                                    0);

            Assert.AreEqual(m_Racetrack9,
                            actual);
        }

        [Test]
        public void GetRacetrackReturnsRacetrackForIndex1And1Test()
        {
            IPath actual = m_Converter.GetRacetrack(m_SimplePath,
                                                    1,
                                                    1);

            Assert.AreEqual(m_Racetrack13,
                            actual);
        }

        [Test]
        public void GetRacetrackReturnsRacetrackForIndex1And2Test()
        {
            IPath actual = m_Converter.GetRacetrack(m_SimplePath,
                                                    1,
                                                    2);

            Assert.AreEqual(m_Racetrack10,
                            actual);
        }

        [Test]
        public void GetRacetrackReturnsRacetrackForIndex1And3Test()
        {
            IPath actual = m_Converter.GetRacetrack(m_SimplePath,
                                                    1,
                                                    3);

            Assert.AreEqual(m_Racetrack14,
                            actual);
        }

        [Test]
        public void GetRacetrackReturnsRacetrackForIndex2And0Test()
        {
            IPath actual = m_Converter.GetRacetrack(m_SimplePath,
                                                    2,
                                                    0);

            Assert.AreEqual(m_Racetrack3,
                            actual);
        }

        [Test]
        public void GetRacetrackReturnsRacetrackForIndex2And1Test()
        {
            IPath actual = m_Converter.GetRacetrack(m_SimplePath,
                                                    2,
                                                    1);

            Assert.AreEqual(m_Racetrack7,
                            actual);
        }

        [Test]
        public void GetRacetrackReturnsRacetrackForIndex2And2Test()
        {
            IPath actual = m_Converter.GetRacetrack(m_SimplePath,
                                                    2,
                                                    2);

            Assert.AreEqual(m_Racetrack4,
                            actual);
        }

        [Test]
        public void GetRacetrackReturnsRacetrackForIndex2And3Test()
        {
            IPath actual = m_Converter.GetRacetrack(m_SimplePath,
                                                    2,
                                                    3);

            Assert.AreEqual(m_Racetrack8,
                            actual);
        }

        [Test]
        public void GetRacetrackReturnsRacetrackForIndex3And0Test()
        {
            IPath actual = m_Converter.GetRacetrack(m_SimplePath,
                                                    3,
                                                    0);

            Assert.AreEqual(m_Racetrack11,
                            actual);
        }

        [Test]
        public void GetRacetrackReturnsRacetrackForIndex3And1Test()
        {
            IPath actual = m_Converter.GetRacetrack(m_SimplePath,
                                                    3,
                                                    1);

            Assert.AreEqual(m_Racetrack15,
                            actual);
        }

        [Test]
        public void GetRacetrackReturnsRacetrackForIndex3And2Test()
        {
            IPath actual = m_Converter.GetRacetrack(m_SimplePath,
                                                    3,
                                                    2);

            Assert.AreEqual(m_Racetrack12,
                            actual);
        }

        [Test]
        public void GetRacetrackReturnsRacetrackForIndex3And3Test()
        {
            IPath actual = m_Converter.GetRacetrack(m_SimplePath,
                                                    3,
                                                    3);

            Assert.AreEqual(m_Racetrack16,
                            actual);
        }

        [Test]
        public void GetRacetrackReturnsUnknownForCurrentNodeSmallTest()
        {
            IPath actual = m_Converter.GetRacetrack(new int[]
                                                    {
                                                    },
                                                    -10,
                                                    0);

            Assert.True(actual.IsUnknown);
        }

        [Test]
        public void GetRacetrackReturnsUnknownForCurrentNodeTooBigTest()
        {
            IPath actual = m_Converter.GetRacetrack(new int[]
                                                    {
                                                    },
                                                    10,
                                                    0);

            Assert.True(actual.IsUnknown);
        }

        [Test]
        public void GetRacetrackReturnsUnknownForNextNodeTooBigTest()
        {
            IPath actual = m_Converter.GetRacetrack(new int[]
                                                    {
                                                    },
                                                    0,
                                                    10);

            Assert.True(actual.IsUnknown);
        }

        [Test]
        public void GetRacetrackReturnsUnknownForNextNodeToSmallTest()
        {
            IPath actual = m_Converter.GetRacetrack(new int[]
                                                    {
                                                    },
                                                    0,
                                                    -10);

            Assert.True(actual.IsUnknown);
        }

        [Test]
        public void PathRoundtripTest()
        {
            var newPath = new[]
                          {
                              1,
                              3
                          };

            m_Converter.Path = newPath;

            Assert.AreEqual(newPath,
                            m_Converter.Path);
        }

        [Test]
        public void PathsDefaultTest()
        {
            Assert.AreEqual(0,
                            m_Converter.Paths.Count());
        }

        [Test]
        public void PathTest()
        {
            m_Converter.Path = m_Path;

            Assert.AreEqual(m_Path,
                            m_Converter.Path);
        }

        [Test]
        public void RacetrackSettingsChangedHandlerCallsUpdateTest()
        {
            var racetracks = Substitute.For <IRacetracks>();
            m_Manager.Racetracks.Returns(racetracks);

            var message = new ColonyRacetracksChangedMessage();

            m_Converter.ColonyRacetracksChangedHandler(message);

            Assert.AreEqual(racetracks,
                            m_Converter.Racetracks);
        }

        [Test]
        public void UpdateSetsRacetracksTest()
        {
            var racetracks = Substitute.For <IRacetracks>();
            m_Manager.Racetracks.Returns(racetracks);

            m_Converter.Update();

            Assert.AreEqual(racetracks,
                            m_Converter.Racetracks);
        }
    }
}