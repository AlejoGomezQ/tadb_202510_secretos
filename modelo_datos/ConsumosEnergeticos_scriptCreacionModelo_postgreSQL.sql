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

-- Tabla: Estratos
create table core.estratos
(
    id                  int not null constraint estratos_pk primary key,
    descripcion         varchar(50) not null
);

comment on table core.estratos is 'Estratos socioeconómicos';
comment on column core.estratos.id is 'Id del estrato';
comment on column core.estratos.descripcion is 'descripción del estrato';

-- Tabla: Departamentos
create table core.departamentos
(
    id                  int not null constraint departamentos_pk primary key,
    nombre              varchar(100) not null constraint departamentos_descripcion_uk unique,
    dane_id             varchar(2) default '00'
);

comment on table core.departamentos is 'Departamentos del país';
comment on column core.departamentos.id is 'id del departamento';
comment on column core.departamentos.nombre is 'nombre del departamento';
comment on column core.departamentos.dane_id is 'codigo DANE asociado al departamento';

-- Tabla: Municipios
create table core.municipios
(
    id                  int not null constraint municipios_pk primary key,
    nombre              varchar(100) not null,
    departamento_id     int not null constraint municipio_departamento_fk references core.departamentos,
    dane_id             varchar(10) not null constraint municipios_dane_uk unique,
    constraint municipio_departamento_uk unique (nombre, departamento_id)
);

comment on table core.municipios is 'Municipios del pais';
comment on column core.municipios.id is 'id del municipio';
comment on column core.municipios.nombre is 'nombre del municipio';
comment on column core.municipios.departamento_id is 'id del departamento asociado al municipio';
comment on column core.municipios.dane_id is 'codigo DANE asociado al municipio';

-- Tabla: Servicios
create table core.servicios
(
    id                  int generated always as identity constraint servicios_pk primary key,
    nombre              varchar(50) not null,
    unidad_medida       varchar(10) not null
);

alter table core.servicios add column uuid uuid default gen_random_uuid();

comment on table core.servicios is 'Descripcion de los servicios a monitorear';
comment on column core.servicios.id is 'Id del servicio';
comment on column core.servicios.nombre is 'nombre del servicio';
comment on column core.servicios.unidad_medida is 'Unidad de medida del servicio';

-- Tabla: Periodos
create table core.periodos
(
    id                  int not null constraint periodos_pk primary key,
    fecha_inicio        date not null,
    fecha_final         date not null,
    total_dias          int not null,
    mes_facturacion     varchar(20) not null
);

alter table core.periodos add column uuid uuid default gen_random_uuid();

comment on table core.periodos is 'Periodos de registro de consumo del servicio';
comment on column core.periodos.id is 'Id del periodo';
comment on column core.periodos.fecha_inicio is 'Fecha de Inicio del periodo';
comment on column core.periodos.fecha_final is 'Fecha de finalización del periodo';
comment on column core.periodos.total_dias is 'Cantidad de dias incluidos en el periodo';
comment on column core.periodos.mes_facturacion is 'Mes para el cual se genera la factura del periodo';

-- Tabla: consumos
create table core.consumos
(
    periodo_id       integer not null constraint consumos_periodos_fk references periodos,
    servicio_id      integer not null constraint consumos_servicios_fk references servicios,
    lectura_actual   integer not null,
    lectura_anterior integer not null,
    constante        float   not null,
    constraint consumos_pk primary key (servicio_id, periodo_id)
);

comment on table core.consumos is 'Registros los consumos por servicio por periodo';
comment on column core.consumos.periodo_id is 'Id del periodo para el cual se registra el consumo';
comment on column core.consumos.servicio_id is 'Id del servicio para el cual se está registrando el consumo en el periodo';
comment on column core.consumos.lectura_actual is 'Lectura del medidor en el periodo actual';
comment on column core.consumos.lectura_anterior is 'Lectura del medidor en el periodo anterior';
comment on column core.consumos.constante is 'Factor de multiplicación utilizada para el servicio durante el periodo';

-- Tabla de Componentes
create table core.componentes
(
    id          integer         not null constraint componentes_pk primary key,
    nombre      varchar(100) not null,
    servicio_id integer      not null constraint componentes_servicios_fk references servicios
);

alter table core.componentes add column uuid uuid default gen_random_uuid();

comment on table core.componentes is 'Componentes de la tarifa de cada uno de los servicios';
comment on column core.componentes.id is 'Id del Componente tarifario';
comment on column core.componentes.nombre is 'Nombre del componente tarifario';
comment on column core.componentes.servicio_id is 'ID del servicio que utiliza este componente tarifario';

-- Tabla: costos_componentes_periodos
create table core.costos_componentes_periodos
(
    periodo_id    integer         not null constraint costos_componentes_periodos_componentes_periodos_periodo_fk references periodos,
    componente_id integer         not null constraint costos_componentes_periodos_componentes_periodos_componente_fk references componentes,
    costo        float not null,
    constraint costos_componentes_periodos_pk primary key (componente_id, periodo_id)
);

comment on table core.costos_componentes_periodos is 'Tarifas para los componentes del cargo del servicio por periodo';
comment on column core.costos_componentes_periodos.periodo_id is 'ID del periodo para el cual se registra el valor del componente del servicio';
comment on column core.costos_componentes_periodos.componente_id is 'ID del componente tarifario para el cual se registra el valor';
comment on column core.costos_componentes_periodos.costo is 'el valor del componente del servicio para el periodo indicado';



-- Vista: v_info_componentes
create or replace view core.v_info_componentes as
(
select distinct
    c.servicio_id,
    s.nombre servicio,
    c.id componente_id,
    c.nombre componente,
    c.uuid componente_uuid,
    s.uuid servicio_uuid
from core.servicios s
    join core.componentes c on s.id = c.servicio_id
);

-- Vista: v_info_costos_componentes
create view core.v_info_costos_componentes as
(
select
    p.id periodo_id,
    p.mes_facturacion,
    s.id servicio_id,
    s.nombre servicio,
    c.id componente_id,
    c.nombre componente,
    ccp.costo
from
    core.costos_componentes_periodos ccp
    join core.periodos p on ccp.periodo_id = p.id
    join core.componentes c on ccp.componente_id = c.id
    join core.servicios s on c.servicio_id = s.id
);

-- Vista: v_info_territorios
create view core.v_info_territorios as
(
select distinct
    m.departamento_id,
    d.dane_id departamento_dane,
    d.nombre departamento,
    m.id municipio_id,
    m.dane_id municipio_dane,
    m.nombre municipio
from core.departamentos d
    join core.municipios m on d.id = m.departamento_id
);

-- Vista: v_info_consumos
create or replace view core.v_info_consumos as (
select distinct
    c.periodo_id,
    p.mes_facturacion,
    c.servicio_id,
    s.uuid servicio_uuid,
    s.nombre servicio,
    c.lectura_actual,
    c.lectura_anterior,
    c.constante,
    ((c.lectura_actual-lectura_anterior)*c.constante) consumo
from core.consumos c
    join core.periodos p on c.periodo_id = p.id
    join core.servicios s on c.servicio_id = s.id
);

