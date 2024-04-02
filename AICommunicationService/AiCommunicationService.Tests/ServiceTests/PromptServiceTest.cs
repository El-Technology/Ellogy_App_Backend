using AICommunicationService.BLL.Dtos;
using AICommunicationService.BLL.Services;
using AICommunicationService.DAL.Interfaces;
using AICommunicationService.DAL.Models;
using AutoFixture;
using AutoMapper;
using Moq;

namespace AiCommunicationService.Tests.ServiceTests;

[TestFixture]
public class PromptServiceTest
{
    private IMapper _mapper;
    private Fixture _fixture;
    private Mock<IAIPromptRepository> _promptRepositoryMock;
    private PromptService _promptService;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _promptRepositoryMock = new Mock<IAIPromptRepository>();
        _promptService = new PromptService(_promptRepositoryMock.Object);

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<AIPrompt, AIPrompt>();
        });

        _mapper = mapperConfig.CreateMapper();
    }

    [Test]
    public async Task AddPromptAsync_AddsNewPromptToDb()
    {
        var createPromptDto = _fixture.Create<CreatePromptDto>();
        var result = await _promptService.AddPromptAsync(createPromptDto);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(createPromptDto, Is.EqualTo(result));
        });
    }

    [Test]
    public async Task UpdatePromptAsync_UpdatesExistingPrompt()
    {
        // Arrange
        var createPromptDto = _fixture.Create<CreatePromptDto>();
        var result = await _promptService.AddPromptAsync(createPromptDto);

        var saveResult = _mapper.Map<AIPrompt>(result);

        var updatePromptDto = new UpdatePrompt
        {
            Description = "New description",
            Functions = "New function",
            Input = "New Input",
            Value = "New Value"
        };

        _promptRepositoryMock.Setup(x => x.GetPromptByTemplateNameAsync(result.TemplateName))
            .ReturnsAsync(result);

        // Act
        var updatedPrompt = await _promptService.UpdatePromptAsync(updatePromptDto, result.TemplateName);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(updatedPrompt, Is.Not.Null);
            Assert.That(updatedPrompt, Is.Not.EqualTo(saveResult));
        });
    }

    [Test]
    public async Task GetAllPromptsAsync_ReturnsAllPrompts()
    {
        // Arrange
        var createPromptsDto = _fixture.CreateMany<CreatePromptDto>(5);
        var expectedPrompts = _mapper.Map<List<AIPrompt>>(createPromptsDto);

        _promptRepositoryMock.Setup(x => x.GetAllPromptsAsync())
            .ReturnsAsync(expectedPrompts);

        // Act
        var result = await _promptService.GetAllPromptsAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(5));
        });
    }

    [Test]
    public async Task DeletePromptAsync_DeletesPrompt()
    {
        // Arrange
        var createPromptsDto = _fixture.CreateMany<CreatePromptDto>(5);
        var promptToDelete = createPromptsDto.First();

        _promptRepositoryMock.Setup(x => x.GetPromptByTemplateNameAsync(promptToDelete.TemplateName))
            .ReturnsAsync(_mapper.Map<AIPrompt>(promptToDelete));

        _promptRepositoryMock.Setup(x => x.DeletePromptAsync(promptToDelete))
            .Returns(Task.CompletedTask);

        // Act
        await _promptService.DeletePromptAsync(promptToDelete.TemplateName);

        var result = await _promptService.GetAllPromptsAsync();

        _promptRepositoryMock.Verify(x => x.GetAllPromptsAsync(), Times.Once);
        _promptRepositoryMock.Verify(x => x.GetPromptByTemplateNameAsync(It.IsAny<string>()), Times.Once);
        _promptRepositoryMock.Verify(x => x.DeletePromptAsync(It.IsAny<AIPrompt>()), Times.Once);
    }
}
