namespace WebAPIAutores.Servicios
{
    public interface IServicio
    {
        Guid ObtenerScoped();
        Guid ObtenerSingleton();
        Guid ObtenerTransient();
        void RealizarTarea();
    }

    public class ServicioA : IServicio
    {
        private readonly ILogger<ServicioA> logger;
        private readonly ServicioTrasient servicioTrasient;
        private readonly ServicioScoped servicioScoped;
        private readonly ServicioSingleton servicioSingleton;

        public ServicioA(ILogger<ServicioA> logger, ServicioTrasient servicioTrasient,
            ServicioScoped servicioScoped, ServicioSingleton servicioSingleton)
        {
            this.logger = logger;
            this.servicioTrasient = servicioTrasient;
            this.servicioScoped = servicioScoped;
            this.servicioSingleton = servicioSingleton;
        }

        public Guid ObtenerTransient() { return servicioTrasient.Guid; }
        public Guid ObtenerScoped() { return servicioScoped.Guid; }
        public Guid ObtenerSingleton() { return servicioSingleton.Guid; }

        public void RealizarTarea()
        {
        }
    }

    public class ServicioB : IServicio
    {
        public Guid ObtenerScoped()
        {
            throw new NotImplementedException();
        }

        public Guid ObtenerSingleton()
        {
            throw new NotImplementedException();
        }

        public Guid ObtenerTransient()
        {
            throw new NotImplementedException();
        }

        public void RealizarTarea()
        {
        }
    }

    public class ServicioTrasient 
    {
        // Esto lo que esta haciendo es creando un string aleatorio
        public Guid Guid = Guid.NewGuid();
    }
    public class ServicioScoped
    {
        // Esto lo que esta haciendo es creando un string aleatorio
        public Guid Guid = Guid.NewGuid();
    }
    public class ServicioSingleton 
    {
        // Esto lo que esta haciendo es creando un string aleatorio
        public Guid Guid = Guid.NewGuid();
    }
}