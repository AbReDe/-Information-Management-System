using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace KTUBYS2.classes
{
    public class Sqlconnectionss
    {
        public static SqlConnection connection = new SqlConnection("Data Source=XERA\\SQLEXPRESS;Initial Catalog=UniversityCourseSelection;User ID=sa;Password=1234");


        public static void chekconnection()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
            else
            {

            }

        }
    }
}