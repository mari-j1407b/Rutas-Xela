# Rutas Xela
Rutas Xela es una aplicación de consola que busca representar y administrar una pequeña red de transporte urbano. El sistema permitirá registrar paradas, establecer conexiones entre ellas y consultar las rutas disponibles para encontrar caminos entre diferentes puntos de la red.

Para desarrollar el proyecto se implementarán cuatro estructuras de datos: Tabla Hash, Grafo, Union-Find y Trie. Cada una tendrá una función específica dentro del sistema y trabajarán de manera integrada para gestionar la información y realizar las consultas.

## Objetivos

- Administrar las paradas y conexiones de una red de transporte urbano.
- Facilitar la búsqueda de paradas mediante códigos y nombres.
- Sugerir nombres de paradas a partir de las letras ingresadas.
- Encontrar rutas entre dos paradas utilizando sus tiempos de recorrido.
- Comprobar la conectividad entre diferentes paradas.
- Aplicar y analizar las estructuras de datos estudiadas durante el curso.

## Estructuras de datos

El proyecto utilizará las siguientes estructuras, implementadas por el equipo:

| Estructura | Aplicación dentro del sistema |
|---|---|
| Tabla Hash | Almacenar y localizar paradas mediante su código único. |
| Grafo | Representar las conexiones entre paradas y calcular rutas. |
| Union-Find | Comprobar si dos paradas pertenecen al mismo conjunto conectado. |
| Trie | Sugerir nombres de paradas mediante búsquedas por prefijo. |

En el grafo se implementarán los recorridos BFS y DFS, además del algoritmo de Dijkstra para encontrar la ruta de menor tiempo.

## Tecnologías

- Lenguaje: C#
- Entorno de desarrollo: Visual Studio / Visual Studio Code
- Control de versiones: Git y GitHub
- Almacenamiento inicial de datos: archivos CSV

## Organización del proyecto

El código se organizará en diferentes clases para mantener separada la lógica de cada estructura de datos y facilitar su integración.

## Equipo de trabajo

Proyecto desarrollado por tres integrantes:

- Integrante 1: Tabla Hash.
- Integrante 2: Grafo.
- Integrante 3: Trie y Union-Find.

## Estado del proyecto

En desarrollo.

---

**Universidad Rafael Landívar**  
Facultad de Ingeniería  
Ingeniería en Informática y Sistemas  
Curso: Estructura de Datos II
