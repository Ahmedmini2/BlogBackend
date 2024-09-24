using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogAPI.Models;

namespace BlogAPI.Controllers
{
    // I Changed The [Route("api/[Controller]")] to [Route("api/posts")] To match the requirments 
    [Route("api/posts")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly BlogContext _context;

        public BlogPostsController(BlogContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var totalPosts = await _context.BlogPosts.CountAsync();
            var totalPages = (int)System.Math.Ceiling((double)totalPosts / pageSize);

            var posts = await _context.BlogPosts
                .OrderByDescending(i => i.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                posts,
                totalPages,
                totalCount = totalPosts
            });
        }

        // GET: api/posts/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogPost>> GetBlogPost(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return Ok(blogPost);
        }

        // POST: api/posts
        [HttpPost]
        public async Task<ActionResult<BlogPost>> CreateBlogPost(BlogPost blogPost)
        {
            blogPost.CreatedAt = DateTime.UtcNow;
            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBlogPost), new { id = blogPost.Id }, blogPost);
        }

        // PUT: api/posts/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlogPost(int id, BlogPost blogPost)
        {
            if (id != blogPost.Id)
            {
                return BadRequest();
            }

            var existingPost = await _context.BlogPosts.FindAsync(id);
            if (existingPost == null)
            {
                return NotFound();
            }

            existingPost.Title = blogPost.Title;
            existingPost.Content = blogPost.Content;
            existingPost.Author = blogPost.Author;
            existingPost.CreatedAt = blogPost.CreatedAt;

            _context.Entry(existingPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogPostExists(id))
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

        // DELETE: api/posts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogPost(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
        }
    }
}