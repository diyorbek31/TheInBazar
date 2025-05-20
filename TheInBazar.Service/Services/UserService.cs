using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TheInBazar.Data.IRepositories;
using TheInBazar.Domain.Entities;
using TheInBazar.Service.CustomExceptions;
using TheInBazar.Service.DTOs;
using TheInBazar.Service.Interfaces;

namespace TheInBazar.Service.Services;

public class UserService : IUserService
{
    private readonly IMapper mapper;
    private readonly IRepository<User> userRepository;
    public UserService(IMapper mapper,IRepository<User> userRepository)
    {
        this.mapper = mapper;
        this.userRepository = userRepository;   
    }
    public async Task<UserForResultDto> CreateAsync(UserForCreationDto dto)
    {
        var user = await this.userRepository.SelectAll().
            FirstOrDefaultAsync(u => u.Firstname.ToLower() == dto.Firstname.ToLower());
        if (user is not null)
            throw new CustomException(409, "User already exists");

        var mappedUser = this.mapper.Map<User>(dto);
        mappedUser.CreatedAt = DateTime.UtcNow;
        var result = await this.userRepository.InsertAsync(mappedUser);
        await this.userRepository.SaveChangeAsync();

        return this.mapper.Map<UserForResultDto>(result);
    }

    public async Task<bool> RemoveAsync(long id)
    {
        var user = await this.userRepository.SelectByIdAsync(id);
        if (user is null)
            throw new CustomException(404, "User is not found");
        await this.userRepository.DeleteAsync(id);
        await this.userRepository.SaveChangeAsync();
        return true;
    }

    public async Task<IEnumerable<UserForResultDto>> RetrieveAllAsync()
    {
        var users = await this.userRepository.SelectAll().ToListAsync();

        return this.mapper.Map<IEnumerable<UserForResultDto>>(users);
    }

    public async Task<UserForResultDto> RetrieveByIdAsync(long id)
    {
        var user = await this.userRepository.SelectByIdAsync(id);
        if (user is null)
            throw new CustomException(404, "User is not found");
        
        return this.mapper.Map<UserForResultDto>(user);
    }

    public async Task<UserForResultDto> UpdateAsync(UserForUpdateDto dto)
    {
        var user = await this.userRepository.SelectByIdAsync(dto.Id);
        if (user is null)
            throw new CustomException(404, "User is not found");


        this.mapper.Map(dto, user);
        user.UpdatedAt = DateTime.Now;
        await this.userRepository.UpdateAsync(user);
        await this.userRepository.SaveChangeAsync();

        return this.mapper.Map<UserForResultDto>(user);
    }
}
