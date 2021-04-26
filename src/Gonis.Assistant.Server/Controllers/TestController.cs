using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gonis.Assistant.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.1")]
    public class TestController : ControllerBase
    {
        /// <summary>
        /// Получение файла по ID договора
        /// </summary>
        /// <returns>Список</returns>
        [HttpGet]
        public async IAsyncEnumerable<string> GetAsync()
        {
            await foreach (var number in GetNumbersAsync())
            {
                yield return $"Step - {number}";
            }
        }

        private async IAsyncEnumerable<int> GetNumbersAsync()
        {
            for (var i = 1; i <= 5; i++)
            {
                await Task.Delay(100);
                yield return i;
            }
        }
    }
}
