using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Maintenance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public PostsController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("placeholder_api");
        }

        /// <summary>
        /// List all posts
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetList()
        {
            var response = await _httpClient.GetAsync("/posts");
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<PostDto>>(responseBody);
            return result;
        }

        /// <summary>
        /// Get posts by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Job.Reader")]
        public async Task<ActionResult<PostDto>> Get(int id)
        {
            var response = await _httpClient.GetAsync($"/posts/{id}");
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PostDto>(responseBody);
            return result;
        }

        /// <summary>
        /// Get comments related to a post
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [HttpGet("comments")]
        [Authorize(Roles = "Job.Admin")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments(int postId)
        {
            var response = await _httpClient.GetAsync($"/comments?postId={postId}");
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<CommentDto>>(responseBody);
            return result;
        }
    }

    public sealed class PostDto
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public sealed class CommentDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Body { get; set; }
    }
}
