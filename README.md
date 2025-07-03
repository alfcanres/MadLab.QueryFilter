# MadLab.QueryFilter: Dynamic Query Filtering for EF Core


A modular, maintainable .NET 8 solution for managing and querying entities with advanced filtering and paging. This project demonstrates clean architecture principles using the Repository and QueryBuilder patterns, making it easy to extend and test.
This is a test project, created as a prove of concept it is not intended to be a full application, as you can see, the only existing UI is a Console app, and it only displays a list of items. If you want to analyze this project your attention should be mainly focused on the MadLab.QueryFilter.Services.Test project and MadLab.QueryFilter.Services project.

## General Idea
We are developing a blog app (Yes, again!! I can see your eyes rolling), where users can post their thoughts, every Post can be part of two main cateogires "Mood" and "Type" and every Post can be voted or commented by other authors. It is intended to be simple so we can focus on the way we are going to work with the decorator pattern. We are using QLite as a data source for simplicity and portability. 


## Features

- **Advanced querying** with composable filters (by author, mood, post type, date range, keyword, etc.)
- **Paging support** for all list queries
- **Separation of concerns** via DTOs and repository abstraction
- **Unit tests** for all service methods

## Architecture

- **Repository Pattern:** Abstracts data access, enabling easy swapping of data sources and simplifying testing.
- **Decorator Pattern:** Allows fluent, reusable composition of queries with filters and paging.
- **Filter Classes:** Encapsulate query logic for each filter (e.g., `FilterByIsPublished`, `FilterByAuthor`), implementing a common interface.

## Example Usage

Let's say you have the following service methods to retrieve posts
```
        public async Task<IEnumerable<PostListDTO>> GetPublishedPagedByAuthorAndMoodTypeAndPostType(int authorId, int moodType, int postType, int currentPage, int pageSize)
        {
            
            var result = await _postRepository.Query()
                        .Include(p => p.Author)
                        .Include(p => p.PostType)
                        .Include(p => p.MoodType)
                        .Where(p => p.AuthorId == authorId && p.MoodTypeId == moodType && p.PostTypeId == postType && p.IsPublished)
                        .AsNoTracking()
                        .ToListAsync();


            return ConvertToDto(result);
        }

        public async Task<IEnumerable<PostListDTO>> GetPublishedPagedByAuthorPostType(int authorId, int postType, int currentPage, int pageSize)
        {
            var result = await _postRepository.Query()
                        .Include(p => p.Author)
                        .Include(p => p.PostType)
                        .Include(p => p.MoodType)
                        .Where(p => p.AuthorId == authorId && p.PostTypeId == postType && p.IsPublished)
                        .AsNoTracking()
                        .ToListAsync();


            return ConvertToDto(result);
        }
```

As you can see, we are repeating some filters, what if can create filters and re use them?, and convert that code into this?:
```
        public async Task<IEnumerable<PostListDTO>> GetPublishedPagedByAuthorAndMoodTypeAndPostType(int authorId, int moodType, int postType, int currentPage, int pageSize)
        {
            _queryBuilder
                .AddFilter(new FilterByIsPublished(true))
                .AddFilter(new FilterByAuthor(authorId))
                .AddFilter(new FilterByMoodType(moodType))
                .AddFilter(new FilterByPostType(postType))
                .AddPaging(currentPage, pageSize);

            IEnumerable<Post> result = await _queryBuilder.Build();

            return ConvertToDto(result);
        }

        public async Task<IEnumerable<PostListDTO>> GetPublishedPagedByAuthorPostType(int authorId, int postType, int currentPage, int pageSize)
        {
            _queryBuilder
                .AddFilter(new FilterByIsPublished(true))
                .AddFilter(new FilterByAuthor(authorId))
                .AddFilter(new FilterByPostType(postType))
                .AddPaging(currentPage, pageSize);

            IEnumerable<Post> result = await _queryBuilder.Build();

            return ConvertToDto(result);
        }
```

With this approach, we can add or remove filters, but also the code is more readable, and if for any reason we need to create a new "Get" method we can re use any filter we already created. 


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
   
 5. **C# Unit testing**
   - All examples are contianed in a test project. So if you want to understand what is going on, you will need to run and maybe debug some tests. Maybe even create your own


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