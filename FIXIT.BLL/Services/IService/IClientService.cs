using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIXIT.BLL.DTOs.ClientDTOs;
using FIXIT.BLL.DTOs.CraftsmanDTOs;

namespace FIXIT.BLL.Services.Intrfaces
{
    public interface IClientService
    {
        Task<IEnumerable<GetAllClientsDTO>> GetAllClientsAsync();
        Task<GetAllClientsDTO> GetClientsByIdAsync(int id);
        Task CreateClientAsync(CreateClientDTO client);
       Task<bool>UpdateClientAsync(int id, UpdateClientDTO ClientDto);
        void DeleteClient(int id);


    }
}
