using Dapper;
using Discount.Core.Entities;
using Discount.Core.Repositories;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Discount.Infrastructure.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;

        private const string CONNECTION = "DatabaseSettings:ConnectionString";

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            await using var connection = new NpgsqlConnection(_configuration.GetValue<string>(CONNECTION));

            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>(
                "SELECT * FROM Coupon WHERE ProductName = @ProductName", new { ProductName = productName }
            );

            if (coupon is null)
            {
                return new Coupon { ProductName = "No Discount" };
            }

            return coupon;
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            await using var connection = new NpgsqlConnection(_configuration.GetValue<string>(CONNECTION));

            var affectedRows = await connection.ExecuteAsync(
                "INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
                coupon
            );

            if (affectedRows == 0)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            await using var connection = new NpgsqlConnection(_configuration.GetValue<string>(CONNECTION));

            var affectedRows = await connection.ExecuteAsync(
                "UPDATE Coupon SET ProductName = @ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id",
                coupon
            );

            if (affectedRows == 0)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            await using var connection = new NpgsqlConnection(_configuration.GetValue<string>(CONNECTION));

            var affectedRows = await connection.ExecuteAsync(
                "DELETE FROM Coupon WHERE ProductName = @ProductName",
                new { ProductName = productName }
            );

            if (affectedRows == 0)
            {
                return false;
            }

            return true;
        }
    }
}
