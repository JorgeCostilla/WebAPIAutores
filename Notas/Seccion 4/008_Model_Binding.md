# INTRODUCCION AL MODEL BINDING

El model binding nos permite mappear datos de una peticion http a los parametros de un endpoint o acción.

Ya hemos visto un ejemplo de Model Binding. 

Ejemplo :

En este endpoint mapeamos los parametros id y param2, en el pasado se les conocia como parametros de ruta.

Son los parametros que podemos recibir a la ruta del endPoint. 

```c#
[HttpGet("{id:int}/{param2=persona}")]
public async Task<ActionResult<Autor>> Get(int id, string param2)
{
    var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

    if (autor == null)
    {
        return NotFound();
    }
    return autor;
}
```

No estamos limitados a recibir los valores desde la ruta. 

Por ejemplo en el pasado vimo que podiamos pasar un Autor a travez del cuerpo de la peticion http.

```c#
[HttpPost]
public async Task<ActionResult> Post(Autor autor)
{
    context.Add(autor);
    await context.SaveChangesAsync();
    return Ok();
}
```

## Tambien se pueden colocar atributos a nivel de parametros

Ejemplo de un atributo de parametros.

Con [FromRoute] se indica que el valor del nombre va a venir desde la ruta
```c#
[HttpGet("{nombre}")]
public async Task<ActionResult<Autor>> Get([FromRoute]string nombre)
{
    var autor = await context.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));

    if (autor == null)
    {
        return NotFound();
    }
    return autor;
}
```

Ejemplo de FromBody que indica que el dato va a venir desde el cuerpo de la peticion.

```c#
[HttpPost]
public async Task<ActionResult> Post([FromBody] Autor autor)
{
    context.Add(autor);
    await context.SaveChangesAsync();
    return Ok();
}
```

## Listado de tipo de acciones
- [FromRoute] -> Desde la ruta.
- [FromBody] -> Desde el cuerpo de la peticion http.
- [FromHeader] -> Desde la cabecera. Recuerda que una peticion http pueden tener un cuerpo pero tambien una cabecera y son solo un conjunto de llaves y valores
- [FromQuery] -> va a obtener el valor desde el queryString. ¿Que es un querystring? Son unos valores que se colocan despues de la ruta que son un conjunto de llaves y valores.
- [FromServices] -> Que quiere decir que vienen desde servicios (Se explicaran mas adelante que son los servicios)
- [FromForm] -> Que es cuando la fuente vendra con el tipo de contenido o content-type application/x-www-url-formencoded (Esto lo vamos a utilizar cuando queramos recibir archivo a nuestra webApi desde nuestros clientes como imagenes word pdf etc) 


## Ejemplo de FromHeader y de FromQuery

El [FromQuery] se ve de la sigueinte manera en la ruta **api/autores/primero?nombre=felipe&apellido=gavilan** despues del signo de pregunta son las variables y cada variable se separa por un andpersand
```c#
[HttpGet("primero")] // api/autores/primero?nombre=felipe&apellido=gavilan
public async Task<ActionResult<Autor>> PrimerAutor([FromHeader] int miValor, [FromQuery] string nombre)
{
    return await context.Autores.FirstOrDefaultAsync();
}

```

Al probarlo en swagger vemos el siguiente codigo
- Los -H son los valores de los headers
- Y la ruta vemos las variables que retornar como FromQuery
```
curl -X 'GET' \
  'http://localhost:5006/api/autores/primero?nombre=Jorge' \
  -H 'accept: text/plain' \
  -H 'miValor: 89'
```