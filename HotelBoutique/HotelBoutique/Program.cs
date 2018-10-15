using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;

namespace HotelBoutique
{
    class Program
    {
        static String connectionString = ConfigurationManager.ConnectionStrings["conexionHotelBoutique"].ConnectionString;
        static SqlConnection conexion = new SqlConnection(connectionString);
        static string cadena;
        static SqlCommand comando;

        static void Main(string[] args)
        {
            int menuChoice = 0;
            do
            {
                Console.WriteLine("Hotel Boutique");
                Console.WriteLine("1. Registrar Cliente");
                Console.WriteLine("2. Editar Cliente");
                Console.WriteLine("3. Check In");
                Console.WriteLine("4. Check Out");
                Console.WriteLine("5. Salir");

                menuChoice = Convert.ToInt32(Console.ReadLine());
                Menu(menuChoice);

            } while (menuChoice != 5);

         Console.ReadLine();
        }
        public static void RegistrarCliente()
        {

            //Pedir e Insertar los datos del cliente en la tabla
            Console.WriteLine("Inserte el nombre del cliente:");
            string Nombre = Console.ReadLine();
            Console.WriteLine("Inserte los apellidos del cliente:");
            string Apellidos = Console.ReadLine();
            Console.WriteLine("Inserte el DNI del cliente:");
            string DNI = Console.ReadLine();

            
            conexion.Open();

            cadena = "INSERT INTO Cliente VALUES ( '" + Nombre + "','"+ Apellidos+"','" + DNI + "')";
            comando = new SqlCommand(cadena, conexion);
            comando.ExecuteNonQuery();

            conexion.Close();
            Console.WriteLine("El cliente ha sido registrado");
            return;
        }
        public static void EditarCliente()
        {

            Console.WriteLine("introduzca el DNI del cliente");
            string editDNI = Console.ReadLine();
            
            //Editamos cliente
            
            conexion.Open();
            cadena = "SELECT * FROM CLIENTE WHERE DNI LIKE '"+editDNI+"'";
            comando = new SqlCommand(cadena, conexion);
            SqlDataReader registros = comando.ExecuteReader();

            if (registros.Read())
            {
                Console.WriteLine("El registro existe");
                Console.WriteLine("Introduzca el nuevo nombre");
                string nuevoNombre = Console.ReadLine();
                Console.WriteLine("Introduzca los nuevos apellidos");
                string nuevoApellidos = Console.ReadLine();

                
                //Editamos los datos del cliente
                conexion.Close();
                conexion.Open();
                cadena = "UPDATE Cliente SET Nombre = '" + nuevoNombre + "' WHERE LIKE DNI '"+editDNI+"'";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                conexion.Close();
                conexion.Open();
                cadena = "UPDATE Cliente SET Apellidos = '" + nuevoApellidos + "' WHERE LIKE DNI '" + editDNI + "'";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                conexion.Close();

                Console.WriteLine("El cliente ha sido registrado de nuevo");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("El registro no existe");
            }
            conexion.Close();
            registros.Close();
         return;
                
        }
        public static void CheckIn()
        {
            
                Console.WriteLine("introduzca el DNI del cliente");
                string editDNI = Console.ReadLine();

                //Editamos cliente
        
                conexion.Open();
                cadena = "SELECT * FROM CLIENTE WHERE DNI LIKE '" + editDNI + "'";
                comando = new SqlCommand(cadena, conexion);
                SqlDataReader registros = comando.ExecuteReader();

                if (registros.Read())
                {
                    conexion.Close();
                    Console.WriteLine("Cliente esta registrado");

                    conexion.Open();
                    cadena = "SELECT * FROM Habitación WHERE Estado like 'libre'";
                    comando = new SqlCommand(cadena, conexion);
                    SqlDataReader habitacion = comando.ExecuteReader();

                    while (habitacion.Read())
                    {
                        Console.WriteLine(habitacion["CodHabitación"].ToString() + "\t" + habitacion["Estado"].ToString());
                        Console.WriteLine();
                    }

                    Console.ReadLine();
                    habitacion.Close();
                    conexion.Close();

                    //RESERVAR HABITACION

                    Console.WriteLine("Elija el numero de su habitación");

                    //Crear el codigo de Reserva

                    conexion.Open();
                    cadena = "SELECT max(CodReserva) FROM Reserva";
                    comando = new SqlCommand(cadena, conexion);
                    SqlDataReader codReservaR = comando.ExecuteReader();
                    int codReserva = Convert.ToInt32(codReservaR.Read())+1;
                    conexion.Close();

                    int codHabitación = Convert.ToInt32(Console.ReadLine());
                            
                    conexion.Open();
                    cadena = "UPDATE Habitación SET Estado = 'ocupado' WHERE CodHabitación LIKE'" + codHabitación + "'";
                    comando = new SqlCommand(cadena, conexion);
                    comando.ExecuteNonQuery();
                    conexion.Close();

                    conexion.Open();
                    cadena = "INSERT INTO Reserva (CodReserva, CodHabitación, DNI, Checkin) VALUES ('" + codReserva + "','" + codHabitación + "','" + editDNI + "','" + DateTime.Today.ToString("dd/MM/yyyy") + "')";
                    comando = new SqlCommand(cadena, conexion);
                    comando.ExecuteNonQuery();
                    conexion.Close();
                    Console.WriteLine("Su habitación ha sido reservada");
                    Console.ReadLine();
                    //exit = true;
                }
                else
                {
                Console.WriteLine("No se puede realizar la reserva, DNI inexistente, registre al cliente");
                }
            
            return;
        }
        public static void CheckOut()
        {
            Console.WriteLine("Introduzca el DNI del cliente");
            string DNI = Console.ReadLine();

            conexion.Open();
            cadena = "SELECT * FROM Reserva WHERE DNI LIKE '" + DNI + "' AND Checkout IS NULL";
            comando = new SqlCommand(cadena, conexion);
            SqlDataReader registros = comando.ExecuteReader();

            if (registros.Read())
            {
                conexion.Close();
                //Console.WriteLine("El Cliente esta registrado");

                conexion.Open();
                cadena = "UPDATE Reserva SET Checkout = GETDATE() WHERE DNI LIKE '"+DNI+"'";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                conexion.Close();

                //Seleccionar el Codhabitacion del que hacemos el checkout
                conexion.Open();
                cadena = "SELECT CodHabitación FROM Reserva WHERE DNI LIKE '" + DNI + "'";
                comando = new SqlCommand(cadena, conexion);
                SqlDataReader codHabitaciónR = comando.ExecuteReader();

                int codHabitación = Convert.ToInt32(codHabitaciónR.Read());
                
                conexion.Close();
                //int CodHabitación = Convert.ToInt32(Console.ReadLine());

                conexion.Open();
                cadena = "UPDATE Habitación SET Estado = 'libre' WHERE CodHabitación LIKE'" + codHabitación + "'";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                conexion.Close();


                Console.WriteLine("Se ha completado la salida del hotel");
            }
            else
            {
                Console.WriteLine("No se puede realizar el Check out, DNI inexistente, vuelva a introducir el DNI");
            }

            return;
        }
        public static void Menu(int Choice)
        {
            bool exit = false;
            do
            {
                switch (Choice)
                {
                    case 1:
                        RegistrarCliente();
                        exit = true;
                        break;
                    case 2:
                        EditarCliente();
                        exit = true;
                        break;
                    case 3:
                        CheckIn();
                        exit = true;
                        break;
                    case 4:
                        CheckOut();
                        exit = true;
                        break;
                    case 5:
                        Console.WriteLine("Que tenga usted un buen día");
                        exit = true;
                        break;
                }
            } while(exit == false);
            //return;
        }
    }
}
