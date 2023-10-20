using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Servicios
{
    public class EscribirEnArchivo : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string nombreArchivo = "Archivo 1.txt";
        private Timer timer;

        public EscribirEnArchivo (IWebHostEnvironment env)
        {
            this.env = env;
        }
        
        // Cuando se inciliza escribe en el archivo proceso iniciado
        // Ademas return de que la tarea finalzo
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Iniciamos el timer en 0 y se ejecutara cada 5 segundos
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            Escribir("Proceso iniciado");
            return Task.CompletedTask;
        }

        // Cuando se inciliza escribe en el archivo proceso finalizado
        // Ademas return de que la tarea finalzo
        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose(); // Detenemos el timer
            Escribir("Proceso finalizado");
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            Escribir("Proceso en ejecucion: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
        }

        // Metodo aux en el cual escribe en un archivo de texto el mensaje
        private void Escribir(string mensaje)
        {
            var ruta = $@"{env.ContentRootPath}\wwwroot\{nombreArchivo}";
            using (StreamWriter writer = new StreamWriter(ruta, append: true))
            {
                writer.WriteLine(mensaje);
            }
        }
    }
}