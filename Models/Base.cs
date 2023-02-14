using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace Hotel_Management_MVC.Models
{
    public class Base
    {
        protected string BaseConnection
        {
            get
            {
                var configurationBuilder = new ConfigurationBuilder();
                var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                configurationBuilder.AddJsonFile(path, false);
                var root = configurationBuilder.Build();
                var connectionString = root.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
                return connectionString;
            }
        }

        public static string EncryptPassword(string password)
        {
            StringBuilder encryptedPassword = new StringBuilder(password);
            for (int i = 0; i < password.Length; i++)
            {
                if (i % 2 == 0)
                {
                    encryptedPassword[i] = (char)(((int)password[i]) + 1);
                }
                else
                {
                    encryptedPassword[i] = (char)(((int)password[i]) - 1);
                }
            }
            return encryptedPassword.ToString();
        }

        public static string DecryptPassword(string password)
        {
            StringBuilder encryptedPassword = new StringBuilder(password);
            for (int i = 0; i < password.Length; i++)
            {
                if (i % 2 != 0)
                {
                    encryptedPassword[i] = (char)(((int)password[i]) + 1);
                }
                else
                {
                    encryptedPassword[i] = (char)(((int)password[i]) - 1);
                }
            }
            return encryptedPassword.ToString();
        }

        public DataTable GetDataTable(string query, List<SqlParameter> parameters, CommandType type)
        {
            DataTable _dt = new DataTable();
            using (var con = new SqlConnection(BaseConnection))
            {
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = type;
                    if (parameters != null)
                    {
                        foreach (SqlParameter _para in parameters)
                        {
                            cmd.Parameters.Add(_para);
                        }
                        con.Open();
                        using (var _adpter = new SqlDataAdapter(cmd))
                        {
                            _adpter.Fill(_dt);
                        }
                    }
                }
            }
            return _dt;
        }
    }
}
