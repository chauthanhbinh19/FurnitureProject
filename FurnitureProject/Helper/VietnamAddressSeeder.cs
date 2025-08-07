using FurnitureProject.Data;
using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FurnitureProject.Helper
{
    public class VietnamAddressSeeder
    {
        private readonly AppDbContext _context;

        public VietnamAddressSeeder(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync("https://provinces.open-api.vn/api/?depth=3");
            var provinces = JsonConvert.DeserializeObject<List<Province>>(response);

            foreach (var province in provinces)
            {
                if (!await _context.Provinces.AnyAsync(p => p.Code == province.Code))
                {
                    _context.Provinces.Add(new Province
                    {
                        Code = province.Code,
                        Name = province.Name
                    });
                }

                foreach (var district in province.Districts)
                {
                    if (!await _context.Districts.AnyAsync(d => d.Code == district.Code))
                    {
                        _context.Districts.Add(new District
                        {
                            Code = district.Code,
                            Name = district.Name,
                            ProvinceCode = province.Code
                        });
                    }

                    foreach (var ward in district.Wards)
                    {
                        if (!await _context.Wards.AnyAsync(w => w.Code == ward.Code))
                        {
                            _context.Wards.Add(new Ward
                            {
                                Code = ward.Code,
                                Name = ward.Name,
                                DistrictCode = district.Code
                            });
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
