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
    public async Task AssignPlantAsync_WhenSunlightMatchesAndSpaceFits_AssignsAndSaves()
    {
        var plant = new Plant { Id = 2, Name = "Tomato", SunlightRequirement = SunlightLevel.FullSun, Spacing = 6 };
        var gardenItem = new GardenItem
        {
            Id = 1,
            Width = 24,
            Height = 24,
            SunlightReceived = SunlightLevel.FullSun,
            GardenItemPlants = new List<GardenItemPlant>()
        };
        var expected = new GardenItemDto { Id = 1 };

        _mockItemRepo.Setup(r => r.GetByIdWithPlantsAsync(1)).ReturnsAsync(gardenItem);
        _mockPlantRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(plant);
        _mockItemRepo.Setup(r => r.UpdateAsync(It.IsAny<GardenItem>())).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<GardenItemDto>(gardenItem)).Returns(expected);

        var result = await _service.AssignPlantAsync(1, 2);

        result.Should().NotBeNull();
        _mockItemRepo.Verify(r => r.UpdateAsync(It.Is<GardenItem>(
            gi => gi.GardenItemPlants.Any(gip => gip.PlantId == 2))), Times.Once);
    }

    [Fact]
    public async Task AssignPlantAsync_WhenSunlightMismatch_ThrowsInvalidOperationException()
    {
        var gardenItem = new GardenItem
        {
            Id = 1,
            Width = 24,
            Height = 24,
            SunlightReceived = SunlightLevel.FullShade,
            GardenItemPlants = new List<GardenItemPlant>()
        };
        var plant = new Plant { Id = 2, Name = "Tomato", SunlightRequirement = SunlightLevel.FullSun, Spacing = 6 };

        _mockItemRepo.Setup(r => r.GetByIdWithPlantsAsync(1)).ReturnsAsync(gardenItem);
        _mockPlantRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(plant);

        var act = async () => await _service.AssignPlantAsync(1, 2);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Sunlight mismatch*");
        _mockItemRepo.Verify(r => r.UpdateAsync(It.IsAny<GardenItem>()), Times.Never);
    }

    [Fact]
    public async Task AssignPlantAsync_WhenNoSpaceRemaining_ThrowsInvalidOperationException()
    {
        var existingPlant = new Plant { Id = 3, Spacing = 6m };
        var gardenItem = new GardenItem
        {
            Id = 1,
            Width = 10,
            Height = 10,
            SunlightReceived = SunlightLevel.FullSun,
            GardenItemPlants = new List<GardenItemPlant>
            {
                new GardenItemPlant { PlantId = 3, Plant = existingPlant }
            }
        };
        // Used area = 36, available = 64. New plant needs 81 (9×9) — doesn't fit.
        var newPlant = new Plant { Id = 2, Name = "Tomato", SunlightRequirement = SunlightLevel.FullSun, Spacing = 9 };

        _mockItemRepo.Setup(r => r.GetByIdWithPlantsAsync(1)).ReturnsAsync(gardenItem);
        _mockPlantRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(newPlant);

        var act = async () => await _service.AssignPlantAsync(1, 2);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Not enough space*");
        _mockItemRepo.Verify(r => r.UpdateAsync(It.IsAny<GardenItem>()), Times.Never);
    }

    [Fact]
    public async Task AssignPlantAsync_WhenGardenItemNotFound_ThrowsKeyNotFoundException()
    {
        _mockItemRepo.Setup(r => r.GetByIdWithPlantsAsync(99)).ReturnsAsync((GardenItem?)null);

        var act = async () => await _service.AssignPlantAsync(99, 1);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*99*");
    }

    [Fact]
    public async Task AssignPlantAsync_WhenPlantNotFound_ThrowsKeyNotFoundException()
    {
        var gardenItem = new GardenItem
        {
            Id = 1,
            Width = 24,
            Height = 24,
            SunlightReceived = SunlightLevel.FullSun,
            GardenItemPlants = new List<GardenItemPlant>()
        };

        _mockItemRepo.Setup(r => r.GetByIdWithPlantsAsync(1)).ReturnsAsync(gardenItem);
        _mockPlantRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Plant?)null);

        var act = async () => await _service.AssignPlantAsync(1, 99);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*99*");
    }

    [Fact]
    public async Task RemovePlantAsync_RemovesJunctionEntryAndUpdates()
    {
        var entry = new GardenItemPlant { GardenItemId = 1, PlantId = 5 };
        var gardenItem = new GardenItem
        {
            Id = 1,
            SunlightReceived = SunlightLevel.FullSun,
            GardenItemPlants = new List<GardenItemPlant> { entry }
        };
        var expected = new GardenItemDto { Id = 1 };

        _mockItemRepo.Setup(r => r.GetByIdWithPlantsAsync(1)).ReturnsAsync(gardenItem);
        _mockItemRepo.Setup(r => r.GetGardenItemPlantAsync(1, 5)).ReturnsAsync(entry);
        _mockItemRepo.Setup(r => r.RemoveGardenItemPlantAsync(entry)).Returns(Task.CompletedTask);
        _mockItemRepo.Setup(r => r.UpdateAsync(It.IsAny<GardenItem>())).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<GardenItemDto>(gardenItem)).Returns(expected);

        var result = await _service.RemovePlantAsync(1, 5);

        result.Should().NotBeNull();
        _mockItemRepo.Verify(r => r.RemoveGardenItemPlantAsync(entry), Times.Once);
    }

    [Fact]
    public async Task RemovePlantAsync_WhenPlantNotAssigned_ThrowsKeyNotFoundException()
    {
        var gardenItem = new GardenItem
        {
            Id = 1,
            SunlightReceived = SunlightLevel.FullSun,
            GardenItemPlants = new List<GardenItemPlant>()
        };

        _mockItemRepo.Setup(r => r.GetByIdWithPlantsAsync(1)).ReturnsAsync(gardenItem);
        _mockItemRepo.Setup(r => r.GetGardenItemPlantAsync(1, 99)).ReturnsAsync((GardenItemPlant?)null);

        var act = async () => await _service.RemovePlantAsync(1, 99);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*99*");
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
