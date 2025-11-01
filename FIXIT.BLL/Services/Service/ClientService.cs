using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FIXIT.BLL.DTOs.ClientDTOs;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.Repositories;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Services.Intrfaces;
using FIXIT.DAL.Models;

namespace FIXIT.BLL.Services.Service
{
    public class ClientService: IClientService
    {
        private readonly IGenericRepository<Client> repo;
        private readonly IMapper mapper;

        public ClientService(IGenericRepository<Client> repo,IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        public async Task CreateClientAsync(CreateClientDTO client)
        {
        await  repo.AddAsync(mapper.Map<Client>(client));
            repo.Save();

            //throw new NotImplementedException();
        }

        public void DeleteClient(int id)
        {
            repo.Delete(id);
            repo.Save();
            //throw new NotImplementedException();
        }

        public async Task<IEnumerable<GetAllClientsDTO>> GetAllClientsAsync()
        {
            var clients = await repo.GetAllAsync();
            var result = mapper.Map<List<GetAllClientsDTO>>(clients);
            return result;
            //  throw new NotImplementedException();
        }

        public async Task<GetAllClientsDTO> GetClientsByIdAsync(int id)
        {
            var client= await repo.GetAsync(id);
            if (client == null)
                return null;
            return mapper.Map<GetAllClientsDTO>(client);

            // throw new NotImplementedException();
        }

        public bool UpdateClient(int id, UpdateClientDTO ClientDto)
        {
           if( repo.Update(mapper.Map<Client>(ClientDto),id))
            {
                repo.Save();
                return true;
            
            }
           else
               return false;
        }
    }
}
