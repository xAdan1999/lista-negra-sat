using System.Threading.Tasks;
using System.Configuration;
using System.Text;
using System.Net;
using System.IO;
using System;

namespace ConsoleAppListaNegraSAT
{
    internal class Program
    {
        #region proyect description

        /*
        ---------------------------------------------------------------------------------------
        Author      : Adan Cruz Campa
        Date        : 22/06/2023
        Last update : 11/12/2023
        Purpose     : Permite descargar el Articulo 69-B (lista negra del SAT)
        ---------------------------------------------------------------------------------------- 
        */

        #endregion

        async static Task Main(string[] args)
        {
            //Verificar conexion a internet
            if (CheckInternetConnection())
            {
                //Inicia proceso
                Console.WriteLine("Descargando archivo...");

                //Establecer la url del recurso
                string url = ConfigurationManager.AppSettings.Get("url");

                //Establecer la ruta donde se guardara el archivo
                string downloadPath = ConfigurationManager.AppSettings.Get("path");

                //Obtener el nombre del archivo con la extension
                string fileName = Path.GetFileName(url);

                //Asignar a la ruta de la descarga el nombre del archivo
                string path = Path.Combine(downloadPath, fileName);

                //Ejecutar la descarga con los parametros establecidos
                await DownloadFile(new Uri(url), path);
            }
            else
            {
                //Si no se encontro conexion a internet registrar el error
                WriteToFile("Error de red", "No se pudo descargar el archivo debido a que no se encontró conexión a internet", DateTime.Now);
            }
        }

        //Funcion que permite descargar un archivo de una url
        private static async Task DownloadFile(Uri uri, string filePath)
        {
            try
            {
                //Objeto para poder descargar
                using (WebClient webClient = new WebClient())
                {
                    await webClient.DownloadFileTaskAsync(uri, filePath);
                }
            }
            catch (Exception ex)
            {
                //Si ocurre algun error de cualquier otro tipo registrarlo
                WriteToFile("Error al descargar el archivo", ex.ToString(), DateTime.Now);
            }
        }

        //Funcion que permite comprobar si se tiene internet para poder descargar el archivo
        private static bool CheckInternetConnection()
        {
            try
            {
                //Se intenta abrir una conexion utilizando WebClient.OpenRead()
                //Si no se produce ninguna excepcion, significa que hay conexion a Internet
                using (var stream = new WebClient().OpenRead(address: "http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                //Si se produce una excepcion, significa que no hay conexion a internet
                return false;
            }
        }

        //Funcion para guardar logs, servira para poder detectar errores
        private static void WriteToFile(string message, string error, DateTime date)
        {
            //Para obtener la carpeta donde se guardaran los logs
            string logsPath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";

            //Si la carpeta de logs no existe
            if (!Directory.Exists(logsPath))
            {
                //Crearla para poder guardarlos
                Directory.CreateDirectory(logsPath);
            }

            //Obtener el archivo de logs
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";

            //Si no existe
            if (!File.Exists(filePath))
            {
                //Crearlo para poder escribir en el
                using (StreamWriter sw = File.CreateText(path: filePath))
                {
                    //Escribir el mensaje
                    sw.WriteLine(value: "Message: " + message);
                    sw.WriteLine(value: "Error:   " + error);
                    sw.WriteLine(value: "Date:    " + date);
                    sw.WriteLine(WriteSeparationLine());
                }
            }
            else
            {
                //Si ya existe simplemente volver a escribir sobre el
                using (StreamWriter sw = File.AppendText(path: filePath))
                {
                    //Escribir el mensaje
                    sw.WriteLine(value: "Message: " + message);
                    sw.WriteLine(value: "Error:   " + error);
                    sw.WriteLine(value: "Date:    " + date);
                    sw.WriteLine(WriteSeparationLine());
                }
            }
        }

        //Funcion que escribe una linea de separacion en cada error registrado
        private static string WriteSeparationLine()
        {
            var separation = new StringBuilder();
            for (int i = 0; i < 100; i++)
                separation.Append(value: "-");
            return separation.ToString();
        }
    }
}
