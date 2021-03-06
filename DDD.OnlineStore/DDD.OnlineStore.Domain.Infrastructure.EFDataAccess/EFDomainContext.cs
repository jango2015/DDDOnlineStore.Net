﻿using DDD.OnlineStore.Domain.Common;
using DDD.OnlineStore.Domain.Infrastructure.EFDataAccess.Common;
using DDD.OnlineStore.Domain.Infrastructure.EFDataAccess.Configuration;
using DDD.OnlineStore.Domain.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks; 

namespace DDD.OnlineStore.Domain.Infrastructure.EFDataAccess
{
    public class EFDomainContext : DbContext
    {
        public virtual DbSet<User> UserSet { get; set; }
        public virtual DbSet<PurchaseOrder> OrderSet { get; set; }
        public virtual DbSet<Product> ProductSet { get; set; }
        public virtual DbSet<Role> RoleSet { get; set; }
        public virtual DbSet<Category> CategorySet { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShoppingCart>()
                        .HasOptional(a => a.User)
                        .WithMany()
                        .HasForeignKey(a => a.UserID);

            modelBuilder.Entity<PurchaseOrder>()
                        .HasOptional(a => a.User)
                        .WithMany()
                        .HasForeignKey(a => a.UserID);

            modelBuilder.Entity<ShoppingCart>().ToTable("ShoppingCarts");
            modelBuilder.Entity<PurchaseOrder>().ToTable("PurchaseOrders");
 
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// creates a new DB if not exists on Startup
        /// the new DB will be automatically filled with test data
        /// </summary>
        public static void CreateInitialDB() 
        {
            Database.SetInitializer<EFDomainContext>(new DBInitializer());        
        }

        #region provides include feature out of EF Context
        
        static EFDomainContext() { QueryableExtensions.Includer = new DbIncluder(); }
        private class DbIncluder : QueryableExtensions.IIncluder
        {
            public IQueryable<T> Include<T, TProperty>(IQueryable<T> source, Expression<Func<T, TProperty>> path)
                where T : class
            {
                return DbExtensions.Include(source, path);
            }
        }
        
        #endregion
    }
}
