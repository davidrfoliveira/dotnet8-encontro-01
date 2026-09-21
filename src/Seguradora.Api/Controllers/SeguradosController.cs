using Microsoft.AspNetCore.Mvc;
namespace Seguradora.Api.Controllers;

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
      public ActionResult<IEnumerable<Segurado>> Listar() => Ok(_repo.Listar());

      [HttpGet("{id}")]
      public ActionResult<IEnumerable<Segurado>> ObterPorId(string id) => Ok(_repo.ObterPorId(id));

    [HttpPost]
    public ActionResult<Segurado> Criar(SeguradoRequest requisicao)
    {
        var segurado = new Segurado
        {
            Nome = requisicao.Nome,
            Documento = new Cpf(requisicao.Cpf),
            DataNascimento = requisicao.DataNascimento
        };

        _repo.Adicionar(segurado);

        return CreatedAtAction(nameof(ObterPorId), new {id = segurado.Id}, segurado);
    }
}