namespace FurnitureProject.Models
{
    public class PostCategoryLink : BaseEntity
    {
        public Guid PostId { get; set; }
        public Post Post { get; set; }

        public Guid PostCategoryId { get; set; }
        public PostCategory PostCategory { get; set; }
    }
}
