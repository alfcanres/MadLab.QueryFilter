# MadLab.QueryFilter: Dynamic Query Filtering for EF Core

## Overview

**MadLab.QueryFilter** is a lightweight C# library designed to make dynamic filtering of Entity Framework Core (EF Core) queries clean, modular, and maintainable. Using the Decorator Pattern, it enables developers to stack and compose query filters at runtime, supporting a scalable repository and service architecture. It natively supports SQLite but can be adapted to other databases supported by EF Core.

---

## Why Use MadLab.QueryFilter?

In typical EF Core projects, applying dynamic filters (such as user-selected search criteria) often leads to large, hard-to-maintain queries with many conditional statements. MadLab.QueryFilter solves this by letting you define small, reusable filter components that can be combined at runtime—making complex query logic easier to manage, test, and extend.

---

## Key Concepts

### 1. The Decorator Pattern

MadLab.QueryFilter leverages the Decorator Pattern, meaning each filter "wraps" a query and can add its own filtering logic. This approach allows you to:

- Add or remove filters dynamically
- Compose multiple filters together
- Keep filter logic isolated and reusable

### 2. Clean Repository and Service Architecture

The library encourages separating concerns through repositories (for data access) and services (for business logic), making your codebase more maintainable and testable.

### 3. EF Core and SQLite

The library is built for EF Core, using LINQ expressions for filtering, and includes support for SQLite, which is ideal for lightweight and embedded scenarios.

---

## How to Use MadLab.QueryFilter

### Prerequisites

- Basic understanding of C# and .NET
- Familiarity with Entity Framework Core, including DbContext and IQueryable
- Knowledge of LINQ expressions
- (Optional) Understanding of the Repository and Service patterns

### 1. Install the Library

Assuming the package is published on NuGet:

```shell
dotnet add package MadLab.QueryFilter
```

### 2. Define Your Filters

Create filter classes implementing a shared interface (usually something like `IQueryFilter<T>`), each adding its own logic to the query:

```csharp name=ProductCategoryFilter.cs
public class ProductCategoryFilter : IQueryFilter<Product>
{
    private readonly string _category;

    public ProductCategoryFilter(string category)
    {
        _category = category;
    }

    public IQueryable<Product> Apply(IQueryable<Product> query)
    {
        return query.Where(p => p.Category == _category);
    }
}
```

### 3. Compose Filters at Runtime

In your repository or service, build up a list of filters based on the user's input:

```csharp name=ProductRepository.cs
public IQueryable<Product> GetFilteredProducts(IEnumerable<IQueryFilter<Product>> filters)
{
    IQueryable<Product> query = _context.Products;

    foreach (var filter in filters)
    {
        query = filter.Apply(query);
    }

    return query;
}
```

### 4. Using in Practice

Suppose you want to filter products by both category and price range:

```csharp name=UsageExample.cs
var filters = new List<IQueryFilter<Product>>
{
    new ProductCategoryFilter("Books"),
    new ProductPriceRangeFilter(minPrice: 10, maxPrice: 50)
};

var products = repository.GetFilteredProducts(filters).ToList();
```

---

## What Do I Need to Understand to Use This Library?

1. **Entity Framework Core Basics**
   - How to define DbContext and DbSet
   - How IQueryable and LINQ queries work

2. **The Decorator Pattern**
   - Understanding how to wrap and chain functionality

3. **Repository and Service Patterns**
   - Separating data access from business logic

4. **C# Interfaces and Generics**
   - Implementing and using interfaces (e.g., `IQueryFilter<T>`)
   - Using generics for reusable components

---

## Example Project Structure

```
MyApp/
 ├── Filters/
 │    ├── ProductCategoryFilter.cs
 │    └── ProductPriceRangeFilter.cs
 ├── Repositories/
 │    └── ProductRepository.cs
 ├── Services/
 │    └── ProductService.cs
 ├── Models/
 │    └── Product.cs
 └── Program.cs
```

---

## Final Thoughts

MadLab.QueryFilter is ideal for projects where you need flexible, dynamic query composition in EF Core. It encourages clean, maintainable code by isolating filter logic and leveraging proven design patterns.

If you're comfortable with EF Core, LINQ, and basic C# design patterns, you should be able to integrate MadLab.QueryFilter into your application with ease!

---

**Tip:** For advanced scenarios, consider writing custom filters that combine multiple conditions, or extending the pattern to support sorting and paging as well.

---