using AutoMapper;
using FIXIT.API.Erorrs.Exceptions;
using FIXIT.BLL.DTOs.ClientDTOs;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Repositories.Repo;
using FIXIT.BLL.Services.Intrfaces;
using FIXIT.DAL.Models;

namespace FIXIT.BLL.Services.Service
{
    public class ClientService: IClientService
    {
        private readonly IClientRepo repo;
        private readonly IMapper mapper;

        public ClientService(IClientRepo repo,IMapper mapper)
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
                throw new NotFoundException(nameof(Client), id);
            
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

        public async Task<GetAllClientsDTO> GetClientByEmail(string Email)
        {
            var normalizedEmail = Email.ToUpper();
            var Client = await repo.GetClientByEmailAsync(normalizedEmail);
            return mapper.Map<GetAllClientsDTO>(Client);
        }



    }
}
