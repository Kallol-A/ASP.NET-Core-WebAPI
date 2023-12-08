using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        // GET api/tests
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "test1", "test2", "test3" };
        }

        // GET api/tests/2
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "My test2";
        }

        // POST api/tests
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/tests/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/tests/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
