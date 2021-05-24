using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Three_layer_demo.Domain.Entities;

namespace Three_layer_demo.Domain.Interfaces.Services
{
    public interface IActorService
    {
        Task<IList<Actor>> GetAll();
        Task<Actor> GetOne(int actorId);
        Task Update(Actor actor);
        Task Add(Actor actor);
        Task Delete(int actorId);
    }
}
