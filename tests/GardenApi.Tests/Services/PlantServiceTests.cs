namespace GardenApi.Tests.Services;

public class PlantServiceTests
{
    private readonly Mock<IPlantRepository> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly PlantService _service;

    public PlantServiceTests()
    {
        _mockRepo = new Mock<IPlantRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new PlantService(_mockRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetAllPlantsAsync_ReturnsMappedDtos()
    {
        var plants = new List<Plant> { new() { Id = 1, Name = "Tomato" } };
        var expected = new List<PlantDto> { new() { Id = 1, Name = "Tomato" } };

        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(plants);
        _mockMapper.Setup(m => m.Map<IEnumerable<PlantDto>>(plants)).Returns(expected);

        var result = await _service.GetAllPlantsAsync();

        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Tomato");
    }

    [Fact]
    public async Task GetPlantByIdAsync_WhenNotFound_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Plant?)null);

        var result = await _service.GetPlantByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPlantsByTypeAsync_DelegatesToRepository()
    {
        var plants = new List<Plant> { new() { Id = 1, Type = PlantType.Vegetable } };
        var expected = new List<PlantDto> { new() { Id = 1, Type = "Vegetable" } };

        _mockRepo.Setup(r => r.GetByTypeAsync(PlantType.Vegetable)).ReturnsAsync(plants);
        _mockMapper.Setup(m => m.Map<IEnumerable<PlantDto>>(plants)).Returns(expected);

        var result = await _service.GetPlantsByTypeAsync(PlantType.Vegetable);

        result.Should().HaveCount(1);
        _mockRepo.Verify(r => r.GetByTypeAsync(PlantType.Vegetable), Times.Once);
    }

    [Fact]
    public async Task GetCompatiblePlantsForSunlightAsync_DelegatesToRepository()
    {
        var plants = new List<Plant> { new() { Id = 1, SunlightRequirement = SunlightLevel.FullSun } };
        var expected = new List<PlantDto> { new() { Id = 1 } };

        _mockRepo.Setup(r => r.GetCompatibleWithSunlightAsync(SunlightLevel.FullSun)).ReturnsAsync(plants);
        _mockMapper.Setup(m => m.Map<IEnumerable<PlantDto>>(plants)).Returns(expected);

        var result = await _service.GetCompatiblePlantsForSunlightAsync(SunlightLevel.FullSun);

        result.Should().HaveCount(1);
        _mockRepo.Verify(r => r.GetCompatibleWithSunlightAsync(SunlightLevel.FullSun), Times.Once);
    }

    [Fact]
    public async Task DeletePlantAsync_WhenNotFound_ReturnsFalseWithoutDeleting()
    {
        _mockRepo.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);

        var result = await _service.DeletePlantAsync(99);

        result.Should().BeFalse();
        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
    }
}
