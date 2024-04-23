//using AutoFixture;
//using AutoMapper;
//using Microsoft.EntityFrameworkCore;
//using UserManager.BLL.Mapping;
//using UserManager.DAL.Context.UserContext;

//namespace UserManager.Tests.ServiceTests
//{
//    public abstract class BaseClassForServices
//    {
//        protected IMapper _mapper;
//        protected Fixture _fixture;
//        protected UserManagerDbContext _userManagerDbContext;

//        [SetUp]
//        public void Setup()
//        {
//            var options = new DbContextOptionsBuilder<UserManagerDbContext>()
//                .UseInMemoryDatabase(databaseName: "InMemoryDb")
//                .Options;

//            var mapperConfig = new MapperConfiguration(cfg =>
//            {
//                cfg.AddProfile<UserProfile>();
//            });

//            _mapper = mapperConfig.CreateMapper();

//            _fixture = new Fixture();
//            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

//            _userManagerDbContext = new UserManagerDbContext(options);
//        }

//        [TearDown]
//        public async Task TearDown()
//        {
//            await _userManagerDbContext.DisposeAsync();
//        }
//    }
//}
