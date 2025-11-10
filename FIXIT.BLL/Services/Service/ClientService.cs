using AutoMapper;
using FIXIT.API.Erorrs.Exceptions;
using FIXIT.BLL.DTOs.ClientDTOs;
using FIXIT.BLL.Exceptions;
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

        public async Task<GetAllClientsDTO> GetClientsByIdAsync(int id)
        {
            var client= await repo.GetAsync(id);
            if (client == null)
                throw new NotFoundException(nameof(Client), id);
            
            return mapper.Map<GetAllClientsDTO>(client);

         
        }

        public bool UpdateClient(int id, UpdateClientDTO ClientDto)
        {
			if (id != ClientDto.Id)
				throw new ValidationException("Id mismatch between route and body.");

			var updated = repo.Update(mapper.Map<Client>(ClientDto), id);
			if (!updated)
				throw new NotFoundException(nameof(Client), id);

			repo.Save();
			return true;
		}
    }
}
