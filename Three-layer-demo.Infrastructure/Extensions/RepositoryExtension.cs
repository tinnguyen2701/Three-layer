using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Three_layer_demo.Domain.Interfaces;

namespace Three_layer_demo.Infrastructure.Extensions
{
    public static class RepositoryExtension
    {
        public static IQueryable<T> Where<T>(this IRepository<T> repository, Expression<Func<T, bool>> predicate) where T : class
        {
            return repository.Entities.Where(predicate);
        }

    }
}
