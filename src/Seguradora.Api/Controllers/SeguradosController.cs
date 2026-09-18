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

      [HttpPost]
      public ActionResult<IEnumerable<Segurado>> ListarId(string id) => Ok(_repo.ObterPorId(id));
}