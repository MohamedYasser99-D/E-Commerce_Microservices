using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Common;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderContext :DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options):base(options)
        {
        }
       public DbSet<Order> Orders { set; get; }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var item in ChangeTracker.Entries<EntityBase>())
            {
                switch (item.State)
                {
                    case EntityState.Modified:
                        item.Entity.LastModifiedBy = "svn";
                        item.Entity.LastModifiedDate = DateTime.Now;    
                        break;
                    case EntityState.Added:
                        item.Entity.CreatedBy = "svn";
                        item.Entity.CreatedDate = DateTime.Now;
                        break;
                    default:
                        break;
                }

            }
            
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
