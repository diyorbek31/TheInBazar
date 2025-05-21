using TheInBazar.Api;
using TheInBazar.Data.DbContexts;
using TheInBazar.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http.Json;
using Xunit;

namespace TheInBazar.Test.Controllers
{
    public class InMemoryDatabase
    {
        private readonly WebApplicationFactory<Program> _factory;

        public InMemoryDatabase()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");

                    builder.ConfigureTestServices(services =>
                    {
                        services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                        services.RemoveAll(typeof(AppDbContext));

                        var provider = new ServiceCollection()
                            .AddEntityFrameworkInMemoryDatabase()
                            .BuildServiceProvider();

                        services.AddDbContext<AppDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("TestDb")
                                   .UseInternalServiceProvider(provider);
                        });
                    });
                });
        }

        [Fact]
        public async Task OnGetUsers_WhenExecuteApi_ShouldReturnExpectedUsers()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedService = scope.ServiceProvider;
                var dbContext = scopedService.GetRequiredService<AppDbContext>();

                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();

                dbContext.Users.Add(new User()
                {
                    Id = 1,
                    Firstname = "Diyorbek",
                    Lastname = "Juraev",
                    Email = "31diyor",
                    Password = "3105"
                });

                dbContext.SaveChanges();
            }

            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("api/users");
            var result = await response.Content.ReadFromJsonAsync<Response<List<User>>>();

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(200);
            result.Data.Should().NotBeNull();
            result.Data.Count.Should().Be(1);

            var user = result.Data[0];
            user.Firstname.Should().Be("Diyorbek");
            user.Lastname.Should().Be("Juraev");
            user.Email.Should().Be("31diyor");
        }
    }
}
