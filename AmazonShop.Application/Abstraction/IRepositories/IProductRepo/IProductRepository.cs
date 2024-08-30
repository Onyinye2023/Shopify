﻿using AmazonShop.Domain.Enums;
namespace AmazonShop.Application.Abstraction.IRepositories.IProductRepo
{
    public interface IProductRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetByNameAsync(string name);
        Task<IEnumerable<T>> GetByCategoryAsync(ProductCategory category);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task AddSingleProductAsync(T entity, string userName);
        Task AddMultipleProductAsync(IEnumerable<T> entity, string username);
        Task<bool> DeleteAsync(T entity, int quantity);
        Task SaveChangesAsync();
    }
}
