using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.Runtime.InteropServices;

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
            Console.Write("Enter Id Of the Emloyee : ");
            int emp_id = Convert.ToInt32(Console.ReadLine());
            if(Validation.validateId(conn, emp_id))
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
                Console.Write("Enter Role : ");
                string role = Console.ReadLine();

                if(role == "admin" || role == "manager" || role == "user")
                {
                    Console.Write("Enter New Password : ");
                    string pass = Console.ReadLine();

                    SqlCommand cmd = new SqlCommand("SELECT id FROM roles WHERE name = @name", conn);
                    cmd.Parameters.AddWithValue("@name", role);
                    int role_id = (int) cmd.ExecuteScalar();

                    SqlCommand updateDetails = new SqlCommand("updateDetails", conn);
                    updateDetails.CommandType = CommandType.StoredProcedure;
                    updateDetails.Parameters.AddWithValue("@id", emp_id);
                    updateDetails.Parameters.AddWithValue("@name", add_name);
                    updateDetails.Parameters.AddWithValue("@surname", add_surname);
                    updateDetails.Parameters.AddWithValue("@age", add_age);
                    updateDetails.Parameters.AddWithValue("@gender", add_gender);
                    updateDetails.Parameters.AddWithValue("@no", add_no);
                    updateDetails.Parameters.AddWithValue("@email", add_email);
                    updateDetails.Parameters.AddWithValue("@role", role_id);
                    updateDetails.Parameters.AddWithValue("@pass", pass);

                    int updateResult = updateDetails.ExecuteNonQuery();

                    Console.WriteLine();
                    Console.WriteLine("Data Updated Successfully!");
                }
                else
                {
                    Console.WriteLine("Invalid Role Entered!");
                }
            }
            else
            {
                Console.WriteLine("Entered ID is not registered under the System!");
            }
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

            if(role== "admin" || role == "manager" || role == "user")
            {
                Console.Write("Enter Pemissions Saperated by coma (,) : ");
                string[] permissions = Console.ReadLine().Split(',');
                
                if(Validation.validatePermissions(conn, permissions))
                {
                    Console.Write("Enter Password of the User : ");
                    string add_pass = Console.ReadLine();

                    int role_id = 0;
                    SqlCommand roleCmd = new SqlCommand("SELECT id FROM roles WHERE name = @name", conn);
                    roleCmd.Parameters.AddWithValue("@name", role);
                    using (SqlDataReader rd = roleCmd.ExecuteReader())
                    {
                        while(rd.Read())
                        {
                            role_id = Convert.ToInt32(rd["id"]);
                        }
                    }


                    SqlCommand addCmd = new SqlCommand("addUser", conn);
                    addCmd.CommandType = CommandType.StoredProcedure;
                    addCmd.Parameters.AddWithValue("@name", add_name);
                    addCmd.Parameters.AddWithValue("@surname", add_surname);
                    addCmd.Parameters.AddWithValue("@age", add_age);
                    addCmd.Parameters.AddWithValue("@gender", add_gender);
                    addCmd.Parameters.AddWithValue("@no", add_no);
                    addCmd.Parameters.AddWithValue("@email", add_email);
                    addCmd.Parameters.AddWithValue("@role", role_id);
                    addCmd.Parameters.AddWithValue("@pass", add_pass);

                    int emp_id = Convert.ToInt32(addCmd.ExecuteScalar());

                    int[] permission_id = new int[permissions.Length];

                    for(int i=0; i<permissions.Length; i++)
                    {
                        SqlCommand permissionCmd = new SqlCommand("SELECT id FROM permissions WHERE name = @name", conn);
                        permissionCmd.Parameters.AddWithValue("@name", permissions[i]);
                        using (SqlDataReader rd = permissionCmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                permission_id[i] = Convert.ToInt32(rd["id"]);
                            }
                        }
                    }
                    //list_personal_details,edit_personal_details
                    
                    foreach (var item in permission_id)
                    {
                    SqlCommand addPrmission = new SqlCommand("assignPermission", conn);
                    addPrmission.CommandType = CommandType.StoredProcedure;
                    addPrmission.Parameters.AddWithValue("@user_id", emp_id);
                    addPrmission.Parameters.AddWithValue("@permission_id", item);

                    int permissionResult = addPrmission.ExecuteNonQuery();
                    }
                    Console.WriteLine("Data Inserted Successfully!");
                }
                else
                {
                    Console.WriteLine("Invalid Permission Entered!");
                }
            }
            else
            {
                Console.WriteLine("Invalid Role Entered!");
            }
        }

        public static void list_personal_details(SqlConnection conn)
        {
            Console.WriteLine();
            Console.Write("Enter the ID of the Employee : ");
            int emp_id = Convert.ToInt32(Console.ReadLine());
            if(Validation.validateId(conn, emp_id))
            {
                SqlCommand cmd = new SqlCommand("getOneUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("id", emp_id);
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
            else
            {
                Console.WriteLine("Entered ID is not available in the system!");
            }
        }

        public static void edit_personal_details(SqlConnection conn)
        {

        }

        public static void romove_permission(SqlConnection conn)
        {
            Console.WriteLine();
            Console.Write("Enter Id of the user : ");
            int emp_id = Convert.ToInt32(Console.ReadLine());
            if(Validation.validateId(conn, emp_id))
            {
                Console.Write("Enter Pemissions Saperated by coma (,) : ");
                string[] input_permissions = Console.ReadLine().Split(',');

                if(Validation.validatePermissions(conn, input_permissions))
                {
                    SqlCommand countQ = new SqlCommand("SELECT count(*) AS count FROM permissions WHERE id IN (SELECT permission_id FROM user_has_permissions WHERE user_id = @user_id)", conn);
                    countQ.Parameters.AddWithValue("@user_id", emp_id);
                    int permission_count = Convert.ToInt32(countQ.ExecuteScalar());

                    string[] user_permissions = new string[permission_count];

                    SqlCommand avilable_permissionQ = new SqlCommand("SELECT name FROM permissions WHERE id IN (SELECT permission_id FROM user_has_permissions WHERE user_id = @user_id)", conn);
                    avilable_permissionQ.Parameters.AddWithValue("@user_id", emp_id);
                    SqlDataReader avail_permission = avilable_permissionQ.ExecuteReader();
                    //Console.WriteLine("--------------");
                    //while (avail_permission.Read())
                    //{
                    //    Console.WriteLine(avail_permission["name"]);
                    //}
                    //Console.WriteLine("--------------");

                    int count = 0;
                    while(avail_permission.Read())
                    {
                        //Console.WriteLine("---->{0}", Convert.ToString(avail_permission["name"]));
                        user_permissions[count] = Convert.ToString(avail_permission["name"]);
                        count++;
                    }
                    avail_permission.Close();
                    foreach (var item in input_permissions)
                    {
                        if(user_permissions.Contains(item))
                        {
                            SqlCommand permissionIdQ = new SqlCommand("SELECT id FROM permissions WHERE name = @name", conn);
                            permissionIdQ.Parameters.AddWithValue("@name", item);
                            int permission_id = Convert.ToInt32(permissionIdQ.ExecuteScalar());
                            Console.WriteLine("--{0}--{1}", emp_id, permission_id);
                            SqlCommand removePermission = new SqlCommand("removePermission", conn);
                            removePermission.CommandType = CommandType.StoredProcedure;
                            removePermission.Parameters.AddWithValue("@user_id", emp_id);
                            removePermission.Parameters.AddWithValue("@permission_id", permission_id);

                            int removeResult = removePermission.ExecuteNonQuery();

                            Console.WriteLine("Permission '{0}' Removed SuccessFully", item);
                        }
                        else
                        {
                            Console.WriteLine("Permission '{0}' is not assigned to the user", item);
                        }
                    }
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Invalid Permission/s Entered!");
                }
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Entered Id is not registered unnder the system!");
            }
        }

        public static void assign_permissions(SqlConnection conn)
        {
            Console.WriteLine("1) Assign Permissions to the User");
            Console.WriteLine("2) Assign Permissions to the Role");
            Console.Write("Enter Your Choice (1  Or 2): ");
            int choice = Convert.ToInt32(Console.ReadLine());
            switch(choice)
            {
                case 1:
                    Console.Write("Enter the ID of the Employee : ");
                    int emp_id = Convert.ToInt32(Console.ReadLine());

                    if(Validation.validateId(conn, emp_id))
                    {
                        Console.Write("Enter Pemissions Saperated by coma (,) : ");
                        string[] permissions = Console.ReadLine().Split(',');

                        if(Validation.validatePermissions(conn, permissions))
                        {
                            int[] permission_id = new int[permissions.Length];
                            Console.WriteLine();
                            Console.WriteLine("Permission ID : {0}", permission_id.Length);
                            Console.WriteLine("Permissions : {0}", permissions.Length);
                            Console.WriteLine();

                            for (int i = 0; i < permissions.Length; i++)
                            {
                                SqlCommand permissionCmd = new SqlCommand("SELECT id FROM permissions WHERE name = @name", conn);
                                permissionCmd.Parameters.AddWithValue("@name", permissions[i]);
                                using (SqlDataReader rd = permissionCmd.ExecuteReader())
                                {
                                    //list_all_users,edit_user_details,add_new_user
                                    while (rd.Read())
                                    {
                                        permission_id[i] = Convert.ToInt32(rd["id"]);

                                    }
                                }
                            }

                            foreach (var item in permission_id)
                            {
                                SqlCommand addPrmission = new SqlCommand("assignPermission", conn);
                                addPrmission.CommandType = CommandType.StoredProcedure;
                                addPrmission.Parameters.AddWithValue("@user_id", emp_id);
                                addPrmission.Parameters.AddWithValue("@permission_id", item);   

                                int permissionResult = addPrmission.ExecuteNonQuery();
                            }
                            Console.WriteLine("Permission Granted Successfully!");
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("Invalid Permission/s entered!");
                        }

                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("Entered id is not registered in the  system!");
                        Console.WriteLine();
                    }
                    break;

                case 2:
                    Console.WriteLine();
                    Console.Write("Enter the Role : ");
                    string role = Console.ReadLine();
                    if(role == "admin" || role == "manager" || role == "user")
                    {
                        Console.Write("Enter Pemissions Saperated by coma (,) : ");
                        string[] permissions = Console.ReadLine().Split(',');
                        if(Validation.validatePermissions(conn, permissions))
                        {
                            int[] permission_id = new int[permissions.Length];

                            for (int i = 0; i < permissions.Length; i++)
                            {
                                SqlCommand permissionCmd = new SqlCommand("SELECT id FROM permissions WHERE name = @name", conn);
                                permissionCmd.Parameters.AddWithValue("@name", permissions[i]);
                                using (SqlDataReader rd = permissionCmd.ExecuteReader())
                                {
                                    while (rd.Read())
                                    {
                                        permission_id[i] = Convert.ToInt32(rd["id"]);
                                    }
                                }
                            }

                            SqlCommand roleCmd = new SqlCommand("SELECT id FROM users WHERE role_id IN (SELECT id FROM roles WHERE name = @role)", conn);
                            roleCmd.Parameters.AddWithValue("@role", role);
                            SqlDataReader data = roleCmd.ExecuteReader();
                            int[] user_id = new int[data.FieldCount];
                            int user_id_count = 0;
                            while(data.Read())
                            {
                                user_id[user_id_count] = (Int32)data["id"];
                                user_id_count++;
                            }
                            data.Close();
                            foreach (var userItem in user_id)
                            {
                                foreach (var permissionItem in permission_id)
                                {
                                    SqlCommand addPrmission = new SqlCommand("assignPermission", conn);
                                    addPrmission.CommandType = CommandType.StoredProcedure;
                                    addPrmission.Parameters.AddWithValue("@user_id", userItem);
                                    addPrmission.Parameters.AddWithValue("@permission_id", permissionItem);

                                    int permissionResult = addPrmission.ExecuteNonQuery();
                                }
                            }

                            Console.WriteLine("Permission Granted Successfully!");
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("Invalid Permission/s entered!");
                        }

                    }
                    else
                    {
                        Console.WriteLine("Invalid Role Entered!");
                    }
                    break;
                default:
                    Console.WriteLine("Invalid Option!");
                    break;
            }
        }

        //public static void permission(SqlConnection conn, int user_id, int permission_id)
        //{
        //    SqlCommand cmd = new SqlCommand("assignPermission", conn);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue ("@user_id", user_id);
        //    cmd.Parameters.AddWithValue("@permission_id", permission_id);
        //    int result = cmd.ExecuteNonQuery();
        //}
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

    public class Validation
    {
        public static bool validateId(SqlConnection conn, int id)
        {
            SqlCommand idCheck = new SqlCommand("SELECT id FROM users", conn);
            using (SqlDataReader rd = idCheck.ExecuteReader())
            {
                while(rd.Read())
                {
                    if(id == (Int32)rd["id"])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool validatePermissions(SqlConnection conn, string[] permissions)
        {
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
                    if (tempFlag)
                    {
                        permissionFlag = false;
                        break;
                    }
                }
            }
            return permissionFlag;
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
                        getPermissions.Parameters.AddWithValue("@id", emp_id);
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
            Console.WriteLine();
            Console.WriteLine("Thank You For Using Our System!");
            Console.ReadKey();
        }
    }
}
