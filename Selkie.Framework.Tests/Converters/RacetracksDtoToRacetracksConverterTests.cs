using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Converters;
using Selkie.Framework.Interfaces;
using Selkie.Services.Common.Dto;

namespace Selkie.Framework.Tests.Converters
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class RacetracksDtoToRacetracksConverterTests
    {
        [SetUp]
        public void Setup()
        {
            m_PathDtoToPath = Substitute.For <IPathDtoToPath>();
        }

        private IPathDtoToPath m_PathDtoToPath;

        private static RacetracksDto CreateRacetracksDto()
        {
            return new RacetracksDto
                   {
                       ForwardToForward = CreatePathDtos(),
                       ForwardToReverse = CreatePathDtos(),
                       ReverseToForward = CreatePathDtos(),
                       ReverseToReverse = CreatePathDtos()
                   };
        }

        private static PathDto[][] CreatePathDtos()
        {
            return new[]
                   {
                       new[]
                       {
                           new PathDto(),
                           new PathDto()
                       },
                       new[]
                       {
                           new PathDto(),
                           new PathDto()
                       }
                   };
        }

        private RacetracksDtoToRacetracksConverter CreateSut()
        {
            return new RacetracksDtoToRacetracksConverter(m_PathDtoToPath);
        }

        [Test]
        public void Constructor_SetsDtoToUnknown_WhenCreated()
        {
            // Arrange
            RacetracksDtoToRacetracksConverter sut = CreateSut();

            // Act
            // Assert
            Assert.True(sut.Dto.IsUnknown);
        }

        [Test]
        public void Convert_SetsRacetracks_WhenCalled()
        {
            // Arrange
            var path = Substitute.For <IPath>();
            m_PathDtoToPath.Path.Returns(path);

            RacetracksDto dto = CreateRacetracksDto();
            RacetracksDtoToRacetracksConverter sut = CreateSut();
            sut.Dto = dto;

            // Act
            sut.Convert();

            // Assert
            Assert.AreEqual(2,
                            sut.Racetracks.ForwardToForward.Length,
                            "ForwardToForward");
            Assert.AreEqual(2,
                            sut.Racetracks.ForwardToReverse.Length,
                            "ForwardToReverse");
            Assert.AreEqual(2,
                            sut.Racetracks.ReverseToForward.Length,
                            "ReverseToForward");
            Assert.AreEqual(2,
                            sut.Racetracks.ReverseToReverse.Length,
                            "ReverseToReverse");
        }

        [Test]
        public void ConvertPathDto_CallsConvert_WhenCalled()
        {
            // Arrange
            var dto = new PathDto();
            RacetracksDtoToRacetracksConverter sut = CreateSut();

            // Act
            sut.ConvertPathDto(dto);

            // Assert
            m_PathDtoToPath.Received().Convert();
        }

        [Test]
        public void ConvertPathDto_ReturnsPath_WhenCalled()
        {
            // Arrange
            var path = Substitute.For <IPath>();
            m_PathDtoToPath.Path.Returns(path);
            var dto = new PathDto();
            RacetracksDtoToRacetracksConverter sut = CreateSut();

            // Act
            IPath actual = sut.ConvertPathDto(dto);

            // Assert
            Assert.AreEqual(path,
                            actual);
        }

        [Test]
        public void ConvertPathDto_SetsDto_WhenCalled()
        {
            // Arrange
            var dto = new PathDto();
            RacetracksDtoToRacetracksConverter sut = CreateSut();

            // Act
            sut.ConvertPathDto(dto);

            // Assert
            Assert.AreEqual(dto,
                            m_PathDtoToPath.Dto);
        }

        [Test]
        public void ConvertPathDtos_ReturnsArray_ForDtoArray()
        {
            // Arrange
            var path = Substitute.For <IPath>();
            m_PathDtoToPath.Path.Returns(path);
            PathDto[][] dtos = CreatePathDtos();
            RacetracksDtoToRacetracksConverter sut = CreateSut();

            // Act
            IPath[][] actual = sut.ConvertPathDtos(dtos);

            // Assert
            Assert.AreEqual(2,
                            actual.Length,
                            "[][]");
            Assert.AreEqual(2,
                            actual.First().Length,
                            "[0][]");
        }

        [Test]
        public void ConvertPathDtos_ReturnsEmptyArray_ForEmptyDtoArray()
        {
            // Arrange
            var dtos = new PathDto[0][];
            RacetracksDtoToRacetracksConverter sut = CreateSut();

            // Act
            IPath[][] actual = sut.ConvertPathDtos(dtos);

            // Assert
            Assert.False(actual.Any());
        }

        [Test]
        public void Dto_Roundtrip()
        {
            // Arrange
            var dto = new RacetracksDto();
            RacetracksDtoToRacetracksConverter sut = CreateSut();

            // Act
            sut.Dto = dto;

            // Assert
            Assert.AreEqual(dto,
                            sut.Dto);
        }
    }
}