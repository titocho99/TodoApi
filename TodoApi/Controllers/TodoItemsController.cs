﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemsDTO>>> GetTodoItems()
        {
            if(_context.TodoItems == null)
            {
                return NotFound();
            }
            return await _context.TodoItems
                .Select(x => ItemToDTO(x))
                .ToListAsync();
            //return await _context.TodoItems.ToListAsync();
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemsDTO>> GetTodoItem(long id)
        {
            if(_context.TodoItems == null)
            {
                return NotFound();
            }

            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }
            return ItemToDTO(todoItem);
            //return todoItem;
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItemsDTO todoDTO)
        {
            if (id != todoDTO.Id)
            {
                return BadRequest();
            }

            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            todoItem.Name= todoDTO.Name;
            todoItem.IsComplete = todoDTO.IsComplete;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
                return NotFound();
            }
            

            return NoContent();
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItemsDTO>> PostTodoItem(TodoItemsDTO todoDTO)
        {
            var todoItem = new TodoItem
            {
                IsComplete = todoDTO.IsComplete,
                Name = todoDTO.Name
            };

            
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
            //return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
            return CreatedAtAction(nameof (GetTodoItem), 
                new { id = todoItem.Id },
                ItemToDTO(todoItem));
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
           
            
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }

        private static TodoItemsDTO ItemToDTO(TodoItem todoItem) =>
            new TodoItemsDTO
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };
    }
}
