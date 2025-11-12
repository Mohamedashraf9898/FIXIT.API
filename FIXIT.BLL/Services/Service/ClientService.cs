using AutoMapper;
using FIXIT.API.Erorrs.Exceptions;
using FIXIT.API.Erorrs.Exceptions;
using FIXIT.BLL.DTOs.ClientDTOs;
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
			if(client == null)
			throw new ValidationException("Client data cannot be null");

			await repo.AddAsync(mapper.Map<Client>(client));
			repo.Save();
		}

        public void DeleteClient(int id)
        {
			var client = repo.GetAsync(id).Result;
			if (client == null)
				throw new NotFoundException(nameof(Client), id);

			repo.Delete(id);
			repo.Save();
		}

        public async Task<IEnumerable<GetAllClientsDTO>> GetAllClientsAsync()
        {
			var clients = await repo.GetAllAsync();
			if (clients == null || !clients.Any())
				throw new NotFoundException(nameof(Client), "No Clients Found");

			return mapper.Map<List<GetAllClientsDTO>>(clients);

		}
		public async Task<GetAllClientsDTO> GetClientByEmail(string Email)
		{
 	 	  var normalizedEmail = Email.ToUpper();
	  	  var client = await repo.GetClientByEmailAsync(normalizedEmail);
  	 	 return mapper.Map<GetAllClientsDTO>(client);

		}
		
        public async Task<GetAllClientsDTO> GetClientsByIdAsync(int id)
        {
            var client= await repo.GetAsync(id);
            if (client == null)
                throw new NotFoundException(nameof(Client), id);
            
            return mapper.Map<GetAllClientsDTO>(client);

         
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
