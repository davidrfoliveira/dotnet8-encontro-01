using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Seguradora.Api.Controllers;

[Authorize(Roles = "Corretor,Admin")]
[ApiController]
[Route("segurados")]
public class SeguradosController : ControllerBase
{
    private readonly ISeguradoRepositorio _repo;
    public SeguradosController(ISeguradoRepositorio seguradoRepositorio)
    {
        _repo = seguradoRepositorio;
    }
  
      [HttpGet]
      public ActionResult<IEnumerable<SeguradoResponse>> Listar() =>
        Ok(_repo.Listar().Select(SeguradoResponse.De));

      [HttpGet("{id}")]
      public ActionResult<SeguradoResponse> ObterPorId(string id)
      {
          var segurado = _repo.ObterPorId(id);
          return segurado is null ? NotFound() : Ok(SeguradoResponse.De(segurado));
      }

    [HttpPost]
    public ActionResult<SeguradoResponse> Criar(SeguradoRequest requisicao)
    {
        var segurado = new Segurado
        {
            Nome = requisicao.Nome,
            Documento = new Cpf(requisicao.Cpf),
            DataNascimento = requisicao.DataNascimento
        };

        _repo.Adicionar(segurado);

        return CreatedAtAction(nameof(ObterPorId), new { id = segurado.Id }, SeguradoResponse.De(segurado));
    }
}