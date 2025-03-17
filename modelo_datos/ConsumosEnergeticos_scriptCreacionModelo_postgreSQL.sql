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
create database servicios_db;

-- Conectarse a la base de datos
\c servicios_db;

-- Creamos un esquema para almacenar todo el modelo de datos del dominio
create schema core;

-- crear el usuario con el que se implementará la creación del modelo
create user servicios_app with encrypted password 'unaClav3';

-- asignación de privilegios para el usuario
grant connect on database servicios_db to servicios_app;
grant create on database servicios_db to servicios_app;
grant create, usage on schema core to servicios_app;
alter user servicios_app set search_path to core;

-- crear el usuario con el que se conectará la aplicación
create user servicios_usr with encrypted password 'unaClav3';

-- asignación de privilegios para el usuario
grant connect on database servicios_db to servicios_usr;
grant usage on schema core to servicios_usr;

-- Privilegios sobre tablas existentes
grant select, insert, update, delete, trigger on all tables in schema core to servicios_usr;

-- privilegios sobre secuencias existentes
grant usage, select on all sequences in schema core to servicios_usr;

-- privilegios sobre funciones existentes
grant execute on all functions in schema core to servicios_usr;

-- privilegios sobre procedimientos existentes
grant execute on all procedures in schema core to servicios_usr;

alter default privileges for user servicios_usr in schema core grant insert, update, delete, select on tables to servicios_usr;
alter default privileges for user servicios_usr in schema core grant execute on routines TO servicios_usr;
alter user servicios_usr set search_path to core;





-- Activar la extensión que permite el uso de UUID
create extension if not exists "uuid-ossp";


-- Con el usuario servicios_app

-- ****************************************
-- Creación de base de datos y usuarios
-- ****************************************

-- Tabla: Servicios
create table servicios
(
    id            integer     not null constraint servicios_pk primary key,
    nombre        varchar(50) not null,
    unidad_medida varchar(10) not null
);

comment on table servicios is 'Descripcion de los servicios a monitorear';
comment on column servicios.id is 'Id del servicio';
comment on column servicios.nombre is 'nombre del servicio';
comment on column servicios.unidad_medida is 'Unidad de medida del servicio';

-- Tabla: Periodos
create table periodos
(
    id           integer not null,
    fecha_inicio date    not null,
    fecha_final  date    not null,
    total_dias   integer not null,
    mes_facturacion  varchar(20) not null
);

alter table periodos add constraint periodos_pk primary key (id);

comment on table periodos is 'Periodos de registro de consumo del servicio';
comment on column periodos.id is 'Id del periodo';
comment on column periodos.fecha_inicio is 'Fecha de Inicio del periodo';
comment on column periodos.fecha_final is 'Fecha de finalización del periodo';
comment on column periodos.total_dias is 'Cantidad de dias incluidos en el periodo';
comment on column periodos.mes_facturacion is 'Mes para el cual se genera la factura del periodo';

-- Tabla: consumos
create table consumos
(
    periodo_id       integer not null constraint consumos_periodos_fk references periodos,
    lectura_actual   integer not null,
    lectura_anterior integer not null,
    constante        float   not null,
    servicio_id      integer not null constraint consumos_servicios_fk references servicios,
    constraint consumos_pk primary key (servicio_id, periodo_id)
);

comment on table consumos is 'Registros los consumos por servicio por periodo';
comment on column consumos.periodo_id is 'Id del periodo para el cual se registra el consumo';
comment on column consumos.lectura_actual is 'Lectura del medidor en el periodo actual';
comment on column consumos.lectura_anterior is 'Lectura del medidor en el periodo anterior';
comment on column consumos.constante is 'Factor de multiplicación utilizada para el servicio durante el periodo';
comment on column consumos.servicio_id is 'Id del servicio para el cual se está registrando el consumo en el periodo';

-- Tabla de Componentes
create table componentes
(
    id          integer generated always as identity constraint componentes_pk primary key,
    nombre      varchar(100) not null,
    servicio_id integer      not null constraint componentes_servicios_fk references servicios
);

comment on table componentes is 'Componentes de la tarifa de cada uno de los servicios';
comment on column componentes.id is 'Id del Componente tarifario';
comment on column componentes.nombre is 'Nombre del componente tarifario';
comment on column componentes.servicio_id is 'ID del servicio que utiliza este componente tarifario';

-- Tabla: tarifas_componentes_periodos
create table tarifas_componentes_periodos
(
    periodo_id    integer         not null constraint tarifas_componentes_periodos_periodo_fk references periodos,
    componente_id integer         not null constraint tarifas_componentes_periodos_componente_fk references componentes,
    tarifa        float default 0 not null,
    constraint tarifas_componentes_periodos_pk primary key (componente_id, periodo_id)
);

comment on table tarifas_componentes_periodos is 'Tarifas para los componentes del cargo del servicio por periodo';
comment on column tarifas_componentes_periodos.periodo_id is 'ID del periodo para el cual se registra el valor del componente del servicio';
comment on column tarifas_componentes_periodos.componente_id is 'ID del componente tarifario para el cual se registra el valor';
comment on column tarifas_componentes_periodos.tarifa is 'el valor del componente del servicio para el periodo indicado';



-- Vista: v_info_componentes
create or replace view core.v_info_componentes as
(
select distinct
    s.id servicio_id,
    s.nombre servicio,
    c.id componente_id,
    c.nombre componente
from core.servicios s
    join core.componentes c on s.id = c.servicio_id
);

-- Vista: v_info_tarifas_componentes
create view core.v_info_tarifas_componentes as
(
select
    p.id periodo_id,
    p.mes_facturacion,
    s.id servicio_id,
    s.nombre servicio,
    c.id componente_id,
    c.nombre componente,
    tcp.tarifa
from
    tarifas_componentes_periodos tcp
    join periodos p on tcp.periodo_id = p.id
    join componentes c on tcp.componente_id = c.id
    join servicios s on c.servicio_id = s.id
);
