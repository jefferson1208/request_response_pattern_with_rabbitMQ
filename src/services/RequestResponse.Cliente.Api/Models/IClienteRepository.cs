using RequestResponse.Core.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RequestResponse.Cliente.Api.Models
{
    public interface IClienteRepository : IRepository<Clientes>
    {
        void Adicionar(Clientes cliente);

        Task<IEnumerable<Clientes>> BuscarTodos();
        Task<Clientes> BuscarPorCpf(string cpf);

        void AdicionarEndereco(Endereco endereco);
        Task<Endereco> BuscarEnderecoPorId(Guid id);
    }
}
