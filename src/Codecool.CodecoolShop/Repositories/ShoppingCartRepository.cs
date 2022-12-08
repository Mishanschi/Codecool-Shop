using Codecool.CodecoolShop.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;

namespace Codecool.CodecoolShop.Repositories
{
    public class ShoppingCartRepository : BaseRepository
    {
        //private SqlConnection connection;

        public ShoppingCartRepository(IConfiguration _configuration) : base(_configuration)
        {
        }



        
        public void InsertIntoShoppingCart(Guid id,string userId)
        {
            bool productAlreadyExistsInCart = CheckIfProductExistsInCart(id,userId);
            string sql = "";
            if (productAlreadyExistsInCart)
            {
                sql = "update ShoppingCart set quantity = quantity +1 where productId = @id";
            }
            else
            {

                sql = "insert into ShoppingCart(productId, quantity,userId) values(@id, 1,@userId)";
            }
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("id", id);
                command.Parameters.AddWithValue("userId", userId);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private bool CheckIfProductExistsInCart(Guid id,string userId)
        {
            var sql = "select sp.productId,sp.userId from ShoppingCart sp";
            var productIds = new List<(Guid,string)>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                productIds = connection.Query<(Guid,string)>(sql).ToList();
            }
            foreach (var eachId in productIds)
            {
                if (eachId.Item1 == id && eachId.Item2==userId)
                {
                    return true;
                }
            }
            return false;
        }

        public List<ShoppingCart> GetAllProductsfromCart(string userId)
        {
            var sql = $"select p.name ,p.id, p.defaultPrice , sp.quantity , (p.defaultPrice * sp.quantity) as subtotal from Product p\r\nleft join ShoppingCart sp on p.id = sp.productId\r\nwhere sp.quantity is not null and sp.userId like '{userId}'\r\norder by defaultPrice";
            var products = new List<ShoppingCart>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                products = connection.Query<ShoppingCart>(sql).ToList();
            }
            return products;
        }

        public void IncreaseQuantity(Guid id)
        {
            string sql = "update ShoppingCart set quantity = quantity +1 where productId = @id";
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("id", id);
                command.ExecuteNonQuery();
            }

        }

        public void DecreaseQuantity(Guid id)
        {

            string sql = "update ShoppingCart set quantity = quantity -1 where productId = @id";
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("id", id);
                command.ExecuteNonQuery();
            }
            CheckForQuantity();

        }

        public void CheckForQuantity()
        {
            var sql = "delete from ShoppingCart where quantity = 0 ";
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
            }
        }



    }
}