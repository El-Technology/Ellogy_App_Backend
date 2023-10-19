using AutoFixture;
using TicketsManager.BLL.Dtos.TicketUsecaseDtos;
using TicketsManager.BLL.Dtos.TicketUsecaseDtos.FullDtos;
using TicketsManager.BLL.Dtos.TicketUsecaseDtos.UsecasesDtos;
using TicketsManager.BLL.Services;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Models;
using TicketsManager.DAL.Repositories;

namespace TicketsManager.Tests.Services
{
    [TestFixture]
    public class UsecasesServiceTest : TestServiceBase
    {
        [Test]
        public async Task CreateUsecasesAsync_ValidInput_ReturnsCorrectResponse()
        {
            var usecaseRepository = new UsecaseRepository(_ticketsManagerDbContext);
            var usecasesService = new UsecasesService(usecaseRepository, _mapper);

            //Create new user with tickets
            var user = _fixture.Create<User>();
            await _ticketsManagerDbContext.Users.AddAsync(user);
            await _ticketsManagerDbContext.SaveChangesAsync();

            var listOfUsecases = new List<CreateUsecasesDto>
            {
                new CreateUsecasesDto
                {
                    TicketId = user.UserTickets.First().Id,
                    Usecase = new UsecaseDto
                    {
                        Title = "Usecase Example",
                        Diagrams = new() { new TicketDiagramDto { Title = "Diagram Example", Description = "Description", PictureLink = "PictureLink" } },
                        Tables = new() { new TicketTableDto { Table = "Table Example" } }
                    }
                }
            };
            var response = await usecasesService.CreateUsecasesAsync(listOfUsecases, user.Id);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Usecases, Has.Count.EqualTo(1));
            Assert.That(response.Usecases.First(), Is.Not.Null);
        }

        [Test]
        public async Task GetUsecasesAsync_ValidInput_ReturnsCorrectResponse()
        {
            var usecaseRepository = new UsecaseRepository(_ticketsManagerDbContext);
            var usecasesService = new UsecasesService(usecaseRepository, _mapper);

            // Create new user with tickets
            var user = _fixture.Create<User>();
            await _ticketsManagerDbContext.Users.AddAsync(user);
            await _ticketsManagerDbContext.SaveChangesAsync();

            var ticketId = user.UserTickets.First().Id;
            var getUsecasesDto = new GetUsecasesDto
            {
                TicketId = ticketId,
                PaginationRequest = new PaginationRequestDto
                {
                    CurrentPageNumber = 1,
                    RecordsPerPage = 10
                }
            };

            var response = await usecasesService.GetUsecasesAsync(getUsecasesDto, user.Id);

            Assert.That(response, Is.Not.Null);
        }

        [Test]
        public async Task UpdateUsecaseAsync_ValidInput_ReturnsCorrectResponse()
        {
            var usecaseRepository = new UsecaseRepository(_ticketsManagerDbContext);
            var usecasesService = new UsecasesService(usecaseRepository, _mapper);

            //Create new user with tickets
            var user = _fixture.Create<User>();
            await _ticketsManagerDbContext.Users.AddAsync(user);
            await _ticketsManagerDbContext.SaveChangesAsync();

            var updateUsecaseFullDto = new UsecaseDataFullDto
            {
                Diagrams = new()
                {
                    new TicketDiagramFullDto
                    {
                        Description = "New description",
                        PictureLink = "New link",
                        Title = "New title",
                    }
                },
                Title = "New title",
                Tables = new()
                {
                    new TicketTableFullDto
                    {
                        Table = "New Table"
                    }
                }
            };

            var response = await usecasesService.UpdateUsecaseAsync(user.UserTickets.First().Usecases.First().Id, updateUsecaseFullDto, user.Id);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Tables.First().Table, Is.EqualTo("New Table"));
        }

        [Test]
        public async Task DeleteUsecasesByTicketIdAsync_ValidInput_CallsRepositoryAndDeletesUsecases()
        {
            var usecaseRepository = new UsecaseRepository(_ticketsManagerDbContext);
            var usecasesService = new UsecasesService(usecaseRepository, _mapper);

            //Create new user with tickets
            var user = _fixture.Create<User>();
            await _ticketsManagerDbContext.Users.AddAsync(user);
            await _ticketsManagerDbContext.SaveChangesAsync();

            await usecasesService.DeleteUsecasesByTicketIdAsync(user.UserTickets.First().Id, user.Id);

            var getUsecasesDto = new GetUsecasesDto
            {
                TicketId = user.UserTickets.First().Id,
                PaginationRequest = new PaginationRequestDto
                {
                    CurrentPageNumber = 1,
                    RecordsPerPage = 10
                }
            };
            var response = await usecasesService.GetUsecasesAsync(getUsecasesDto, user.Id);

            Assert.That(response.Data, Is.Empty);
        }
    }
}
