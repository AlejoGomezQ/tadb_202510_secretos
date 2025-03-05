-- Scripts de clase - Marzo 5 de 2025 
-- Curso de Tópicos Avanzados de base de datos - UPB 202510
-- Juan Dario Rodas - juand.rodasm@upb.edu.co

-- Proyecto: ConsumosEnergéticos - Seguimiento de Servicios Públicos Domiciliarios
-- Motor de Base de datos: PostgreSQL 17.x

-- ***********************************
-- Abastecimiento de imagen en Docker
-- ***********************************
 
-- Descargar la imagen
docker pull postgres:latest

-- Crear el contenedor
docker run --name postgres-energia -e POSTGRES_PASSWORD=unaClav3 -d -p 5432:5432 postgres:latest

-- ****************************************
-- Creación de base de datos y usuarios
-- ****************************************

-- Con usuario Postgres:

-- crear el esquema la base de datos
create database energia_db;

-- Conectarse a la base de datos
\c energia_db;

-- Creamos un esquema para almacenar todo el modelo de datos del dominio
create schema core;

-- crear el usuario con el que se implementará la creación del modelo
create user energia_app with encrypted password 'unaClav3';

-- asignación de privilegios para el usuario
grant connect on database energia_db to energia_app;
grant create on database energia_db to energia_app;
grant create, usage on schema core to energia_app;
alter user energia_app set search_path to core;

-- crear el usuario con el que se conectará la aplicación
create user energia_usr with encrypted password 'unaClav3';

-- asignación de privilegios para el usuario
grant connect on database energia_db to energia_usr;
grant usage on schema core to energia_usr;
alter default privileges for user energia_usr in schema core grant insert, update, delete, select on tables to energia_usr;
alter default privileges for user energia_usr in schema core grant execute on routines TO energia_usr;
alter user energia_usr set search_path to core;

-- Activar la extensión que permite el uso de UUID
create extension if not exists "uuid-ossp";