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
        Task<GetAllClientsDTO> GetClientByEmail(string Email);
        Task CreateClientAsync(CreateClientDTO client);
        bool UpdateClient(int id, UpdateClientDTO ClientDto);
        void DeleteClient(int id);


    }
}
