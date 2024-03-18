//using AutoFixture;
//using AutoMapper;
//using Microsoft.EntityFrameworkCore;
//using TicketsManager.BLL.Mapping;
//using TicketsManager.DAL.Context;
//using TicketsManager.DAL.Models;

//namespace TicketsManager.Tests.Services
//{
//    public abstract class TestServiceBase
//    {
//        protected IMapper _mapper;
//        protected TicketsManagerDbContext _ticketsManagerDbContext;
//        protected Fixture _fixture;

//        [SetUp]
//        public void Setup()
//        {
//            var options = new DbContextOptionsBuilder<TicketsManagerDbContext>()
//                .UseInMemoryDatabase(databaseName: "InMemoryDb")
//                .Options;

//            var mapperConfig = new MapperConfiguration(cfg =>
//            {
//                cfg.AddProfile<TicketProfile>();
//                cfg.AddProfile<MessageProfile>();
//                cfg.AddProfile<NotificationProfile>();
//                cfg.AddProfile<TicketSummaryProfile>();
//                cfg.AddProfile<TicketUsecaseProfile>();
//                cfg.CreateMap<Ticket, Ticket>();
//            });

//            _mapper = mapperConfig.CreateMapper();
//            _ticketsManagerDbContext = new TicketsManagerDbContext(options);
//            _fixture = new Fixture();
//            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
//        }

//        [TearDown]
//        public async Task TearDown()
//        {
//            await _ticketsManagerDbContext.DisposeAsync();
//        }
//    }
//}
