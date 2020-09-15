﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Globaltec.Models;
using Globaltec.Services;
using Globaltec.Repositories;
using Microsoft.AspNetCore.Authorization;
namespace Globaltec.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly PersonContext _context;

        public PersonsController(PersonContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] User model)
        {
            var user = UserRepository.Get(model.Username, model.Password);
            if (user == null) return NotFound(new { message = "Invalid user or password" });

            var token = TokenService.GenerateToken(user);
            user.Password = "";

            return new
            {
                user = user,
                token = token
            };
        }

        // GET: api/Persons
        [HttpGet]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersons([FromQuery] PersonQueryString query)
        {
            return await _context.Persons
              .Where(p => query.Uf == null || p.Uf == query.Uf)
              .ToListAsync();
        }

        // GET: api/Persons/5
        [HttpGet("{id}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<Person>> GetPerson(int id)
        {
            var person = await _context.Persons.FindAsync(id);

            if (person == null) return NotFound();

            return person;
        }

        // PUT: api/Persons/5
        [HttpPut("{id}")]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> PutPerson(int id, Person person)
        {
            if (id != person.Id) return BadRequest();

            _context.Entry(person).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!PersonExists(id))
            {
                return NotFound();
            }

            return CreatedAtAction(
              nameof(GetPerson),
              new { id = person.Id, person },
              person
            );
        }

        // POST: api/Persons
        [HttpPost]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<Person>> PostPerson(Person person)
        {
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
              nameof(GetPerson),
              new { id = person.Id, person },
              person
            );
        }

        // DELETE: api/Persons/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<Person>> DeletePerson(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();

            return person;
        }

        private bool PersonExists(int id)
        {
            return _context.Persons.Any(e => e.Id == id);
        }
    }
}
