using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Three_layer_demo.Domain.Entities;
using Three_layer_demo.Domain.Interfaces;
using Three_layer_demo.Domain.Interfaces.Services;

namespace Three_layer_demo.Service
{
    public class ActorService : IActorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ActorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<Actor>> GetAll()
        {
            return await _unitOfWork.Repository<Actor>().GetAllAsync();
        }

        public async Task<Actor> GetOne(int actorId)
        {
            return await _unitOfWork.Repository<Actor>().FindAsync(actorId);
        }

        public async Task Update(Actor actorInput)
        {
            try
            {
                await _unitOfWork.BeginTransaction();
                var actorRepos = _unitOfWork.Repository<Actor>();
                var actor = await actorRepos.FindAsync(actorInput.Id);
                if (actor == null) throw new KeyNotFoundException();

                actor.Name = actorInput.Name;

                await _unitOfWork.CommitTransaction();
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        public async Task Add(Actor actorInput)
        {
            try
            {
                await _unitOfWork.BeginTransaction();

                var actorRepos = _unitOfWork.Repository<Actor>();
                await actorRepos.InsertAsync(actorInput);
                
                await _unitOfWork.CommitTransaction();
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        public async Task Delete(int actorId)
        {
            try
            {
                await _unitOfWork.BeginTransaction();

                var actorRepos = _unitOfWork.Repository<Actor>();
                var actor = await actorRepos.FindAsync(actorId);
                if (actor == null) throw new KeyNotFoundException();

                await actorRepos.DeleteAsync(actor);

                await _unitOfWork.CommitTransaction();
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackTransaction();
                throw;
            }
        }
    }


}
