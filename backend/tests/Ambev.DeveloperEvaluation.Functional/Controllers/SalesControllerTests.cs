using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using AutoFixture;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using System.Net.Http.Headers;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature;

namespace Ambev.DeveloperEvaluation.Functional.Controllers;

public class SalesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private const string email = "admin@ambev.com";
    private const string password = "P@ssw0rd123";
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly IServiceProvider _serviceProvider;
    private readonly List<Guid> _createdSaleIds = new();
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    private readonly IFixture _fixture;

    public SalesControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        _serviceProvider = _factory.Services;

        _fixture = new Fixture();

        // Configure auth token
        SetupUserAuthentication().Wait();
        SetupAuthentication().Wait();
    }

    private async Task SetupUserAuthentication()
    {
        // Ensure the database is seeded with a user
        var request = _fixture.Build<CreateUserRequest>()
            .With(u => u.Email, email)
            .With(u => u.Password, password)
            .With(u => u.Role, UserRole.Admin)
            .With(u => u.Status, UserStatus.Active)
            .With(u => u.Username, "Admin User")
            .With(u => u.Phone, "10912345678")
            .Create();

        var response = await _client.PostAsJsonAsync("api/Users", request);
    }
    private async Task SetupAuthentication()
    {
        var request = _fixture.Build<AuthenticateUserRequest>()
            .With(u => u.Email, email)
            .With(u => u.Password, password)
            .Create();

        // Login to get token
        var loginResponse = await _client.PostAsJsonAsync("api/auth", request);
        if (loginResponse.IsSuccessStatusCode)
        {
            var authResponse = await loginResponse.Content.ReadFromJsonAsync<ApiResponseWithData<AuthResponse>>();
            if (authResponse != null && authResponse.Data != null)
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", authResponse.Data.Token);
            }
        }
    }

    private async Task DisposeAsync()
    {
        // Cleanup - delete all created sales
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DefaultContext>();

        foreach (var saleId in _createdSaleIds)
        {
            var sale = dbContext.Sales.Include(s => s.Items).FirstOrDefault(s => s.Id == saleId);
            if (sale != null)
            {
                dbContext.SaleItems.RemoveRange(sale.Items);
                dbContext.Sales.Remove(sale);
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private async Task<Guid> CreateTestSaleAsync(string? customerName = null, string? branchName = null)
    {
        var requestSaleItens = _fixture.Build<CreateSaleItemRequest>()
            .With(x => x.Quantity, _fixture.Create<int>() % 5 + 4)
            .CreateMany(2)
            .ToList();

        var request = _fixture.Build<CreateSaleRequest>()
            .With(x => x.CustomerName, customerName ?? _fixture.Create<string>())
            .With(x => x.BranchName, branchName ?? _fixture.Create<string>())
            .With(x => x.Items, requestSaleItens)
            .Create();

        var response = await _client.PostAsJsonAsync("api/Sales", request);

        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        Assert.NotNull(responseData);
        Assert.NotNull(responseData.Data);

        var saleId = responseData.Data.Id;
        _createdSaleIds.Add(saleId);

        return responseData.Data.Id;
    }

    #region Tests

    [Fact(DisplayName = "POST /api/Sales should create a new sale")]
    public async Task CreateSale_ShouldCreateNewSale()
    {
        // Arrange
        var requestSaleItens = _fixture.Build<CreateSaleItemRequest>()
            .With(x => x.Quantity, _fixture.Create<int>() % 5 + 4)
            .With(x => x.UnitPrice, _fixture.Create<decimal>() % 100 + 1)
            .CreateMany(2)
            .ToList();

        var request = _fixture.Build<CreateSaleRequest>()
            .With(x => x.Items, requestSaleItens)
            .Create();

        // Act
        var response = await _client.PostAsJsonAsync("api/Sales", request);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseData = await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        Assert.NotNull(responseData);
        Assert.True(responseData.Success);
        Assert.NotNull(responseData.Data);
        Assert.NotEqual(Guid.Empty, responseData.Data.Id);
        Assert.NotEmpty(responseData.Data.SaleNumber);
        Assert.Equal(request.CustomerName, responseData.Data.CustomerName);
        Assert.Equal(request.BranchName, responseData.Data.BranchName);
        Assert.Equal(request.Items.Count, responseData.Data.Items.Count);

        // Verify discount was applied (10% for 5 items)
        var expectedDiscount = request.Items[0].Quantity * request.Items[0].UnitPrice * 0.1m;
        Assert.Equal(expectedDiscount, responseData.Data.Items[0].Discount);

        // Save ID for cleanup
        _createdSaleIds.Add(responseData.Data.Id);
    }

    [Fact(DisplayName = "POST /api/Sales should apply 10% discount for 4-9 items")]
    public async Task CreateSale_ShouldApply10PercentDiscount_For4To9Items()
    {
        // Arrange
        var requestSaleItens = _fixture.Build<CreateSaleItemRequest>()
            .With(x => x.Quantity, 5)
            .With(x => x.UnitPrice, _fixture.Create<decimal>() % 100 + 1)
            .CreateMany(1)
            .ToList();

        var request = _fixture.Build<CreateSaleRequest>()
            .With(x => x.Items, requestSaleItens)
            .Create();

        // Act
        var response = await _client.PostAsJsonAsync("api/Sales", request);

        // Assert
        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        Assert.NotNull(responseData);
        Assert.NotNull(responseData.Data);

        // Verify 10% discount was applied
        var expectedDiscount = request.Items[0].Quantity * request.Items[0].UnitPrice * 0.1m;
        Assert.Equal(expectedDiscount, responseData.Data.Items[0].Discount);

        // Verify total amount is correct (item cost - discount)
        var expectedTotal = (request.Items[0].Quantity * request.Items[0].UnitPrice) - expectedDiscount;
        Assert.Equal(expectedTotal, responseData.Data.Items[0].TotalAmount);

        // Save ID for cleanup
        _createdSaleIds.Add(responseData.Data.Id);
    }

    [Fact(DisplayName = "POST /api/Sales should apply 20% discount for 10-20 items")]
    public async Task CreateSale_ShouldApply20PercentDiscount_For10To20Items()
    {
        // Arrange
        var requestSaleItens = _fixture.Build<CreateSaleItemRequest>()
            .With(x => x.Quantity, 15)
            .With(x => x.UnitPrice, _fixture.Create<decimal>() % 100 + 1)
            .CreateMany(1)
            .ToList();

        var request = _fixture.Build<CreateSaleRequest>()
            .With(x => x.Items, requestSaleItens)
            .Create();

        // Act
        var response = await _client.PostAsJsonAsync("api/Sales", request);

        // Assert
        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        Assert.NotNull(responseData);
        Assert.NotNull(responseData.Data);

        // Verify 20% discount was applied
        var expectedDiscount = request.Items[0].Quantity * request.Items[0].UnitPrice * 0.2m;
        Assert.Equal(expectedDiscount, responseData.Data.Items[0].Discount);

        // Verify total amount is correct (item cost - discount)
        var expectedTotal = (request.Items[0].Quantity * request.Items[0].UnitPrice) - expectedDiscount;
        Assert.Equal(expectedTotal, responseData.Data.Items[0].TotalAmount);

        // Save ID for cleanup
        _createdSaleIds.Add(responseData.Data.Id);
    }

    [Fact(DisplayName = "POST /api/Sales should not apply discount for fewer than 4 items")]
    public async Task CreateSale_ShouldNotApplyDiscount_ForFewerThan4Items()
    {
        // Arrange
        var requestSaleItens = _fixture.Build<CreateSaleItemRequest>()
            .With(x => x.Quantity, 3)
            .With(x => x.UnitPrice, _fixture.Create<decimal>() % 100 + 1)
            .CreateMany(1)
            .ToList();

        var request = _fixture.Build<CreateSaleRequest>()
            .With(x => x.Items, requestSaleItens)
            .Create();

        // Act
        var response = await _client.PostAsJsonAsync("api/Sales", request);

        // Assert
        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        Assert.NotNull(responseData);
        Assert.NotNull(responseData.Data);

        // Verify no discount was applied
        Assert.Equal(0, responseData.Data.Items[0].Discount);

        // Save ID for cleanup
        _createdSaleIds.Add(responseData.Data.Id);
    }

    [Fact(DisplayName = "POST /api/Sales should reject requests with more than 20 items")]
    public async Task CreateSale_ShouldRejectRequest_WithMoreThan20Items()
    {
        // Arrange
        var requestSaleItens = _fixture.Build<CreateSaleItemRequest>()
            .With(x => x.Quantity, _fixture.Create<int>() % 5 + 21)
            .CreateMany(1)
            .ToList();

        var request = _fixture.Build<CreateSaleRequest>()
            .With(x => x.Items, requestSaleItens)
            .Create();

        // Act
        var response = await _client.PostAsJsonAsync("api/Sales", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "GET /api/Sales/{id} should return a sale")]
    public async Task GetSale_ShouldReturnSale()
    {

        // Arrange - Create a sale to retrieve
        var saleId = await CreateTestSaleAsync("Test Customer", "Test Branch");

        // Act
        var response = await _client.GetAsync($"api/Sales/{saleId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<ApiResponseWithData<GetSaleResponse>>();

        Assert.NotNull(responseData);
        Assert.True(responseData.Success);
        Assert.NotNull(responseData.Data);
        Assert.Equal(saleId, responseData.Data.Id);
        Assert.Equal("Test Customer", responseData.Data.CustomerName);
        Assert.Equal("Test Branch", responseData.Data.BranchName);
        Assert.Equal(2, responseData.Data.Items.Count);
        Assert.Contains(responseData.Data.Items, i => i.ProductName.StartsWith("ProductName"));
    }

    [Fact(DisplayName = "GET /api/Sales/{id} should return 404 for non-existent sale")]
    public async Task GetSale_ShouldReturn404_ForNonExistentSale()
    {
        // Act
        var response = await _client.GetAsync($"api/Sales/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "PUT /api/Sales/{id} should update a sale")]
    public async Task UpdateSale_ShouldUpdateSale()
    {
        // Arrange - Create a sale to update
        var saleId = await CreateTestSaleAsync();

        var requestSaleItens = _fixture.Build<UpdateSaleItemRequest>()
            .With(x => x.Quantity, 10)
            .With(x => x.UnitPrice, 50.00m)
            .With(x => x.ProductName, "Updated Product")
            .CreateMany(1)
            .ToList();

        var request = _fixture.Build<UpdateSaleRequest>()
            .With(x => x.CustomerName, "Updated Customer")
            .With(x => x.BranchName, "Updated Branch")
            .With(x => x.Items, requestSaleItens)
            .Create();

        // Act
        var response = await _client.PutAsJsonAsync($"api/Sales/{saleId}", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<ApiResponseWithData<UpdateSaleResponse>>();

        Assert.NotNull(responseData);
        Assert.True(responseData.Success);
        Assert.NotNull(responseData.Data);
        Assert.Equal(saleId, responseData.Data.Id);
        Assert.Equal("Updated Customer", responseData.Data.CustomerName);
        Assert.Equal("Updated Branch", responseData.Data.BranchName);
        Assert.Single(responseData.Data.Items);
        Assert.Equal("Updated Product", responseData.Data.Items[0].ProductName);

        // Verify 20% discount was applied for 10 items
        var expectedDiscount = 10 * 50.00m * 0.2m;
        Assert.Equal(expectedDiscount, responseData.Data.Items[0].Discount);
    }

    [Fact(DisplayName = "PUT /api/Sales/{id} should return 404 for non-existent sale")]
    public async Task UpdateSale_ShouldReturn404_ForNonExistentSale()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var requestSaleItens = _fixture.Build<UpdateSaleItemRequest>()
            .With(x => x.Quantity, _fixture.Create<int>() % 5 + 4)
            .CreateMany(2)
            .ToList();

        var request = _fixture.Build<UpdateSaleRequest>()
            .With(x => x.Id, nonExistentId)
            .With(x => x.Items, requestSaleItens)
            .Create();

        var content = new StringContent(
            JsonSerializer.Serialize(request, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PutAsync($"api/Sales/{nonExistentId}", content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "DELETE /api/Sales/{id} should cancel a sale")]
    public async Task DeleteSale_ShouldCancelSale()
    {
        // Arrange - Create a sale to delete
        var saleId = await CreateTestSaleAsync();

        // Act
        var response = await _client.DeleteAsync($"api/Sales/{saleId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<ApiResponse>();

        Assert.NotNull(responseData);
        Assert.True(responseData.Success);

        // Verify the sale status is now Cancelled
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        var sale = await dbContext.Sales.FindAsync(saleId);

        Assert.NotNull(sale);
        Assert.Equal(SaleStatus.Cancelled, sale.Status);
    }

    [Fact(DisplayName = "DELETE /api/Sales/{id} should return 404 for non-existent sale")]
    public async Task DeleteSale_ShouldReturn404_ForNonExistentSale()
    {
        // Act
        var response = await _client.DeleteAsync($"api/Sales/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "GET /api/Sales should return paginated list of sales")]
    public async Task GetSales_ShouldReturnPaginatedList()
    {
        // Arrange - Create several sales
        for (int i = 0; i < 3; i++)
        {
            await CreateTestSaleAsync();
        }

        // Act
        var response = await _client.GetAsync("api/Sales?page=1&pageSize=2");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseData = JsonSerializer.Deserialize<PaginatedResponse<GetSalesItemResponse>>(
            responseContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(responseData);
        Assert.True(responseData.Success);
        Assert.Equal(1, responseData.CurrentPage);
        Assert.Equal(2, responseData.Data?.Count());
        Assert.True(responseData.TotalCount >= 3);
    }
    #endregion

    #region Load Tests
    [Fact(DisplayName = "LOAD /api/Sales should handle concurrent requests")]
    public async Task SimpleLoadTest_ShouldHandleConcurrentRequests()
    {
        int concurrentUsers = 40;
        var tasks = new List<Task>();

        for (int i = 0; i < concurrentUsers; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                var request = _fixture.Build<CreateSaleRequest>()
                    .With(x => x.Items, _fixture.Build<CreateSaleItemRequest>()
                        .With(x => x.Quantity, _fixture.Create<int>() % 5 + 4)
                        .CreateMany(2)
                        .ToList()
                    )
                    .Create();

                var response = await _client.PostAsJsonAsync("api/Sales", request);
                response.EnsureSuccessStatusCode();
            }));
        }

        await Task.WhenAll(tasks);
    }

    [Fact(DisplayName = "LOAD /api/Sales should handle high load")]
    public async Task HighLoadTest_ShouldHandleHighLoad()
    {
        // Arrange - Create several sales
        for (int i = 0; i < 20; i++)
        {
            await CreateTestSaleAsync();
        }

        var scenario = Scenario.Create("HighLoadTest", async context =>
        {
            var request =
                Http.CreateRequest("GET", "api/Sales?page=1&pageSize=10")
                    .WithHeader("Content-Type", "application/json");

            var response = await Http.Send(_client, request);

            return response;
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.Inject(rate: 10, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30))
        );

        var result = NBomberRunner
            .RegisterScenarios(scenario)
            .Run();

        var scnStats = result.ScenarioStats.Find("HighLoadTest");
        Assert.NotNull(scnStats);

        Assert.True(result.AllBytes > 0);
        Assert.True(result.AllRequestCount > 0);
        Assert.True(result.AllOkCount > 0);
        Assert.True(result.AllFailCount == 0);

        Assert.True(scnStats.Ok.Request.RPS > 0);
        Assert.True(scnStats.Ok.Request.Count > 0);

        // success rate 100% of all requests
        Assert.True(scnStats.Ok.Request.Percent == 100);
        Assert.True(scnStats.Fail.Request.Percent == 0);

        Assert.True(scnStats.Ok.Latency.MinMs > 0);
        Assert.True(scnStats.Ok.Latency.MaxMs > 0);

        Assert.True(scnStats.Fail.Request.Count == 0);
        Assert.True(scnStats.Fail.Latency.MinMs == 0);

        Assert.True(scnStats.Ok.Latency.Percent50 > 0);
        Assert.True(scnStats.Ok.Latency.Percent75 > 0);
        Assert.True(scnStats.Ok.Latency.Percent99 > 0);
    }
    #endregion

    // Helper class for auth response
    private class AuthResponse
    {
        public string? Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}