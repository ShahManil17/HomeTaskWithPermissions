using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace HomeTaskWithPermissions
{
    public class Services
    {
        public static void list_all_users(SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand("getAllUsers", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader data = cmd.ExecuteReader())
            {
                int fields = data.FieldCount;
                while (data.Read())
                {
                    for (int i = 0; i < fields; i++)
                    {
                        Console.Write("{0} : {1}\t", data.GetName(i), data[data.GetName(i)]);
                    }
                    Console.Write("\n");
                }
            }
        }

        public static void edit_user_details(SqlConnection conn)
        {

        }

        public static void add_new_user(SqlConnection conn)
        {
            Console.Write("Ente Name : ");
            string add_name = Console.ReadLine();
            Console.Write("Enter Surname : ");
            string add_surname = Console.ReadLine();
            Console.Write("Enter Age : ");
            int add_age = Convert.ToInt32(Console.ReadLine());
            Console.Write("Enter Gender (m/f) : ");
            char add_gender = Convert.ToChar(Console.ReadLine());
            Console.Write("Enter Contact No. : ");
            string add_no = Console.ReadLine();
            Console.Write("Enter e-mail address : ");
            string add_email = Console.ReadLine();
            Console.Write("Enter Role (admin, manager Or user) : ");
            string role = Console.ReadLine();
            Console.Write("Enter Pemissions Saperated by coma (,) : ");
            string[] permissions = Console.ReadLine().Split(',');
            Console.Write("Enter Password of the User : ");
            string add_pass = Console.ReadLine();

            foreach (var item in permissions)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();
            Console.WriteLine();

            bool permissionFlag = true;
            SqlCommand cmd = new SqlCommand("SELECT name FROM permissions", conn);
            foreach (var item in permissions)
            {
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    bool tempFlag = true;
                    while (rd.Read())
                    {
                        if (item == Convert.ToString(rd["name"]))
                        {
                            tempFlag = false;
                        }
                    }
                    if(tempFlag)
                    {
                        permissionFlag = false;
                        break;
                    }
                }
            }

            if ((role == "admin" || role == "manager" || role == "user") && permissionFlag)
            {
                Console.WriteLine("Everything is good ..");
                SqlCommand addCmd = new SqlCommand("addUser", conn);
                addCmd.CommandType = CommandType.StoredProcedure;
                addCmd.Parameters.AddWithValue("@new_name", add_name);
                addCmd.Parameters.AddWithValue("@new_surname", add_surname);
                addCmd.Parameters.AddWithValue("@new_age", add_age);
                addCmd.Parameters.AddWithValue("@new_gender", add_gender);
                addCmd.Parameters.AddWithValue("@new_no", add_no);
                addCmd.Parameters.AddWithValue("@new_email", add_email);
                addCmd.Parameters.AddWithValue("@pass", add_pass);

                int insertResult = addCmd.ExecuteNonQuery();

                int role_id = 0;
                SqlCommand roleCmd = new SqlCommand("SELECT role_id FROM roles WHERE name = @name", conn);
                roleCmd.Parameters.AddWithValue("@name", role);
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while(rd.Read())
                    {
                        role_id = Convert.ToInt32(rd["role_id"]);
                    }
                }

                int[] permission_id = new int[permissions.Length];

                for(int i=0; i<permissions.Length; i++)
                {
                    SqlCommand permissionCmd = new SqlCommand("SELECT permission_id FROM permissions WHERE name = @name", conn);
                    roleCmd.Parameters.AddWithValue("@name", permissions[i]);
                    using (SqlDataReader rd = permissionCmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            permission_id[i] = Convert.ToInt32(rd["permission_id"]);
                        }
                    }
                }

                SqlCommand getId = new SqlCommand("SELECT SCOPE_IDENTITY()");
                int emp_id = Convert.ToInt32(getId.ExecuteScalar());
                Console.WriteLine(emp_id);

                foreach (var item in permission_id)
                {
                    Services.permission(conn, emp_id, role_id, item);
                }

            }
            else
            {
                Console.WriteLine("Invalid role or permission entered!");
            }
        }

        public static void list_personal_details(SqlConnection conn)
        {

        }

        public static void edit_personal_details(SqlConnection conn)
        {

        }

        public static void assign_roles(SqlConnection conn)
        {

        }

        public static void assign_permissions(SqlConnection conn)
        {
            Console.Write("Enter the ID of the Employee : ");
            int emp_id = Convert.ToInt32(Console.ReadLine());
            SqlCommand roleCmd = new SqlCommand("SELECT role_id FROM users WHERE id = @emp_id", conn);
            roleCmd.Parameters.AddWithValue("id", emp_id);

            int role_id = 0;
            using (SqlDataReader rd = roleCmd.ExecuteReader())
            {
                while(rd.Read())
                {
                    role_id = Convert.ToInt32(rd["role_id"]);
                }
            }
            Console.Write("Enter Pemissions Saperated by coma (,) : ");
            string[] permissions = Console.ReadLine().Split(',');

            int[] permission_id = new int[permissions.Length];

            for (int i = 0; i < permissions.Length; i++)
            {
                SqlCommand permissionCmd = new SqlCommand("SELECT permission_id FROM permissions WHERE name = @name", conn);
                roleCmd.Parameters.AddWithValue("@name", permissions[i]);
                using (SqlDataReader rd = permissionCmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        permission_id[i] = Convert.ToInt32(rd["permission_id"]);
                    }
                }
            }

            //================= remaining ================

        }

        public static void permission(SqlConnection conn, int user_id, int role_id, int permission_id)
        {
            SqlCommand cmd = new SqlCommand("assignPermission", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue ("@user_id", user_id);
            cmd.Parameters.AddWithValue("@role_id", role_id);
            cmd.Parameters.AddWithValue("@permission_id", permission_id);
            int result = cmd.ExecuteNonQuery();
        }
    }

    public class Authntication
    {
        public static bool auth(SqlConnection conn, int id, string pass)
        {
            SqlCommand cmd = new SqlCommand("SELECT password FROM users WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);
            using (SqlDataReader rd = cmd.ExecuteReader())
            {
                bool val = false;
                while(rd.Read())
                {
                    if (pass == Convert.ToString(rd["password"]))
                    {
                        val = true;
                    }
                    else
                    {
                        val = false;
                    }
                }
                return val;
            }
        }
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            SqlConnection conn = null;
            string cs = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

            try
            {
                conn = new SqlConnection(cs);
            }
            catch
            {
                Console.WriteLine("Sorry but we are having difficulties accesing the database!");
            }
            
            if(conn != null)
            {
                Console.Write("Enter Your ID : ");
                int emp_id = Convert.ToInt32(Console.ReadLine());
                Console.Write("Enter Your Password : ");
                string pass = Console.ReadLine();

                conn.Open();
                try
                {
                    if(Authntication.auth(conn, emp_id, pass))
                    {
                        SqlCommand getPermissions = new SqlCommand("getPermissions", conn);
                        getPermissions.CommandType = CommandType.StoredProcedure;
                        getPermissions.Parameters.AddWithValue("id", emp_id);
                        int choice;
                        do
                        {
                            SqlDataReader rd = getPermissions.ExecuteReader();

                            Dictionary<int, string> pair = new Dictionary<int, string>();
                            
                            for (int i = 1; rd.Read(); i++)
                            {
                                Console.WriteLine("{0}) {1}", i, rd["name"]);
                                pair.Add(i, Convert.ToString(rd["name"]));
                            }
                            Console.WriteLine("0) Quit");
                            Console.Write("Enter Your Option : ");
                            choice = Convert.ToInt32(Console.ReadLine());
                            rd.Close();
                            if (choice != 0)
                            {
                                Console.WriteLine(pair[choice]);
                                var type = typeof(Services);
                                var method = type.GetMethod(pair[choice]);
                                object[] param = { conn };
                                method.Invoke(null, param);
                            }
                        } while (choice != 0);
                    }
                    else
                    {
                        Console.WriteLine("Invalid Credentials!");
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}
