# Actualizando y Borrando Recursos


## Update
Para actualizar un registro vamos a usar [HttpPut]. Algo que es muy comun para tu actualizar un recurso especifico tu debes colocar en la url el id del recurso.
Eso se colocar en el[HttpPut] entonces quedaria asi -> [HttpPut("algo")] y para que se un parametro de ruta seria [HttpPut("{id:int}")] con esto indicamos que id es una variable y es de tipo entero.

Para poder tomar ese valor en el metodo se necesita pasar como parametro al metodo y tiene que coincidir el nombre.

```c#
[HttpPut("{id:int}")]
public async Task<ActionResult> Put(Autor autor, int id)
{
            
}
```

Agregamos la logica de modificacion.
```c#
[HttpPut("{id:int}")]
public async Task<ActionResult> Put(Autor autor, int id)
{
    // Validamos que sea el id del autor correcto
    if (autor.Id != id)
    {
        return BadRequest("El id del autor no coincide con el id de la URL");
    }

    // Validamos que sea el id del autor correcto
    var existe = await context.Autores.AnyAsync(x => x.Id == id);

    if (!existe)
    {
        return NotFound();
    }

    // Actualizamos y guardamos
    context.Update(autor);
    await context.SaveChangesAsync();
    return Ok();
}
```

Si te preguntas porque tenemos que hacer esto de la validacion de la url con el autor. ¿Existe una manera de evitarlo? R= Si pero es lo que nos comento el maestro se usan DTOs pero es algo que vamos a ver en un futuro.

## Delete

Para borrar creamos un metodo Delete de autores con un parametro de id para saber que usuario vamos a eliminar. Lo primero que hay que saber es si existe este usuario. Con AnyAsync comprobamos si existe en la tabla de la base de datos.

La parte de new Autor en el siguiente codigo solo estamos instanciando la entidad Autor.

```c#
 [HttpDelete("{id:int}")]
public async Task<ActionResult> Delete(int id)
{
    // Validamos que sea el id del autor correcto
    var existe = await context.Autores.AnyAsync(x => x.Id == id);

    if (!existe)
    {
        return NotFound();
    }

    context.Remove(new Autor() {Id = id});
    await context.SaveChangesAsync();
    return Ok();
}
```
