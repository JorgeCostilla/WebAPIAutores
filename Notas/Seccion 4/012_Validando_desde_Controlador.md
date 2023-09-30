# VALIDANDO DESDE EL CONTROLADOR - VALIDANDO CONTRA LA BD


## Otro tipo de reglas que vas a ver mucho es a nivel del controlador

Ejemplo yo no quiero poder agregar dos autores con el mismo nombre.

Para esto dentro del metodo donde quiero hacer la validacion se validara contra la BD

```c#
[HttpPost]
public async Task<ActionResult> Post([FromBody] Autor autor)
{
            
    var existeAutorConElMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autor.Nombre);

    if (existeAutorConElMismoNombre)
    {
        return BadRequest($"Ya existe un autor con el nombre {autor.Nombre}");
    }

    context.Add(autor);
    await context.SaveChangesAsync();
    return Ok();
}
```

Al hacer la prueba cumple con la regla de que si ya existe este nombre no lo permita ingresar

``` 
Ya existe un autor con el nombre Claudia
```


### Con esto concluimos los temas de validacion

- Primera fue validaciones por atributos
- Segunda fue validaciones personalizada y agregarlas por atributo
- Tercer fue validaciones por modelo.
- Cuarta validaciones al nivel del controlador.
