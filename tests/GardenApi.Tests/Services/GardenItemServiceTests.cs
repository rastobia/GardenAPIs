namespace GardenApi.Tests.Services;

public class GardenItemServiceTests
{
    private readonly Mock<IGardenItemRepository> _mockItemRepo;
    private readonly Mock<IPlantRepository> _mockPlantRepo;
    private readonly Mock<IGardenRepository> _mockGardenRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GardenItemService _service;

    public GardenItemServiceTests()
    {
        _mockItemRepo = new Mock<IGardenItemRepository>();
        _mockPlantRepo = new Mock<IPlantRepository>();
        _mockGardenRepo = new Mock<IGardenRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new GardenItemService(
            _mockItemRepo.Object,
            _mockPlantRepo.Object,
            _mockGardenRepo.Object,
            _mockMapper.Object);
    }

    [Fact]
    public async Task AssignPlantAsync_WhenSunlightMatches_AssignsAndSaves()
    {
        var gardenItem = new GardenItem { Id = 1, SunlightReceived = SunlightLevel.FullSun };
        var plant = new Plant { Id = 2, Name = "Tomato", SunlightRequirement = SunlightLevel.FullSun };
        var expected = new GardenItemDto { Id = 1 };

        _mockItemRepo.Setup(r => r.GetByIdWithPlantAsync(1)).ReturnsAsync(gardenItem);
        _mockPlantRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(plant);
        _mockItemRepo.Setup(r => r.UpdateAsync(It.IsAny<GardenItem>())).Returns(Task.CompletedTask);
        _mockItemRepo.Setup(r => r.GetByIdWithPlantAsync(1)).ReturnsAsync(gardenItem);
        _mockMapper.Setup(m => m.Map<GardenItemDto>(gardenItem)).Returns(expected);

        var result = await _service.AssignPlantAsync(1, 2);

        result.Should().NotBeNull();
        _mockItemRepo.Verify(r => r.UpdateAsync(It.Is<GardenItem>(gi => gi.PlantId == 2)), Times.Once);
    }

    [Fact]
    public async Task AssignPlantAsync_WhenSunlightMismatch_ThrowsInvalidOperationException()
    {
        var gardenItem = new GardenItem { Id = 1, SunlightReceived = SunlightLevel.FullShade };
        var plant = new Plant { Id = 2, Name = "Tomato", SunlightRequirement = SunlightLevel.FullSun };

        _mockItemRepo.Setup(r => r.GetByIdWithPlantAsync(1)).ReturnsAsync(gardenItem);
        _mockPlantRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(plant);

        var act = async () => await _service.AssignPlantAsync(1, 2);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Sunlight mismatch*");
        _mockItemRepo.Verify(r => r.UpdateAsync(It.IsAny<GardenItem>()), Times.Never);
    }

    [Fact]
    public async Task AssignPlantAsync_WhenGardenItemNotFound_ThrowsKeyNotFoundException()
    {
        _mockItemRepo.Setup(r => r.GetByIdWithPlantAsync(99)).ReturnsAsync((GardenItem?)null);

        var act = async () => await _service.AssignPlantAsync(99, 1);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*99*");
    }

    [Fact]
    public async Task AssignPlantAsync_WhenPlantNotFound_ThrowsKeyNotFoundException()
    {
        var gardenItem = new GardenItem { Id = 1, SunlightReceived = SunlightLevel.FullSun };

        _mockItemRepo.Setup(r => r.GetByIdWithPlantAsync(1)).ReturnsAsync(gardenItem);
        _mockPlantRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Plant?)null);

        var act = async () => await _service.AssignPlantAsync(1, 99);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*99*");
    }

    [Fact]
    public async Task RemovePlantAsync_ClearsPlantIdAndUpdates()
    {
        var gardenItem = new GardenItem { Id = 1, PlantId = 5, SunlightReceived = SunlightLevel.FullSun };
        var expected = new GardenItemDto { Id = 1 };

        _mockItemRepo.Setup(r => r.GetByIdWithPlantAsync(1)).ReturnsAsync(gardenItem);
        _mockItemRepo.Setup(r => r.UpdateAsync(It.IsAny<GardenItem>())).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<GardenItemDto>(gardenItem)).Returns(expected);

        var result = await _service.RemovePlantAsync(1);

        result.Should().NotBeNull();
        _mockItemRepo.Verify(r => r.UpdateAsync(It.Is<GardenItem>(gi => gi.PlantId == null)), Times.Once);
    }

    [Fact]
    public async Task CreateGardenItemAsync_WhenGardenNotFound_ThrowsKeyNotFoundException()
    {
        var request = new CreateGardenItemRequest { GardenId = 99 };
        _mockGardenRepo.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);

        var act = async () => await _service.CreateGardenItemAsync(request);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*99*");
    }

    [Fact]
    public async Task DeleteGardenItemAsync_WhenNotFound_ReturnsFalse()
    {
        _mockItemRepo.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);

        var result = await _service.DeleteGardenItemAsync(99);

        result.Should().BeFalse();
        _mockItemRepo.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
    }
}
