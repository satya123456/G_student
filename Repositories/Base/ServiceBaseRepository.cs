using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Gstudent.Repositories.Base
{
    public class ServiceBaseRepository
    {
        //    private static IConfiguration _configuration;
        //  string _dapperconnection = Startup.StaticConfig.GetSection("ConnectionStrings").GetSection("SqlConnection").Value;
        private readonly string _dapperconnection = ConfigurationManager.ConnectionStrings["studentconn"].ConnectionString;
        private readonly string _dapperconnectionMYSQL = ConfigurationManager.ConnectionStrings["studentconnMYSQL"].ConnectionString;
        private readonly string _dapperconnection74exam = ConfigurationManager.ConnectionStrings["studentconn74exam"].ConnectionString;
        private readonly string _dapperconnection240 = ConfigurationManager.ConnectionStrings["studentattendanceconn"].ConnectionString;
        private readonly string _dapperconnection102 = ConfigurationManager.ConnectionStrings["studentattendanceconngimsr"].ConnectionString;
        private readonly string _dapperconnection50 = ConfigurationManager.ConnectionStrings["studentconn50"].ConnectionString;
        private readonly string _dapperconnection60 = ConfigurationManager.ConnectionStrings["studentconn60"].ConnectionString;
        private readonly string _dapperconnection226 = ConfigurationManager.ConnectionStrings["studentconn226"].ConnectionString;
        private readonly string _dapperconnection226gitam = ConfigurationManager.ConnectionStrings["studentconn226gitam"].ConnectionString;
        private readonly string _dapperconnection156 = ConfigurationManager.ConnectionStrings["studentconn156"].ConnectionString;
        private readonly string _dapperconnection52 = ConfigurationManager.ConnectionStrings["studentconn52"].ConnectionString;
        private readonly string _dapperconnection60cdl = ConfigurationManager.ConnectionStrings["studentconn60cdl"].ConnectionString;
        private readonly string _dapperconnection42 = ConfigurationManager.ConnectionStrings["studentconn42"].ConnectionString;
        private readonly string _dapperconnection19 = ConfigurationManager.ConnectionStrings["studentconn19"].ConnectionString;
        private readonly string _dapperconnection60trans = ConfigurationManager.ConnectionStrings["studentconn60trans"].ConnectionString;
        private readonly string _dapperconnection74 = ConfigurationManager.ConnectionStrings["studentconn74"].ConnectionString;
        private readonly string _dapperconnection74vsp = ConfigurationManager.ConnectionStrings["studentconn74vsp"].ConnectionString;
        private readonly string _dapperconnection74blr = ConfigurationManager.ConnectionStrings["studentconn74blr"].ConnectionString;
        private readonly string _dapperconnection81 = ConfigurationManager.ConnectionStrings["studentconn81"].ConnectionString;
        private readonly string _dapperconnection221gitam = ConfigurationManager.ConnectionStrings["studentconn221gitam"].ConnectionString;
        private readonly string _dapperconnection19gitamdb = ConfigurationManager.ConnectionStrings["studentconn19gitamdn"].ConnectionString;
        private readonly string _dapperconnection19hf = ConfigurationManager.ConnectionStrings["studentconn19hf"].ConnectionString;
        private readonly string _dapperconnection99 = ConfigurationManager.ConnectionStrings["studentconn99"].ConnectionString;
        private readonly string _dapperconnection35 = ConfigurationManager.ConnectionStrings["studentconn35"].ConnectionString;
        private readonly string _dapperconnection25 = ConfigurationManager.ConnectionStrings["studentconn25"].ConnectionString;
        private readonly string _dapperconnection60feedback = ConfigurationManager.ConnectionStrings["studentconn60feedback"].ConnectionString;

        private readonly string _dapperconnection60website = ConfigurationManager.ConnectionStrings["studentconn60website"].ConnectionString;
        private readonly string _dapperconnection52hall = ConfigurationManager.ConnectionStrings["studentconn52hall"].ConnectionString;
        private readonly string _dapperconnection24 = ConfigurationManager.ConnectionStrings["studentconn24"].ConnectionString;

        private readonly string _dapperconnection61 = ConfigurationManager.ConnectionStrings["studentconn61"].ConnectionString;
        private readonly string _dapperconnection6OCDL1 = ConfigurationManager.ConnectionStrings["studentconn60CDL1"].ConnectionString;
        private readonly string _dapperconnection19medical = ConfigurationManager.ConnectionStrings["studentconn19medical"].ConnectionString;

        private readonly string _dapperconnection228 = ConfigurationManager.ConnectionStrings["studentconn228"].ConnectionString;

        private readonly string _dapperconnection73= ConfigurationManager.ConnectionStrings["studentconn73"].ConnectionString;
        private readonly string _dapperconnection74btc= ConfigurationManager.ConnectionStrings["studentconn74btc"].ConnectionString;

        public Task<IEnumerable<T>> Query<T>(string sql, object parameters = null)
        {
            using (var connection = CreateConnection())
            {
                return Task.FromResult<IEnumerable<T>>(connection.Query<T>(sql, parameters));
            }
        }

        public IDbConnection CreateConnection()
        {
            var connection = new SqlConnection(_dapperconnection);
            return connection;
        }
        public IDbConnection CreateConnection60()
        {
            var connection = new SqlConnection(_dapperconnection60);
            return connection;
        }


        protected Task<T> QueryFirstOrDefault<T>(string sql, object parameters = null)
        {
            using (var connection = CreateConnection())
            {
                return Task.FromResult<T>(connection.QueryFirstOrDefault<T>(sql, parameters));
            }
        }
        public List<T> GetAllData<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }

        public List<T> GetAllData60website<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection60website))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }

        public T GetSingleData<T>(string query, Object parameters) where T : class
        {
            using (IDbConnection db = new SqlConnection(_dapperconnection))
            {
                return db.Query<T>(query, parameters).SingleOrDefault();
            }
        }

        public SqlDataReader GetDataReader(string query, Object parameters)
        {
            using (var connection = new SqlConnection(_dapperconnection))
            {
                connection.Open();
                SqlDataReader affectedRows = (SqlDataReader)connection.ExecuteReader(query, parameters);
                connection.Close();
                return affectedRows;
            }
        }
        public int Update(string query, Object parameters)
        {
            using (IDbConnection db = new SqlConnection(_dapperconnection))
            {
                db.Open();
                int rowsAffected = db.Execute(query, parameters);
                db.Close();
                return rowsAffected;
            }
        }
        public int InsertData(string query, Object parameters)
        {
            using (var connection = new SqlConnection(_dapperconnection))
            {
                connection.Open();
                var affectedRows = connection.Execute(query, parameters);
                connection.Close();
                return affectedRows;
            }
        }

        public int Delete(string query, Object parameters)
        {
            using (var db = new SqlConnection(_dapperconnection))
            {
                db.Open();
                var affectedRows = db.Execute(query, parameters);
                db.Close();
                return affectedRows;
            }
        }
        public List<T> GetAllData102<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection102))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }
        public List<T> GetAllData240<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection240))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }
        public List<T> GetAllData50<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection50))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }
        public List<T> GetAllData60<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection60))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }

        public int Update60(string query, Object parameters)
        {
            using (IDbConnection db = new SqlConnection(_dapperconnection60))
            {
                db.Open();
                int rowsAffected = db.Execute(query, parameters);
                db.Close();
                return rowsAffected;
            }
        }
        public int Update226(string query, Object parameters)
        {
            using (IDbConnection db = new SqlConnection(_dapperconnection226))
            {
                db.Open();
                int rowsAffected = db.Execute(query, parameters);
                db.Close();
                return rowsAffected;
            }
        }
        public T GetSingleData226gitam<T>(string query, Object parameters) where T : class
        {
            using (IDbConnection db = new SqlConnection(_dapperconnection226gitam))
            {
                return db.Query<T>(query, parameters).SingleOrDefault();
            }
        }
        public int Update226gitam(string query, Object parameters)
        {
            using (IDbConnection db = new SqlConnection(_dapperconnection226gitam))
            {
                db.Open();
                int rowsAffected = db.Execute(query, parameters);
                db.Close();
                return rowsAffected;
            }
        }




        public int InsertData226gitam(string query, Object parameters)
        {
            using (var connection = new SqlConnection(_dapperconnection226gitam))
            {
                connection.Open();
                var affectedRows = connection.Execute(query, parameters);
                connection.Close();
                return affectedRows;
            }
        }

        public List<T> GetAllData226gitam<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection226gitam))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }


        public List<T> GetData64_156<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection156))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }

        public List<T> GetAllData60cdl<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection60cdl))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }
        public List<T> GetAllData52<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection52))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }


        public List<T> GetAllData42<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection42))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }
        public List<T> GetAllData19<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection19))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }


        public List<T> GetAllData19new<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }
        public int Delete226(string query, Object parameters)
        {
            using (var db = new SqlConnection(_dapperconnection226))
            {
                db.Open();
                var affectedRows = db.Execute(query, parameters);
                db.Close();
                return affectedRows;
            }
        }
        public int InsertData226(string query, Object parameters)
        {
            using (var connection = new SqlConnection(_dapperconnection226))
            {
                connection.Open();
                var affectedRows = connection.Execute(query, parameters);
                connection.Close();
                return affectedRows;
            }
        }
        public List<T> GetAllData226<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection226))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }

        public List<T> GetAllData60trans<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection60trans))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }

        public int InsertData60trans(string query, Object parameters)
        {
            using (var connection = new SqlConnection(_dapperconnection60trans))
            {
                connection.Open();
                var affectedRows = connection.Execute(query, parameters);
                connection.Close();
                return affectedRows;
            }
        }

        public List<T> GetAllData74<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection74))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }


        public List<T> GetAllData74vsp<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection74vsp))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }
        public List<T> GetAllData74blr<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection74blr))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }
        public List<T> GetAllData81<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection81))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }


        public T GetSingleData221gitam<T>(string query, Object parameters) where T : class
        {
            using (IDbConnection db = new SqlConnection(_dapperconnection221gitam))
            {
                return db.Query<T>(query, parameters).SingleOrDefault();
            }
        }
        public int Update221gitam(string query, Object parameters)
        {
            using (IDbConnection db = new SqlConnection(_dapperconnection221gitam))
            {
                db.Open();
                int rowsAffected = db.Execute(query, parameters);
                db.Close();
                return rowsAffected;
            }
        }

        public int InsertData221gitamdb(string query, Object parameters)
        {
            using (var connection = new SqlConnection(_dapperconnection221gitam))
            {
                connection.Open();
                var affectedRows = connection.Execute(query, parameters);
                connection.Close();
                return affectedRows;
            }
        }
        public int InsertData221gitamdb_id(string query, Object parameters)
        {
            var connection = new SqlConnection(_dapperconnection221gitam);
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {


                connection.Open();
                int insertedId = connection.ExecuteScalar<int>(query, parameters, commandTimeout: 300);
                connection.Close();
                return insertedId;

            }
        }

        public int InsertData19gitamdb(string query, Object parameters)
        {
            using (var connection = new SqlConnection(_dapperconnection19gitamdb))
            {
                connection.Open();
                var affectedRows = connection.Execute(query, parameters);
                connection.Close();
                return affectedRows;
            }
        }
        public int InsertData19hf(string query, Object parameters)
        {
            using (var connection = new SqlConnection(_dapperconnection19hf))
            {
                connection.Open();
                var affectedRows = connection.Execute(query, parameters);
                connection.Close();
                return affectedRows;
            }
        }


        public List<T> GetAllData19hf<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection19hf))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }

        public int Delete19hf(string query, Object parameters)
        {
            using (var db = new SqlConnection(_dapperconnection19hf))
            {
                db.Open();
                var affectedRows = db.Execute(query, parameters);
                db.Close();
                return affectedRows;
            }
        }
        public int Update19hf(string query, Object parameters)
        {
            using (IDbConnection db = new SqlConnection(_dapperconnection19hf))
            {
                db.Open();
                int rowsAffected = db.Execute(query, parameters);
                db.Close();
                return rowsAffected;
            }
        }


        public T GetSingleData42<T>(string query, Object parameters) where T : class
        {
            using (IDbConnection db = new SqlConnection(_dapperconnection42))
            {
                return db.Query<T>(query, parameters).SingleOrDefault();
            }
        }
        public List<T> GetAllData99<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection99))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }
        public int InsertData99(string query, Object parameters)
        {
            using (var connection = new SqlConnection(_dapperconnection99))
            {
                connection.Open();
                var affectedRows = connection.Execute(query, parameters);
                connection.Close();
                return affectedRows;
            }
        }
        public int Delete99(string query, Object parameters)
        {
            using (var db = new SqlConnection(_dapperconnection99))
            {
                db.Open();
                var affectedRows = db.Execute(query, parameters);
                db.Close();
                return affectedRows;
            }
        }
        public int Update99(string query, Object parameters)
        {
            using (IDbConnection db = new SqlConnection(_dapperconnection99))
            {
                db.Open();
                int rowsAffected = db.Execute(query, parameters);
                db.Close();
                return rowsAffected;
            }
        }
        public Task<IEnumerable<T>> Query60<T>(string sql, object parameters = null)
        {
            using (var connection = CreateConnection60())
            {
                return Task.FromResult<IEnumerable<T>>(connection.Query<T>(sql, parameters));
            }
        }
        public List<T> GetAllData35<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection35))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }


        public IDbConnection CreateConnection25()
        {
            var connection = new SqlConnection(_dapperconnection25);
            return connection;
        }

        public List<T> GetAllData25<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection25))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }



        public int Delete52(string query, Object parameters)
        {
            using (var db = new SqlConnection(_dapperconnection52))
            {
                db.Open();
                var affectedRows = db.Execute(query, parameters);
                db.Close();
                return affectedRows;
            }
        }


        public int InsertData52(string query, Object parameters)
        {
            using (var connection = new SqlConnection(_dapperconnection52))
            {
                connection.Open();
                var affectedRows = connection.Execute(query, parameters);
                connection.Close();
                return affectedRows;
            }
        }

        public int Update52(string query, Object parameters)
        {
            using (IDbConnection db = new SqlConnection(_dapperconnection52))
            {
                db.Open();
                int rowsAffected = db.Execute(query, parameters);
                db.Close();
                return rowsAffected;
            }
        }

        public int Delete25(string query, Object parameters)
        {
            using (var db = new SqlConnection(_dapperconnection25))
            {
                db.Open();
                var affectedRows = db.Execute(query, parameters);
                db.Close();
                return affectedRows;
            }
        }

        public int InsertData25(string query, Object parameters)
        {
            using (var connection = new SqlConnection(_dapperconnection25))
            {
                connection.Open();
                var affectedRows = connection.Execute(query, parameters);
                connection.Close();
                return affectedRows;
            }
        }

        public int Update25(string query, Object parameters)
        {
            using (IDbConnection db = new SqlConnection(_dapperconnection25))
            {
                db.Open();
                int rowsAffected = db.Execute(query, parameters);
                db.Close();
                return rowsAffected;
            }
        }
        public List<T> GetAllData60feedback<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection60feedback))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }
        public int InsertData60feedback(string query, Object parameters)
        {
            using (var connection = new SqlConnection(_dapperconnection60feedback))
            {
                connection.Open();
                var affectedRows = connection.Execute(query, parameters);
                connection.Close();
                return affectedRows;
            }
        }
        public List<T> GetAllData52hall<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection52hall))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }


        public int InsertData52hall(string query, Object parameters)
        {
            using (var connection = new SqlConnection(_dapperconnection52hall))
            {
                connection.Open();
                var affectedRows = connection.Execute(query, parameters);
                connection.Close();
                return affectedRows;
            }
        }



        public List<T> GetAllData61<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection61))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }
        public List<T> GetAllData60CDL<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection6OCDL1))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }
        public int Delete19(string query, Object parameters)
        {
            using (var db = new SqlConnection(_dapperconnection19))
            {
                db.Open();
                var affectedRows = db.Execute(query, parameters);
                db.Close();
                return affectedRows;
            }
        }
        public int InsertData19(string query, Object parameters)
        {
            using (var connection = new SqlConnection(_dapperconnection19))
            {
                connection.Open();
                var affectedRows = connection.Execute(query, parameters);
                connection.Close();
                return affectedRows;
            }
        }
        public int InsertData24(string query, Object parameters)
        {
            using (var connection = new SqlConnection(_dapperconnection24))
            {
                connection.Open();
                var affectedRows = connection.Execute(query, parameters);
                connection.Close();
                return affectedRows;
            }
        }
        public int Update19(string query, Object parameters)
        {
            using (IDbConnection db = new SqlConnection(_dapperconnection19))
            {
                db.Open();
                int rowsAffected = db.Execute(query, parameters);
                db.Close();
                return rowsAffected;
            }
        }
        public List<T> GetAllData228<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection228))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }

        public int Update19medical(string query, Object parameters)
        {
            using (IDbConnection db = new SqlConnection(_dapperconnection19medical))
            {
                db.Open();
                int rowsAffected = db.Execute(query, parameters);
                db.Close();
                return rowsAffected;
            }
        }

        public int InsertData19medical(string query, Object parameters)
        {
            using (var connection = new SqlConnection(_dapperconnection19medical))
            {
                connection.Open();
                var affectedRows = connection.Execute(query, parameters);
                connection.Close();
                return affectedRows;
            }
        }

        public List<T> GetAllData19medical<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection19medical))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }

        public int Insert19(string query, Object parameters)
        {
            using (IDbConnection db = new SqlConnection(_dapperconnection19))
            {
                db.Open();
                int rowsAffected = db.Execute(query, parameters);
                db.Close();
                return rowsAffected;
            }
        }

        public int Deleteorinsert(string query, Object parameters)
        {
            using (var connection = new SqlConnection(_dapperconnection42))
            {
                connection.Open();
                var affectedRows = connection.Execute(query, parameters);
                connection.Close();
                return affectedRows;
            }
        }
        public int InsertData102(string query, Object parameters)
        {
            using (var connection = new SqlConnection(_dapperconnection102))
            {
                connection.Open();
                var affectedRows = connection.Execute(query, parameters);
                connection.Close();
                return affectedRows;
            }
        }
        public int Update102(string query, Object parameters)
        {
            using (IDbConnection db = new SqlConnection(_dapperconnection102))
            {
                db.Open();
                int rowsAffected = db.Execute(query, parameters);
                db.Close();
                return rowsAffected;
            }
        }
        public int Delete102(string query, Object parameters)
        {
            using (var db = new SqlConnection(_dapperconnection102))
            {
                db.Open();
                var affectedRows = db.Execute(query, parameters);
                db.Close();
                return affectedRows;
            }
        }
        public T GetSingleData102<T>(string query, Object parameters) where T : class
        {
            using (IDbConnection db = new SqlConnection(_dapperconnection102))
            {
                return db.Query<T>(query, parameters).SingleOrDefault();
            }
        }


        public List<T> GetAllData73<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection73))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }

        public T GetSingleData52hall<T>(string query, Object parameters) where T : class
        {
            using (IDbConnection db = new SqlConnection(_dapperconnection52hall))
            {
                return db.Query<T>(query, parameters).SingleOrDefault();
            }
        }



        public int Update52hall(string query, Object parameters)
        {
            using (IDbConnection db = new SqlConnection(_dapperconnection52hall))
            {
                db.Open();
                int rowsAffected = db.Execute(query, parameters);
                db.Close();
                return rowsAffected;
            }
        }
        public List<T> GetAllData74exam<T>(string query, Object parameters) where T : class
        {
            List<T> Data = new List<T>();
            using (var connection = new SqlConnection(_dapperconnection74exam))
            {

                Data = connection.Query<T>(query, parameters).ToList();

            }
            return Data;
        }

      


        public T GetAllData74BELLTHECATS<T>(string query, Object parameters) where T : class
        {
            using (IDbConnection db = new SqlConnection(_dapperconnection74btc))
            {
                return db.Query<T>(query, parameters).SingleOrDefault();
            }
        }
        public int InsertData42(string query, Object parameters)
        {
            using (var connection = new SqlConnection(_dapperconnection42))
            {
                connection.Open();
                var affectedRows = connection.Execute(query, parameters);
                connection.Close();
                return affectedRows;
            }
        }
        public int Update42(string query, Object parameters)
        {
            using (IDbConnection db = new SqlConnection(_dapperconnection42))
            {
                db.Open();
                int rowsAffected = db.Execute(query, parameters);
                db.Close();
                return rowsAffected;
            }
        }
        public int Delete42(string query, Object parameters)
        {
            using (var db = new SqlConnection(_dapperconnection42))
            {
                db.Open();
                var affectedRows = db.Execute(query, parameters);
                db.Close();
                return affectedRows;
            }
        }
    }
}


