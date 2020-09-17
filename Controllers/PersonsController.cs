using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Globaltec.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Globaltec.Controllers
{
    [Route("api/persons")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly PersonContext _context;

        public PersonsController(PersonContext context)
        {
            _context = context;
        }

        /// <summary>
        /// list all persons (query by UF enable)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersons([FromQuery] PersonQueryString query)
        {
            try
            {
                return await _context.Persons
                    .Where(p => query.Uf == null || p.Uf == query.Uf)
                    .ToListAsync();
            }
            catch (System.Exception)
            {
                return UnprocessableEntity();
            }
        }

        /// <summary>
        /// list a person by code
        /// </summary>
        [HttpGet("{code}")]
        [Authorize(Roles = "manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Person>> GetPerson(int code)
        {
            try
            {
                var person = await _context.Persons.FindAsync(code);
                if (person == null) return NotFound();
                return person;
            }
            catch (System.Exception)
            {
                return UnprocessableEntity();
            }
        }

        /// <summary>
        /// update a person
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{code}")]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> PutPerson(int code, Person person)
        {
            try
            {
                if (code != person.Code) return BadRequest();
                _context.Entry(person).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetPerson),
                    new { code = person.Code, person },
                    person
                );
            }
            catch (DbUpdateConcurrencyException) when (!PersonExists(code))
            {
                return NotFound();
            }
            catch (System.Exception)
            {
                return UnprocessableEntity();
            }
        }

        /// <summary>
        /// create a new person
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "manager")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<Person>> PostPerson(Person person)
        {
            try
            {
                _context.Persons.Add(person);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                nameof(GetPerson),
                new { code = person.Code, person },
                person
                );
            }
            catch (System.Exception)
            {
                return UnprocessableEntity();
            }
        }

        /// <summary>
        /// delete a person by code
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{code}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<Person>> DeletePerson(int code)
        {
            try
            {
                var person = await _context.Persons.FindAsync(code);
                if (person == null) return NotFound();

                _context.Persons.Remove(person);
                await _context.SaveChangesAsync();
                return person;
            }
            catch (System.Exception)
            {
                return UnprocessableEntity();
            }
        }

        private bool PersonExists(int code)
        {
            return _context.Persons.Any(e => e.Code == code);
        }
    }
}
