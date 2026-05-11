namespace GardenApi.Tests.Services;

public class GardenServiceTests
{
    private readonly Mock<IGardenRepository> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GardenService _service;

    public GardenServiceTests()
    {
        _mockRepo = new Mock<IGardenRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new GardenService(_mockRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetAllGardensAsync_ReturnsAllGardens()
    {
        var gardens = new List<Garden> { new() { Id = 1, Name = "My Garden" } };
        var expected = new List<GardenDto> { new() { Id = 1, Name = "My Garden" } };

        _mockRepo.Setup(r => r.GetAllWithItemsAsync()).ReturnsAsync(gardens);
        _mockMapper.Setup(m => m.Map<IEnumerable<GardenDto>>(gardens)).Returns(expected);

        var result = await _service.GetAllGardensAsync();

        result.Should().HaveCount(1);
        result.First().Name.Should().Be("My Garden");
    }

    [Fact]
    public async Task GetGardenByIdAsync_WhenFound_ReturnsDto()
    {
        var garden = new Garden { Id = 1, Name = "Test Garden" };
        var expected = new GardenDto { Id = 1, Name = "Test Garden" };

        _mockRepo.Setup(r => r.GetByIdWithItemsAsync(1)).ReturnsAsync(garden);
        _mockMapper.Setup(m => m.Map<GardenDto>(garden)).Returns(expected);

        var result = await _service.GetGardenByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetGardenByIdAsync_WhenNotFound_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetByIdWithItemsAsync(99)).ReturnsAsync((Garden?)null);

        var result = await _service.GetGardenByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateGardenAsync_SetsTimestampsBeforeSaving()
    {
        var request = new CreateGardenRequest { Name = "New Garden" };
        var mappedGarden = new Garden { Name = "New Garden" };
        var savedGarden = new Garden { Id = 1, Name = "New Garden" };
        var expected = new GardenDto { Id = 1, Name = "New Garden" };

        _mockMapper.Setup(m => m.Map<Garden>(request)).Returns(mappedGarden);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Garden>())).ReturnsAsync(savedGarden);
        _mockMapper.Setup(m => m.Map<GardenDto>(savedGarden)).Returns(expected);

        var result = await _service.CreateGardenAsync(request);

        result.Id.Should().Be(1);
        _mockRepo.Verify(r => r.AddAsync(It.Is<Garden>(g =>
            g.CreatedDate != default && g.ModifiedDate != default)), Times.Once);
    }

    [Fact]
    public async Task UpdateGardenAsync_WhenFound_UpdatesModifiedDate()
    {
        var request = new UpdateGardenRequest { Name = "Updated" };
        var existingGarden = new Garden { Id = 1, Name = "Old", ModifiedDate = DateTime.UtcNow.AddDays(-1) };
        var expected = new GardenDto { Id = 1, Name = "Updated" };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingGarden);
        _mockMapper.Setup(m => m.Map(request, existingGarden));
        _mockMapper.Setup(m => m.Map<GardenDto>(existingGarden)).Returns(expected);

        var result = await _service.UpdateGardenAsync(1, request);

        result.Should().NotBeNull();
        _mockRepo.Verify(r => r.UpdateAsync(It.Is<Garden>(g =>
            g.ModifiedDate > DateTime.UtcNow.AddMinutes(-1))), Times.Once);
    }

    [Fact]
    public async Task UpdateGardenAsync_WhenNotFound_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Garden?)null);

        var result = await _service.UpdateGardenAsync(99, new UpdateGardenRequest());

        result.Should().BeNull();
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Garden>()), Times.Never);
    }

    [Fact]
    public async Task DeleteGardenAsync_WhenFound_DeletesAndReturnsTrue()
    {
        _mockRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);

        var result = await _service.DeleteGardenAsync(1);

        result.Should().BeTrue();
        _mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteGardenAsync_WhenNotFound_ReturnsFalseWithoutDeleting()
    {
        _mockRepo.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);

        var result = await _service.DeleteGardenAsync(99);

        result.Should().BeFalse();
        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
    }
}
