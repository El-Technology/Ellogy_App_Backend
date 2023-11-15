using AICommunicationService.BLL.Dtos;
using AICommunicationService.BLL.Services;
using AICommunicationService.DAL.Context.AiCommunication;
using AICommunicationService.DAL.Models;
using AICommunicationService.DAL.Repositories;
using AutoFixture;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AiCommunicationService.Tests.ServiceTests
{
    [TestFixture]
    public class PromptServiceTest
    {
        private IMapper _mapper;
        private Fixture _fixture;
        private AICommunicationContext _context;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var options = new DbContextOptionsBuilder<AICommunicationContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDb")
                .Options;

            _context = new AICommunicationContext(options);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AIPrompt, AIPrompt>();
            });

            _mapper = mapperConfig.CreateMapper();
        }

        [TearDown]
        public async Task TearDown()
        {
            await _context.DisposeAsync();
        }

        [Test]
        public async Task AddPromptAsync_AddsNewPromptToDb()
        {
            var promptRepository = new AIPromptRepository(_context);
            var promptService = new PromptService(promptRepository);

            var createPromptDto = _fixture.Create<CreatePromptDto>();
            var result = await promptService.AddPromptAsync(createPromptDto);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(createPromptDto, Is.EqualTo(result));
            });
        }

        [Test]
        public async Task UpdatePromptAsync_UpdatesExistingPrompt()
        {
            var promptRepository = new AIPromptRepository(_context);
            var promptService = new PromptService(promptRepository);

            //Add prompt
            var createPromptDto = _fixture.Create<CreatePromptDto>();
            var result = await promptService.AddPromptAsync(createPromptDto);

            var saveResult = _mapper.Map<AIPrompt>(result);

            //Update prompt
            var updatePromptDto = new UpdatePrompt
            {
                Description = "New description",
                Functions = "New function",
                Input = "New Input",
                Value = "New Value"
            };
            var updatedPrompt = await promptService.UpdatePromptAsync(updatePromptDto, result.TemplateName);

            Assert.Multiple(() =>
            {
                Assert.That(updatedPrompt, Is.Not.Null);
                Assert.That(updatedPrompt, Is.Not.EqualTo(saveResult));
            });
        }

        [Test]
        public async Task GetAllPromptsAsync_ReturnsAllPrompts()
        {
            await ClearDatabase();
            var promptRepository = new AIPromptRepository(_context);
            var promptService = new PromptService(promptRepository);

            //Add prompts
            var createPromptsDto = _fixture.CreateMany<CreatePromptDto>(5);
            foreach (var prompt in createPromptsDto)
            {
                await promptService.AddPromptAsync(prompt);
            }

            var result = await promptService.GetAllPromptsAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Has.Count.EqualTo(5));
            });
        }

        [Test]
        public async Task DeletePromptAsync_DeletesPrompt()
        {
            await ClearDatabase();
            var promptRepository = new AIPromptRepository(_context);
            var promptService = new PromptService(promptRepository);

            //Add prompts
            var createPromptsDto = _fixture.CreateMany<CreatePromptDto>(5);
            foreach (var prompt in createPromptsDto)
            {
                await promptService.AddPromptAsync(prompt);
            }

            await promptService.DeletePromptAsync(createPromptsDto.First().TemplateName);

            var result = await promptService.GetAllPromptsAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Has.Count.EqualTo(4));
            });
        }

        private async Task ClearDatabase()
        {
            var prompts = await _context.AIPrompts.ToListAsync();
            _context.AIPrompts.RemoveRange(prompts);
            await _context.SaveChangesAsync();
        }
    }
}
