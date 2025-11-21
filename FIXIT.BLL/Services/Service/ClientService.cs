using AutoMapper;
using FIXIT.API.Erorrs.Exceptions;
using FIXIT.API.Erorrs.Exceptions;
using FIXIT.BLL.DTOs.ClientDTOs;
using FIXIT.BLL.Helper.UploadHandler;
using FIXIT.BLL.Exceptions;
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
        private readonly UploadHandler uploadHandler;

        public ClientService(IClientRepo repo,IMapper mapper, UploadHandler uploadHandler)
        {
            this.repo = repo;
            this.mapper = mapper;
            this.uploadHandler = uploadHandler;
        }

        public async Task CreateClientAsync(CreateClientDTO client)
        {
            //string? imagePath = null;

            //if (client.ProfileImage != null)
            //{
            //    imagePath = uploadHandler.Upload(client.ProfileImage, "Clients");
            //}
            if (client == null) throw new ValidationException("Client data cannot be null.");
            var clientEntity = mapper.Map<Client>(client);

            //clientEntity.ProfileImage = imagePath;

            await repo.AddAsync(clientEntity);

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
				throw new NotFoundException(nameof(Client), "No Clients were Found");

			return mapper.Map<List<GetAllClientsDTO>>(clients);

		}
		public async Task<GetAllClientsDTO> GetClientByEmail(string Email)
		{
 	 	  var normalizedEmail = Email.ToUpper();
	  	  var client = await repo.GetClientByEmailAsync(normalizedEmail);
            if (client == null)
                 throw new NotFoundException(nameof(Client), "No Client was  Found");
            return mapper.Map<GetAllClientsDTO>(client);

		}
		
        public async Task<GetAllClientsDTO> GetClientsByIdAsync(int id)
        {
            var client= await repo.GetAsync(id);
            if (client == null)
                throw new NotFoundException(nameof(Client), id);
            
            return mapper.Map<GetAllClientsDTO>(client);

         
        }

        public async Task<bool> UpdateClientAsync(int id, UpdateClientDTO clientDto)
        {
            // 🔹 Step 1: Get the existing client asynchronously
            var existingClient = await repo.GetAsync(id);
            if (existingClient == null)
                return false;

            // 🔹 Step 2: Map updated fields from DTO to entity
            mapper.Map(clientDto, existingClient);

            // 🔹 Step 3: Handle image upload
            if (clientDto.ProfileImage != null)
            {
                // Optionally delete the old image file
                if (!string.IsNullOrEmpty(existingClient.ProfileImage))
                {
                    var oldPath = Path.Combine("wwwroot", existingClient.ProfileImage);
                    if (File.Exists(oldPath))
                        File.Delete(oldPath);
                }

                // Upload new image
                existingClient.ProfileImage = uploadHandler.Upload(clientDto.ProfileImage);
            }

            // 🔹 Step 4: Update the record (sync)
            repo.Update(existingClient, id);

            // 🔹 Step 5: Save changes (sync)
            repo.Save();

            return true;
        }




    }
}
