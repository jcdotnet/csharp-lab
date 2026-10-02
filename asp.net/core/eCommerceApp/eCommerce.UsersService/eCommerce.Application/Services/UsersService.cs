using AutoMapper;
using eCommerce.Application.DTO;
using eCommerce.Application.RepositoryContracts;
using eCommerce.Application.ServiceContracts;
using eCommerce.Domain.Entities;

namespace eCommerce.Application.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;

        public UsersService(IUsersRepository usersRepository, IMapper mapper)
        {
            _usersRepository = usersRepository;
            _mapper = mapper;
        }

        public async Task<UserDto?> GetUser(Guid userId)
        {
            ApplicationUser? fromRepo = await _usersRepository.GetUserByUserId(userId);

            // mapping from ApplicationUser to UserDto (automapper)
            return _mapper.Map<UserDto>(fromRepo); 
        }

        public async Task<AuthenticationResponse?> Login(LoginRequest loginRequest)
        {
            ApplicationUser? fromRepo = await _usersRepository.GetUserByEmailAndPassword(
                loginRequest.Email, 
                loginRequest.Password
            );
            if (fromRepo is null) return null;

            // mapping from ApplicationUser to AuthenticationResponse
            //return new AuthenticationResponse(
            //    fromRepo.UserId, fromRepo.Email, fromRepo.UserName, fromRepo.Gender,
            //    Token: "token",
            //    Success: true
            //);
            // using AutoMapper instead
            return _mapper.Map<AuthenticationResponse>( fromRepo ) with
            {
                Token = "token", Success = true,
            };
        }

        public async Task<AuthenticationResponse?> Register(RegisterRequest registerRequest)
        {
            //ApplicationUser user = new()
            //{
            //    UserName = registerRequest.UserName,
            //    Email = registerRequest.Email,
            //    Password = registerRequest.Password,
            //    Gender = registerRequest.Gender.ToString(),
            //};
            // using AutoMapper instead
            ApplicationUser user = _mapper.Map<ApplicationUser>(registerRequest);
            ApplicationUser? fromRepo = await _usersRepository.AddUser(user);
            if (fromRepo is null) return null;
            
            //return new AuthenticationResponse(
            //    fromRepo.UserId, fromRepo.Email, fromRepo.UserName, fromRepo.Gender,
            //    Token: "token",
            //    Success: true
            //);
            // using AutoMapper instead
            return _mapper.Map<AuthenticationResponse>(fromRepo) with
            {
                Token = "token",
                Success = true,
            };
        }
    }
}
