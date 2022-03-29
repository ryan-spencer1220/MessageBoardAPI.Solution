using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MessageBoard.Models;
using Microsoft.AspNetCore.Cors;

namespace MessageBoard.Controllers
{
  [Route("api/1.0/Messages")]
  [ApiController]
  [ApiVersion("1.0")]
  public class MessagesV1Controller : ControllerBase
  {
    private readonly MessageBoardContext _db;
    private readonly IUriService uriService;

    public MessagesV1Controller(MessageBoardContext db, IUriService uriService)
    {
      _db = db;
      // Pagination
      this.uriService = uriService;
    }

    // GET api/messages
    [EnableCors("Policy")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Message>>> Get([FromQuery] PaginationFilter filter, string author)
    {
      var route = Request.Path.Value;
      var query = _db.Messages.AsQueryable();
      // Pagination
      var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
      var pagedData = await _db.Messages
        .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
        .Take(validFilter.PageSize)
        .ToListAsync();
      var totalRecords = await _db.Messages.CountAsync();
      if (author != null)
      {
        query = query.Where(entry => entry.Author == author);
      }
      // var pagedReponse = 
      return Ok(PaginationHelper.CreatePagedReponse<Message>(pagedData, validFilter, totalRecords, uriService, route));
      // return await query.ToListAsync();
    }

    // POST api/messages
    [HttpPost]
    public async Task<ActionResult<Message>> Post(Message message)
    {
      _db.Messages.Add(message);
      await _db.SaveChangesAsync();

      return CreatedAtAction(nameof(GetMessage), new { id = message.MessageId }, message);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Message>> GetMessage(int id)
    {
        var message = await _db.Messages.FindAsync(id);

        if (message == null)
        {
            return NotFound();
        }

        return message;
    }

    // PUT: api/Animals/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Message message)
    {
      if (id != message.MessageId)
      {
        return BadRequest();
      }

      _db.Entry(message).State = EntityState.Modified;

      try
      {
        await _db.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!MessageExists(id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      return NoContent();
    }
    private bool MessageExists(int id)
    {
      return _db.Messages.Any(e => e.MessageId == id);
    }

    // DELETE: api/Animals/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMessage(int id)
    {
      var message = await _db.Messages.FindAsync(id);
      if (message == null)
      {
        return NotFound();
      }

      _db.Messages.Remove(message);
      await _db.SaveChangesAsync();

      return NoContent();
    }
  }
}