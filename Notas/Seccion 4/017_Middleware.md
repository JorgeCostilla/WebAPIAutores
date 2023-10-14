# MIDDLEWARE

Hasta ahora hemos dicho que cuando nuestra web API reciba una peticion HTTP es una acción de un controlador la que recibe esta peticion y la procresa.

Esto en realida no es una descripcion precisa de lo que pasa cuando se recibe una peticion http.

Una peticion http llega a nuestro web API y pasa por lo que se conoce como una tuberia de peticiones HTTP.


### ¿Que es una tuberia de peticiones?
Es una cadena de procesos conectados de tal forma que la salida de cada elemento de la cadena es la entrada del proximo.

Entonces la tuberia de peticiones es el conjunto de procesos conectados los cuales se recibe una peticion http y la procesan para dar algún tipo de resultado.

Uno de esos procesos es el proceso de los endpoints que es donde se maneja hacia donde van las peticiones http al ser recibidas. Que en nuestro caso va a ser enviar dichas peticiones a los respectivos controladores segun la ruta utilizada por el cliente de la aplicación.

A cada uno de estos procesos de la tuberia le llamamos middleware

Otro middleware importante es el de autorizacion que es el que habilita la funcionalidad de denegar acceoso a un recurso dependiendo si el usuario tiene permiso de acceder.

Es normal que configuremos autorizacion en determinadops controles para asi configurar que solo usuarios autenticados en la Web API podran usar las acciones en el controlador.

Sin embargo es importante que para que la autorizacion funciones a nivel de los contralodores, primero hayamoos pasado por el middleware de autorizacion.

El orden de los middlewares es importante.

## Gráfica de los middleware de nuestra aplicacion

Request(peticion http) la recibe el primer middleware (quisa haga algo con esta peticion) despues lo pasa al siguiente middleware -> segundo middleware (quisa haga otra cosa) se lo pasa al tercero -> asi sucesibamente hasta el ultimo middleware que al final retorna al middleware anterior y este al anterior y este al anterior y asi sucesivamente hasta llegar al primer middleware original y este se encarga de responder al cliente de nuestra aplicacion.

## Donde se trabajan los middleware?
Esto lo trabajamos en la clase startup en el metodo Configure.