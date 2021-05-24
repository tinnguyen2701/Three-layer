using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Three_layer_demo.Domain.Entities;
using Three_layer_demo.Domain.Interfaces;
using Three_layer_demo.Infrastructure.Extensions;

namespace Three_layer_demo.Infrastructure.Repositories
{
    public static class ActorRepository
    {
        public static async Task<IList<Actor>> GetAll(this IRepository<Actor> repository)
        {
            var actors = new List<Actor>();

            await repository.DbContext.LoadStoredProc("spGetActors")
                .ExecuteStoredProcAsync(result =>
                {
                    actors = result.ReadNextListOrEmpty<Actor>();
                });


            return actors;
        }

    }
}
