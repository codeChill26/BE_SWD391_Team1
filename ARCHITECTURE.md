# Project Architecture - Interface & Implementation Pattern

## Overview
Project s? d?ng ki?n trúc 3 t?ng v?i Interface & Implementation pattern ?? t?ng tính testability và maintainability.

```
???????????????????????????????????????????
?         Presentation Layer              ?
?         (WebAPI Controllers)            ?
???????????????????????????????????????????
               ?
               ? Uses Interfaces
               ?
???????????????????????????????????????????
?      Business Logic Layer (BLL)         ?
?                                         ?
?  ????????????????????????????????????  ?
?  ?  Interfaces/                     ?  ?
?  ?  - IOrderService                 ?  ?
?  ?  - IInventoryService             ?  ?
?  ????????????????????????????????????  ?
?               ?                         ?
?               ? Implemented by          ?
?               ?                         ?
?  ????????????????????????????????????  ?
?  ?  Services/                       ?  ?
?  ?  - OrderService                  ?  ?
?  ?  - InventoryService              ?  ?
?  ????????????????????????????????????  ?
???????????????????????????????????????????
               ?
               ? Uses Interfaces
               ?
???????????????????????????????????????????
?     Data Access Layer (DAL)             ?
?                                         ?
?  ????????????????????????????????????  ?
?  ?  Interfaces/                     ?  ?
?  ?  - IOrderRepository              ?  ?
?  ?  - IInventoryRepository          ?  ?
?  ?  - IProductRepository            ?  ?
?  ?  - IFranchiseStoreRepository     ?  ?
?  ?  - ICentralKitchenRepository     ?  ?
?  ?  - IOrderItemRepository          ?  ?
?  ????????????????????????????????????  ?
?               ?                         ?
?               ? Implemented by          ?
?               ?                         ?
?  ????????????????????????????????????  ?
?  ?  Repositories/                   ?  ?
?  ?  - OrderRepository               ?  ?
?  ?  - InventoryRepository           ?  ?
?  ?  - ProductRepository             ?  ?
?  ?  - FranchiseStoreRepository      ?  ?
?  ?  - CentralKitchenRepository      ?  ?
?  ?  - OrderItemRepository           ?  ?
?  ????????????????????????????????????  ?
???????????????????????????????????????????
               ?
               ? Uses EF Core
               ?
???????????????????????????????????????????
?          Database (PostgreSQL)          ?
???????????????????????????????????????????
```

## Layer Details

### 1. Data Access Layer (DAL)

#### Interfaces (`DataAccessLayer/Interfaces/`)
??nh ngh?a contract cho data access operations.

**IOrderRepository.cs**
```csharp
public interface IOrderRepository
{
    Task<order> CreateAsync(order order);
    Task<order?> GetByIdAsync(string orderId);
    Task<List<order>> GetByStoreIdAsync(string storeId);
    Task<bool> UpdateStatusAsync(string orderId, string status);
    Task<bool> ExistsAsync(string orderId);
}
```

#### Implementations (`DataAccessLayer/Repositories/`)
Implement các interfaces và x? lý database operations.

**OrderRepository.cs**
```csharp
public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;
    // Implementation...
}
```

**Benefits:**
- ? Tách bi?t business logic kh?i data access code
- ? D? dàng mock cho unit testing
- ? Có th? thay ??i implementation mà không ?nh h??ng business logic

---

### 2. Business Logic Layer (BLL)

#### Interfaces (`BLL/Interfaces/`)
??nh ngh?a contract cho business operations.

**IOrderService.cs**
```csharp
public interface IOrderService
{
    Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto);
    Task<OrderResponseDto> GetOrderByIdAsync(string orderId);
    Task<List<OrderResponseDto>> GetOrdersByStoreAsync(string storeId);
    Task<bool> UpdateOrderStatusAsync(string orderId, string status);
}
```

#### Implementations (`BLL/Services/`)
Implement business logic và orchestrate repositories.

**OrderService.cs**
```csharp
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IFranchiseStoreRepository _storeRepository;
    // ... other repositories
    
    public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto)
    {
        // Business validation
        // Orchestrate multiple repositories
        // Return DTO
    }
}
```

**Benefits:**
- ? Tách bi?t business logic kh?i presentation layer
- ? Reusable business logic
- ? D? dàng test business rules
- ? S? d?ng DTOs ?? tránh expose entities

---

### 3. Presentation Layer (WebAPI)

#### Controllers (`Controller/Controllers/`)
S? d?ng service interfaces ?? handle HTTP requests.

**OrderController.cs**
```csharp
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    
    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] CreateOrderDto dto)
    {
        var order = await _orderService.CreateOrderAsync(dto);
        return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
    }
}
```

**Benefits:**
- ? Thin controllers - ch? handle HTTP concerns
- ? Không ch?a business logic
- ? D? dàng test

---

## Dependency Injection Setup

T?t c? interfaces và implementations ???c ??ng ký trong `Program.cs`:

```csharp
// DAL Repositories
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IFranchiseStoreRepository, FranchiseStoreRepository>();
builder.Services.AddScoped<ICentralKitchenRepository, CentralKitchenRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// BLL Services
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
```

**Lifetime:**
- `AddScoped` - M?t instance per HTTP request
- Phù h?p cho database operations

---

## Data Flow Example

### Request Flow: Create Order

```
1. HTTP POST /api/Order
   ?
2. OrderController.CreateOrder()
   ?
3. IOrderService.CreateOrderAsync()
   ? (validates and orchestrates)
   ??? IFranchiseStoreRepository.ExistsAsync()
   ??? ICentralKitchenRepository.ExistsAsync()
   ??? IProductRepository.GetByIdsAsync()
   ??? IOrderRepository.CreateAsync()
   ??? IOrderItemRepository.CreateRangeAsync()
   ?
4. Map entities to DTOs
   ?
5. Return OrderResponseDto
```

---

## Repository Pattern Benefits

### Traditional Approach (Direct DbContext in Services)
```csharp
public class OrderService
{
    private readonly AppDbContext _context; // ? Tight coupling
    
    public async Task CreateOrderAsync(CreateOrderDto dto)
    {
        var order = new order { ... };
        _context.orders.Add(order); // ? Hard to test
        await _context.SaveChangesAsync();
    }
}
```

### Repository Pattern Approach
```csharp
public class OrderService
{
    private readonly IOrderRepository _orderRepository; // ? Loose coupling
    
    public async Task CreateOrderAsync(CreateOrderDto dto)
    {
        var order = new order { ... };
        await _orderRepository.CreateAsync(order); // ? Easy to mock
    }
}
```

---

## Testing Benefits

### Unit Testing Services
```csharp
[Test]
public async Task CreateOrder_ShouldReturnOrderDto_WhenValid()
{
    // Arrange
    var mockOrderRepo = new Mock<IOrderRepository>();
    var mockStoreRepo = new Mock<IFranchiseStoreRepository>();
    // ... setup mocks
    
    var service = new OrderService(
        mockOrderRepo.Object,
        mockStoreRepo.Object,
        ...
    );
    
    // Act
    var result = await service.CreateOrderAsync(dto);
    
    // Assert
    Assert.NotNull(result);
    mockOrderRepo.Verify(x => x.CreateAsync(It.IsAny<order>()), Times.Once);
}
```

### Integration Testing Repositories
```csharp
[Test]
public async Task GetByIdAsync_ShouldReturnOrder_WhenExists()
{
    // Arrange - Use real DbContext with in-memory database
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase("TestDb")
        .Options;
    
    using var context = new AppDbContext(options);
    var repository = new OrderRepository(context);
    
    // Act & Assert
    var order = await repository.GetByIdAsync("ORD_123");
    Assert.NotNull(order);
}
```

---

## File Structure

```
SWD392_BE/
?
??? DataAccessLayer/ (DAL)
?   ??? Interfaces/
?   ?   ??? IOrderRepository.cs
?   ?   ??? IOrderItemRepository.cs
?   ?   ??? IInventoryRepository.cs
?   ?   ??? IProductRepository.cs
?   ?   ??? IFranchiseStoreRepository.cs
?   ?   ??? ICentralKitchenRepository.cs
?   ?
?   ??? Repositories/
?   ?   ??? OrderRepository.cs
?   ?   ??? OrderItemRepository.cs
?   ?   ??? InventoryRepository.cs
?   ?   ??? ProductRepository.cs
?   ?   ??? FranchiseStoreRepository.cs
?   ?   ??? CentralKitchenRepository.cs
?   ?
?   ??? Entities/
?   ?   ??? order.cs
?   ?   ??? product.cs
?   ?   ??? ...
?   ?
?   ??? Context/
?       ??? AppDbContext.cs
?
??? BLL/
?   ??? Interfaces/
?   ?   ??? IOrderService.cs
?   ?   ??? IInventoryService.cs
?   ?
?   ??? Services/
?   ?   ??? OrderService.cs
?   ?   ??? InventoryService.cs
?   ?
?   ??? DTOs/
?       ??? Order/
?       ?   ??? CreateOrderDto.cs
?       ?   ??? OrderResponseDto.cs
?       ??? Inventory/
?           ??? InventoryItemDto.cs
?
??? Controller/ (WebAPI)
    ??? Controllers/
    ?   ??? OrderController.cs
    ?   ??? InventoryController.cs
    ?
    ??? Program.cs (DI Configuration)
```

---

## Key Principles

### 1. **Separation of Concerns**
- Controllers: HTTP handling
- Services: Business logic
- Repositories: Data access

### 2. **Dependency Inversion Principle (SOLID)**
- High-level modules (Services) không ph? thu?c vào low-level modules (Repositories)
- C? hai ??u ph? thu?c vào abstractions (Interfaces)

### 3. **Interface Segregation**
- M?i interface ch? ch?a methods c?n thi?t
- Không ép clients implement methods không dùng ??n

### 4. **Single Responsibility**
- M?i repository ch?u trách nhi?m cho 1 entity
- M?i service ch?u trách nhi?m cho 1 business domain

---

## Summary

? **DAL Layer:** 6 Interfaces + 6 Implementations (Repositories)
? **BLL Layer:** 2 Interfaces + 2 Implementations (Services)
? **WebAPI Layer:** 2 Controllers s? d?ng Service Interfaces
? **Dependency Injection:** T?t c? dependencies ???c inject qua constructor
? **Testability:** D? dàng mock và unit test
? **Maintainability:** Thay ??i implementation không ?nh h??ng interface
