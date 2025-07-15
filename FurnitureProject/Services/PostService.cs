using FurnitureProject.Models;
using FurnitureProject.Repositories;

namespace FurnitureProject.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;

        public PostService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await _postRepository.GetAllAsync();
        }

        public async Task<Post?> GetByIdAsync(Guid id)
        {
            return await _postRepository.GetByIdAsync(id);
        }

        public async Task<(bool Success, string? Message)> CreateAsync(Post post)
        {
            try
            {
                await _postRepository.AddAsync(post);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Message)> UpdateAsync(Post post)
        {
            try
            {
                await _postRepository.UpdateAsync(post);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await _postRepository.DeleteAsync(id);
            return true;
        }
    }

}
