using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Repository
{
    public class DonorRepository: IDonorRepository
    {
        private readonly StoreContext context;

        public DonorRepository(StoreContext context)
        {
            this.context = context;
        }
        public async Task<bool> CreateDonorAsync(Donor donor)
        {
            context.donors.Add(donor);
            await context.SaveChangesAsync();
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteDonorAsync(int id)
        {
            var donor = await context.donors.FindAsync(id);
            if (donor == null)
                return false;
            context.donors.Remove(donor);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Donor>> GetAllDonorsAsync()
        {
            return await context.donors.ToListAsync();
        }

        public async Task<bool> UpdateDonorAsync(Donor donor)
        {
            var d = await context.donors.FindAsync(donor.Id);
            if (d == null)
                return false;
            d.Phone = donor.Phone;
            d.Email = donor.Email;
            d.FirstName = donor.FirstName;
            d.LastName = donor.LastName;
            d.Donations = donor.Donations;
            d.Id = donor.Id;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Donor> GetById(int id)
        {
            var d = await context.donors.FindAsync(id);
            return d;
        }

        public async Task<Donor> GetByName(string name)
        {
            var d = await context.donors
                .Where(d => d.FirstName + " " + d.LastName == name)
                .FirstOrDefaultAsync();
            return d;
        }

        public async Task<Donor> GetByEmail(string email)
        {
            var d = await context.donors
              .Where(d => d.Email == email)
              .FirstOrDefaultAsync();
            return d;
        }

        public async Task<Donor> GetByGift(string giftName)
        {
            var d = await context.gifts
                .Where(g => g.Name == giftName)
                .Select(g => g.Donor)
                .FirstOrDefaultAsync();
            return d;
        }

        public async Task<bool> AddDonation(int id,int giftId)
        {
            var donor = await context.donors.FindAsync(id);
            if (donor == null)
                return false;
            var gift =  await context.gifts.FindAsync(giftId);
            if (gift == null)
                return false;
            donor.Donations.Add(gift);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
