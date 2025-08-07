using CloudinaryDotNet.Actions;

namespace FurnitureProject.Models
{
    public class District
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public int ProvinceCode { get; set; }
        public List<Ward> Wards { get; set; }
    }
}
