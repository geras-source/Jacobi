using Jacobi.Services.Jacobi;
using Jacobi.Services.Storage;
using Microsoft.AspNetCore.Mvc;

namespace JacobiApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JacobiController : ControllerBase
    {
        private readonly IJacobiCalculator _jacobiCalculator;
        private readonly IAnswerStorage _answerStorage;

        public JacobiController(IJacobiCalculator jacobiCalculator, IAnswerStorage answerStorage)
        {
            _jacobiCalculator = jacobiCalculator;
            _answerStorage = answerStorage;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<List<Answer>>> Calculate(List<double[]> input)
        {
            if (!Validator(input)) return BadRequest();

            try
            {
                var rows = input.Count;
                var cols = input[0].Length;
                var matrix = new double[rows, cols];

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        matrix[i, j] = input[i][j];
                    }
                }
            
                _jacobiCalculator.Calculate(matrix);

                var results = await _answerStorage.GetAll();

                if (results == null) throw new ArgumentNullException(nameof(results));

                return Ok(results);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        private bool Validator(List<double[]> input)
        {
            if(input.Count == 0) return false;
            if(input.Count == 1 && input[0].Length == 0) return false;
            if(input.Count == 1 && input[0].Length == 1) return false;
            if(input.Count != input[0].Length ) return false;

            return true;
        }
    }
}