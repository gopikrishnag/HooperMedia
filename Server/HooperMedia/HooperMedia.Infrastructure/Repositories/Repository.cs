﻿using Microsoft.EntityFrameworkCore;
using HooperMedia.Infrastructure.Repositories.Interfaces;
using HooperMedia.Infrastructure.Data;

namespace HooperMedia.Infrastructure.Repositories
{
    
    public class Repository<TEntity>(ApplicationDbContext context) : IRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext context = context;
        private readonly DbSet<TEntity> dbSet = context.Set<TEntity>();

       
        public virtual async Task<TEntity?> GetByIdAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

       
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        
        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            await dbSet.AddAsync(entity);
            await context.SaveChangesAsync();
            return entity;
        }

       
        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            dbSet.Update(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        
        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity is null)
            {
                return false;
            }

            dbSet.Remove(entity);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
