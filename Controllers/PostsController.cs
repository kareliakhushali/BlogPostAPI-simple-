using BlogPostAPI.Models;
using BlogPostAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogPostAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public PostsController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await _appDbContext.Posts.ToListAsync();
            return Ok(posts);
        }
        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetPostById")]
        public async Task<IActionResult> GetPostById(Guid id)
        {
            var post = await _appDbContext.Posts.FirstOrDefaultAsync(x => x.Id == id);
            if(post != null)
            {
                return Ok(post);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> AddPost(AddPostRequest addPostRequest)
        {
            // convert DTO  to Entity
            var post = new Post()
            {
                Title = addPostRequest.Title,
                Content =  addPostRequest.Content,
                Author = addPostRequest.Author,
                FeaturedImageHandle =  addPostRequest.FeaturedImageHandle,
                PublishedDate = addPostRequest.PublishedDate,
                UpdatedDate = addPostRequest.UpdatedDate,
                Summary = addPostRequest.Summary,
                UrlHandle =  addPostRequest.UrlHandle,
                Visible = addPostRequest.Visible
            };
           post.Id = Guid.NewGuid();
            await _appDbContext.Posts.AddAsync(post);
            await _appDbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPostById),new { id = post.Id },post);

        }
        [HttpPut]
        [Route("{id:guid}")]
        public async Task <IActionResult> UpdatePost([FromRoute] Guid id , UpdatePostRequest updatePostRequest)
        {
            // convert DTO to entity
           /* var post = new Post()
            {
                Title = updatePostRequest.Title,
                Content = updatePostRequest.Content,
                Author = updatePostRequest.Author,
                FeaturedImageHandle = updatePostRequest.FeaturedImageHandle,
                PublishedDate = updatePostRequest.PublishedDate,
                UpdatedDate = updatePostRequest.UpdatedDate,
                Summary = updatePostRequest.Summary,
                UrlHandle = updatePostRequest.UrlHandle,
                Visible = updatePostRequest.Visible

            };*/
            // check if exists
            var existingPost = await _appDbContext.Posts.FindAsync(id);
            if(existingPost != null)
            {
                existingPost.Title = updatePostRequest.Title;
                existingPost.Content = updatePostRequest.Content;
                existingPost.Author = updatePostRequest.Author;
                existingPost.FeaturedImageHandle = updatePostRequest.FeaturedImageHandle;
                existingPost.PublishedDate = updatePostRequest.PublishedDate;
                existingPost.Summary = updatePostRequest.Summary;
                existingPost.UrlHandle = updatePostRequest.UrlHandle;
                existingPost.Visible = updatePostRequest.Visible;
                await _appDbContext.SaveChangesAsync();
                return Ok(existingPost);
            }
            return NotFound();

        }
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var existingPost = await _appDbContext.Posts.FindAsync(id);
            if(existingPost != null)
            {
                _appDbContext.Remove(existingPost);
                await _appDbContext.SaveChangesAsync();
                return Ok(existingPost);
            }
            return NotFound();
        }

    }
}
