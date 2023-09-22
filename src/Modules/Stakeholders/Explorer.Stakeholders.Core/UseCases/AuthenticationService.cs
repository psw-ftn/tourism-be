using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using FluentResults;

namespace Explorer.Stakeholders.Core.UseCases;

public class AuthenticationService : IAuthenticationService
{
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IUserRepository _userRepository;
    private readonly ICrudRepository<Person> _personRepository;

    public AuthenticationService(IUserRepository userRepository, ICrudRepository<Person> personRepository, ITokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
        _userRepository = userRepository;
        _personRepository = personRepository;
    }

    public Result<AuthenticationTokensDto> Login(CredentialsDto credentials)
    {
        var user = _userRepository.GetActiveByName(credentials.Username);
        if (user == null || credentials.Password != user.Password)
            return Result.Fail(FailureCode.NotFound);
            
        return _tokenGenerator.GenerateAccessToken(user, _userRepository.GetPersonId(user.Id));
    }

    public Result<AuthenticationTokensDto> RegisterTourist(AccountRegistrationDto account)
    {
        if(_userRepository.Exists(account.Email)) return Result.Fail(FailureCode.NonUniqueUsername);

        var user = _userRepository.Create(new User(account.Email, account.Password, UserRole.Tourist));
        var result = _personRepository.Create(new Person(user.Id, account.Name, account.Surname, account.Email));

        return _tokenGenerator.GenerateAccessToken(user, result.Value.Id);
    }
}