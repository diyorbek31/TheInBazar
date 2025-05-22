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

        [Fact]
        public async Task OnPostUser_WhenValidData_ShouldCreateUser()
        {
            var client = _factory.CreateClient();
            var newUser = new User()
            {
                Firstname = "Asadbek",
                Lastname = "Adhamov",
                Email = "asad1106",
                Password = "1234"
            };

            var response = await client.PostAsJsonAsync("api/users", newUser);
            response.EnsureSuccessStatusCode();

            // firts checking is it avaiable this kind of user or not
            var db = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
            var savedUser = db.Users.FirstOrDefault(u => u.Email == "asad1106");

            savedUser.Should().NotBeNull();
            savedUser!.Firstname.Should().Be("Asadbek");
        }
        [Fact]
        public async Task OnPutUser_WhenValidUpdate_ModifyNewUser()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Users.Add(new User 
                { 
                    Id = 1,
                    Firstname = "Diyorbek",
                    Lastname = "Juraev", 
                    Email = "newemail@me.com",
                    Password = "passwordold" 
                });
                db.SaveChanges();
            }
            var newUser = new User()
            {
                Id = 1,
                Firstname = "Omina",
                Lastname = "Bahodirova",
                Email = "new1221",
                Password = "1234"
            };

            var client = _factory.CreateClient();

            var response = await client.PutAsJsonAsync("api/users",newUser);
            response.EnsureSuccessStatusCode();

            var scope2 = _factory.Services.CreateScope();
            var db2 = scope2.ServiceProvider.GetRequiredService<AppDbContext>();
            var user = db2.Users.First(u => u.Id == 1);
            user.Firstname.Should().Be("Omina");
        }
        [Fact]
        public async Task OnDeleteUser_WhenValidId_ShouldDeleteUser()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Users.Add(new User
                {
                    Id = 15,
                    Firstname = "Nozimjon",
                    Lastname = "Usmonaliyev",
                    Email = "deleted@me.com",
                    Password = "blablabla"
                });
                db.SaveChanges();
            }
            var client = _factory.CreateClient();

            var response = await client.DeleteAsync("api/users/15");
            response.EnsureSuccessStatusCode();

            using(var scope2 = _factory.Services.CreateScope())
            {
                var db2 = scope2.ServiceProvider.GetRequiredService<AppDbContext>();
                var user = db2.Users.FirstOrDefault(u => u.Id == 15);
                user.Should().BeNull(); // it should be deleted
            }
            
        }
    }
}
