using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.DeleteUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // === Request -> Command mappings ===
        CreateMap<CreateUserRequest, CreateUserCommand>();

        // GetUser request -> command mappings
        CreateMap<GetUserRequest, GetUserCommand>()
            .ConstructUsing(src => new GetUserCommand(src.Id));

        // DeleteUser request -> command mappings
        CreateMap<DeleteUserRequest, DeleteUserCommand>()
            .ConstructUsing(src => new DeleteUserCommand(src.Id));

        // === Result -> Response mappings ===

        // CreateUser result -> response mappings
        CreateMap<CreateUserResult, CreateUserResponse>();

        // GetUser result -> response mappings
        CreateMap<GetUserResult, GetUserResponse>();
    }
}