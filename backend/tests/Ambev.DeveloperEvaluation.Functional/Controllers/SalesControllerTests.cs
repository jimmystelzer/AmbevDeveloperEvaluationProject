using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Controllers;

public class SalesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly IServiceProvider _serviceProvider;
    private readonly List<Guid> _createdSaleIds = new();
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public SalesControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        _serviceProvider = _factory.Services;
        
        // Configure auth token
        // SetupAuthentication().Wait();
    }

    // private async Task SetupAuthentication()
    // {
    //     // Login to get token
    //     var loginResponse = await _client.PostAsJsonAsync("api/auth/login", new
    //     {
    //         Email = "admin@ambev.com",
    //         Password = "P@ssw0rd123"
    //     });

    //     if (loginResponse.IsSuccessStatusCode)
    //     {
    //         var authResponse = await loginResponse.Content.ReadFromJsonAsync<ApiResponseWithData<AuthResponse>>();
    //         if (authResponse != null && authResponse.Data != null)
    //         {
    //             _client.DefaultRequestHeaders.Authorization = 
    //                 new AuthenticationHeaderValue("Bearer", authResponse.Data.Token);
    //         }
    //     }
    // }

    private void Dispose()
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
        
        dbContext.SaveChanges();
    }

    private async Task<Guid> CreateTestSale()
    {
        var request = new CreateSaleRequest
        {
            Date = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Test Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Test Branch",
            Items = new List<CreateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Product A",
                    Quantity = 5,
                    UnitPrice = 100.00m
                },
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Product B",
                    Quantity = 12,
                    UnitPrice = 75.00m
                }
            }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        var response = await _client.PostAsync("api/Sales", content);
        response.EnsureSuccessStatusCode();
        
        var responseData = await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        Assert.NotNull(responseData);
        Assert.NotNull(responseData.Data);
        
        var saleId = responseData.Data.Id;
        _createdSaleIds.Add(saleId);
        
        return saleId;
    }

    #region Tests

    [Fact(DisplayName = "POST /api/Sales should create a new sale")]
    public async Task CreateSale_ShouldCreateNewSale()
    {
        // Arrange
        var request = new CreateSaleRequest
        {
            Date = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "New Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "New Branch",
            Items = new List<CreateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Test Product",
                    Quantity = 5,
                    UnitPrice = 100.00m
                }
            }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("api/Sales", content);

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
        var request = new CreateSaleRequest
        {
            Date = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Discount Test Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Discount Test Branch",
            Items = new List<CreateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Discount Test Product",
                    Quantity = 5,
                    UnitPrice = 100.00m
                }
            }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("api/Sales", content);

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
        var request = new CreateSaleRequest
        {
            Date = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Discount Test Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Discount Test Branch",
            Items = new List<CreateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Discount Test Product",
                    Quantity = 15,
                    UnitPrice = 100.00m
                }
            }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("api/Sales", content);

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
        var request = new CreateSaleRequest
        {
            Date = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "No Discount Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "No Discount Branch",
            Items = new List<CreateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "No Discount Product",
                    Quantity = 3,
                    UnitPrice = 100.00m
                }
            }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("api/Sales", content);

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
        var request = new CreateSaleRequest
        {
            Date = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Too Many Items Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Too Many Items Branch",
            Items = new List<CreateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Too Many Items Product",
                    Quantity = 21,  // Exceeds maximum of 20
                    UnitPrice = 100.00m
                }
            }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("api/Sales", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "GET /api/Sales/{id} should return a sale")]
    public async Task GetSale_ShouldReturnSale()
    {
        // Arrange - Create a sale to retrieve
        var saleId = await CreateTestSale();

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
        Assert.Contains(responseData.Data.Items, i => i.ProductName == "Product A");
        Assert.Contains(responseData.Data.Items, i => i.ProductName == "Product B");
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
        var saleId = await CreateTestSale();
        
        var request = new UpdateSaleRequest
        {
            Id = saleId,
            Date = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Updated Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Updated Branch",
            Items = new List<UpdateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Updated Product",
                    Quantity = 10,
                    UnitPrice = 50.00m
                }
            }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PutAsync($"api/Sales/{saleId}", content);

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
        var request = new UpdateSaleRequest
        {
            Id = nonExistentId,
            Date = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Updated Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Updated Branch",
            Items = new List<UpdateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Updated Product",
                    Quantity = 5,
                    UnitPrice = 100.00m
                }
            }
        };

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
        var saleId = await CreateTestSale();

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
            await CreateTestSale();
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

    // Helper class for auth response
    private class AuthResponse
    {
        public string? Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}