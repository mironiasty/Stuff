using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Reflection;
using System.Text;
using System.Collections.Generic;

namespace RandomStuff
{
    public class SqlHelper
    {
        public static SqlConnection SetUpConnection()
        {
            ConnectionStringSettings connString = ConfigurationManager.ConnectionStrings["ConnectionString"];
            if (connString == null || string.IsNullOrEmpty(connString.ConnectionString))
                throw new Exception("No 'ConnectionString' found in .config file");

            return new SqlConnection(connString.ConnectionString);
        }

        private static SqlCommand GetCommand(string sprocName)
        {
            SqlConnection connection;
            SqlCommand command;

            connection = SqlHelper.SetUpConnection();
            command = new SqlCommand(sprocName);
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;

            return command;
        }


        private static void UpdateParameters(ref SqlCommand command, dynamic parameters)
        {

            foreach (var prop in parameters.GetType().GetProperties())
            {
                var value = prop.GetValue(parameters, null);
                if (value == null)
                {
                    value = DBNull.Value;
                }
                command.Parameters.AddWithValue("@" + ConvertNotation(prop.Name), value);
            }

        }

        /// <summary>
        /// Save some data using stored procedure
        /// </summary>
        /// <param name="sprocName"></param>
        /// <param name="parameters"></param>
        /// <returns>true if at least one record was modified with this query</returns>
        public static bool SaveData(string sprocName, dynamic parameters)
        {

            SqlCommand command = GetCommand(sprocName);
            UpdateParameters(ref command, parameters);


            command.Connection.Open();
            int rowsChanged = command.ExecuteNonQuery();
            command.Connection.Close();

            return rowsChanged > 0;
        }

        /// <summary>
        /// Get values from first row returned by stored procedure
        /// </summary>
        /// <param name="sprocName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IDictionary<string, object> GetRow(string sprocName, dynamic parameters)
        {


            SqlCommand command = GetCommand(sprocName);
            UpdateParameters(ref command, parameters);

            command.Connection.Open();
            IDataReader reader = command.ExecuteReader();
            IDictionary<string, object> dict = null;
            if (reader.Read())
            {
                dict = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    dict.Add(reader.GetName(i), reader.GetValue(i));
                }
            }

            command.Connection.Close();

            return dict;
        }

        /// <summary>
        /// Get rows returned by given stored procedure
        /// </summary>
        /// <param name="sprocName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IList<IDictionary<string, object>> GetRows(string sprocName, dynamic parameters)
        {

            SqlCommand command = GetCommand(sprocName);
            UpdateParameters(ref command, parameters);

            var results = new List<IDictionary<string, object>>();

            command.Connection.Open();
            IDataReader reader = command.ExecuteReader();
            IDictionary<string, object> dict = null;
            while (reader.Read())
            {
                dict = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    dict.Add(reader.GetName(i), reader.GetValue(i));
                }
                results.Add(dict);
            }

            command.Connection.Close();

            return results;
        }

        /// <summary>
        /// Query DB for scalar value using given sproc with parameters
        /// </summary>
        /// <typeparam name="T">Type of value returned by sproc</typeparam>
        /// <param name="sprocName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static T? GetScalar<T>(string sprocName, dynamic parameters) where T : struct
        {
            SqlCommand command = GetCommand(sprocName);
            UpdateParameters(ref command, parameters);

            command.Connection.Open();
            object result = command.ExecuteScalar();
            command.Connection.Close();

            return result as T?;
        }

        /// <summary>
        /// Convert notations from nameOfParameter to NAME_OF_PARAMETER
        /// </summary>
        /// <param name="name">nameOfParameter</param>
        /// <returns>NAME_OF_PARAMETER</returns>
        private static string ConvertNotation(string name)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char letter in name.ToCharArray())
            {
                if (letter.CompareTo('A') >= 0 && letter.CompareTo('Z') <= 0)
                {
                    sb.Append('_');
                }
                sb.Append(letter);
            }
            return sb.ToString().ToUpper();
        }

        /// <summary>
        /// Convert notation from NAME_OF_PARAMTER to nameOfParameter
        /// </summary>
        /// <param name="name">NAME_OF_PARAMTER</param>
        /// <returns>nameOfParameter</returns>
        private static string ConvertNotationBackward(string name)
        {
            StringBuilder sb = new StringBuilder();
            bool nextUpper = false;
            foreach (char letter in name.ToLower().ToCharArray())
            {
                if ('_' == letter)
                    nextUpper = true;
                else
                    if (!nextUpper || (nextUpper = false)) //+1 for obscurity :P
                        sb.Append(letter);
                    else
                        sb.Append(char.ToUpper(letter));
            }
            return sb.ToString().ToUpper();
        }

    }
}
