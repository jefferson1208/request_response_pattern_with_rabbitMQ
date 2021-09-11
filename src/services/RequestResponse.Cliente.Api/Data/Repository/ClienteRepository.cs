using Microsoft.EntityFrameworkCore;
using RequestResponse.Cliente.Api.Models;
using RequestResponse.Core.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RequestResponse.Cliente.Api.Data.Repository
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ClienteContext _context;

        public ClienteRepository(ClienteContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<IEnumerable<Clientes>> BuscarTodos()
        {
            return await _context.Clientes.AsNoTracking().ToListAsync();
        }

        public Task<Clientes> BuscarPorCpf(string cpf)
        {
            return _context.Clientes.FirstOrDefaultAsync(c => c.Cpf.Numero == cpf);
        }

        public void Adicionar(Clientes cliente)
        {
            _context.Clientes.Add(cliente);
        }

        public async Task<Endereco> BuscarEnderecoPorId(Guid id)
        {
            return await _context.Enderecos.FirstOrDefaultAsync(e => e.ClienteId == id);
        }

        public void AdicionarEndereco(Endereco endereco)
        {
            _context.Enderecos.Add(endereco);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
