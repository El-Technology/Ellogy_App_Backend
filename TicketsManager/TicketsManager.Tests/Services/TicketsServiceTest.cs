using AutoFixture;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;
using TicketsManager.BLL.Dtos.MessageDtos;
using TicketsManager.BLL.Dtos.TicketDtos;
using TicketsManager.BLL.Services;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Models;
using TicketsManager.DAL.Repositories;

namespace TicketsManager.Tests.Services
{
    [TestFixture]
    public class TicketsServiceTest : TestServiceBase
    {
        [Test]
        public async Task GetTicketsAsync_ReturnsAllUserTickets()
        {
            var userRepository = new UserRepository(_ticketsManagerDbContext);
            var ticketsRepository = new TicketsRepository(_ticketsManagerDbContext, userRepository);
            var ticketsService = new TicketsService(_mapper, ticketsRepository, userRepository);

            //Create new user with tickets
            var user = _fixture.Create<User>();
            await _ticketsManagerDbContext.Users.AddAsync(user);
            await _ticketsManagerDbContext.SaveChangesAsync();

            //Create get request
            var paginationRequest = new PaginationRequestDto { CurrentPageNumber = 1, RecordsPerPage = 10 };
            var tickets = await ticketsService.GetTicketsAsync(user.Id, paginationRequest, user.Id);

            Assert.Multiple(() =>
            {
                Assert.That(tickets, Is.Not.Null);
                Assert.That(tickets.Data, Has.Count.GreaterThan(1));
            });
        }

        [Test]
        public async Task SearchTicketsByNameAsync_ReturnsTickets()
        {
            var userRepository = new UserRepository(_ticketsManagerDbContext);
            var ticketsRepository = new TicketsRepository(_ticketsManagerDbContext, userRepository);
            var ticketsService = new TicketsService(_mapper, ticketsRepository, userRepository);

            //Create new user with tickets
            var user = _fixture.Create<User>();
            await _ticketsManagerDbContext.Users.AddAsync(user);
            await _ticketsManagerDbContext.SaveChangesAsync();

            //Create search request by title
            var searchRequest = new SearchTicketsRequestDto
            {
                Pagination = new PaginationRequestDto { CurrentPageNumber = 1, RecordsPerPage = 10 },
                TicketTitle = user.UserTickets.First().Title
            };
            var tickets = await ticketsService.SearchTicketsByNameAsync(user.Id, searchRequest, user.Id);

            Assert.Multiple(() =>
            {
                Assert.That(tickets, Is.Not.Null);
                Assert.That(tickets.Data.First().Title, Is.EqualTo(user.UserTickets.First().Title));
            });
        }

        [Test]
        public async Task CreateTicketAsync_CreatesNewTicket()
        {
            var userRepository = new UserRepository(_ticketsManagerDbContext);
            var ticketsRepository = new TicketsRepository(_ticketsManagerDbContext, userRepository);
            var ticketsService = new TicketsService(_mapper, ticketsRepository, userRepository);

            //Create new user without tickets
            var user = new User
            {
                Id = Guid.NewGuid()
            };
            await _ticketsManagerDbContext.Users.AddAsync(user);
            await _ticketsManagerDbContext.SaveChangesAsync();

            //Create new ticket
            var createTicketRequest = _fixture.Create<TicketCreateRequestDto>();
            var createTicket = await ticketsService.CreateTicketAsync(createTicketRequest, user.Id, user.Id);

            Assert.Multiple(() =>
            {
                Assert.That(createTicket, Is.Not.Null);
                Assert.That(createTicket.Title, Is.EqualTo(createTicketRequest.Title));
            });
        }

        [Test]
        public async Task DeleteTicketAsync_DeletesTicket()
        {
            var userRepository = new UserRepository(_ticketsManagerDbContext);
            var ticketsRepository = new TicketsRepository(_ticketsManagerDbContext, userRepository);
            var ticketsService = new TicketsService(_mapper, ticketsRepository, userRepository);

            //Create new user with tickets
            var user = _fixture.Create<User>();
            await _ticketsManagerDbContext.Users.AddAsync(user);
            await _ticketsManagerDbContext.SaveChangesAsync();

            //Save ticket for delete data
            var ticketForDelete = user.UserTickets.First();

            //Delete ticket
            await ticketsService.DeleteTicketAsync(ticketForDelete.Id, user.Id);

            //Try to search deleted ticket
            var searchRequest = new SearchTicketsRequestDto
            {
                Pagination = new PaginationRequestDto { CurrentPageNumber = 1, RecordsPerPage = 10 },
                TicketTitle = ticketForDelete.Title
            };
            var tickets = await ticketsService.SearchTicketsByNameAsync(user.Id, searchRequest, user.Id);

            Assert.That(tickets.Data, Is.Empty);
        }

        [Test]
        public async Task UpdateTicket_UpdateTicketsFields()
        {
            var userRepository = new UserRepository(_ticketsManagerDbContext);
            var ticketsRepository = new TicketsRepository(_ticketsManagerDbContext, userRepository);
            var ticketsService = new TicketsService(_mapper, ticketsRepository, userRepository);

            //Create new user with tickets
            var user = _fixture.Create<User>();
            await _ticketsManagerDbContext.Users.AddAsync(user);
            await _ticketsManagerDbContext.SaveChangesAsync();

            var notUpdatedTicket = _mapper.Map<Ticket>(_ticketsManagerDbContext.Tickets.Where(a => a.Id == user.UserTickets.First().Id).First());

            var ticketUpdate = new TicketUpdateRequestDto
            {
                Title = "New Title",
                Messages = new List<MessageResponseDto>
                {
                    new MessageResponseDto
                    {
                        Content = "Content",
                        Sender = "Sender"
                    }
                }
            };

            var updatedTicket = await ticketsService.UpdateTicketAsync(notUpdatedTicket.Id, ticketUpdate, user.Id);

            Assert.Multiple(() =>
            {
                Assert.That(updatedTicket, Is.Not.Null);
                Assert.That(notUpdatedTicket.Title, Is.Not.EqualTo(updatedTicket.Title));
            });
        }

        [Test]
        public async Task DownloadAsDocAsync_ConvertsBase64DataToDoc()
        {
            var userRepository = new UserRepository(_ticketsManagerDbContext);
            var ticketsRepository = new TicketsRepository(_ticketsManagerDbContext, userRepository);
            var ticketsService = new TicketsService(_mapper, ticketsRepository, userRepository);

            var base64Data = new[]
            {
                Convert.ToBase64String(Encoding.UTF8.GetBytes("<h1>Hello, World!</h1>")),
                Convert.ToBase64String(Encoding.UTF8.GetBytes("<p>This is a test document.</p>"))
            };

            var result = await ticketsService.DownloadAsDocAsync(base64Data);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);

            using var stream = new MemoryStream(result);
            using var package = WordprocessingDocument.Open(stream, false);
            var body = package.MainDocumentPart.Document.Body;

            Assert.That(body, Is.Not.Null);

            var paragraphs = body.Elements<Paragraph>();

            Assert.That(paragraphs, Is.Not.Null);
            Assert.That(paragraphs.Count(), Is.EqualTo(2));
        }
    }
}
