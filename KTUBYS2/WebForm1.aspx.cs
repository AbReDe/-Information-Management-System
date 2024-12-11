using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using KTUBYS2.classes;
using System.Data;

namespace KTUBYS2
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        


        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected void Button1_Click1(object sender, EventArgs e)
        {
            SqlCommand commandlogin = new SqlCommand("Select * From Users where UserID=@userid and PasswordHash=@pasvord", Sqlconnectionss.connection);

            Sqlconnectionss.chekconnection();

            commandlogin.Parameters.AddWithValue("@userid", TextBox1.Text);
            commandlogin.Parameters.AddWithValue("@pasvord", TextBox2.Text);

            DataTable dt = new DataTable();

            SqlDataAdapter da = new SqlDataAdapter(commandlogin);

            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                Response.Write("girişyapıldu");

            }
            else
            {
                Response.Write("Id veya şifre yanlıs");
            }

        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        protected void TextBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}