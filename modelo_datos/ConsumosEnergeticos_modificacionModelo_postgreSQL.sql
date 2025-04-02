-- Scripts de clase - Abril 2 de 2025 
-- Curso de Tópicos Avanzados de base de datos - UPB 202510
-- Juan Dario Rodas - juand.rodasm@upb.edu.co

-- Proyecto: ConsumosEnergéticos - Seguimiento de Servicios Públicos Domiciliarios
-- Motor de Base de datos: PostgreSQL 17.x

-- ***********************************************
-- Modificaciones al modelo por fallos en diseño
-- ***********************************************

-- ###############################
-- Tabla Componentes
-- ###############################

-- Borrar las vistas que dependen de la tabla de componentes
drop view v_info_componentes;
drop view v_info_costos_componentes;

-- Borrar claves foráneas que dependan de la tabla componentes
alter table costos_componentes_periodos drop constraint costos_componentes_periodos_componentes_periodos_componente_fk;

-- crear nueva columna de autonumérico para el id
alter table componentes add nuevo_id integer generated always as identity;

-- comparar que ambas columnas quedaron igual
select distinct id, nuevo_id from componentes
order by id;

-- quitar la clave primaria de la tabla
alter table componentes drop constraint componentes_pk;

-- eliminar la columna id
alter table componentes drop column id;

-- renombrar la columna nuevo_id a id
alter table componentes rename column nuevo_id to id;

-- activar la clave primaria
alter table componentes add constraint componentes_pk primary key (id);

-- Recrear clave foráneas que dependan de la tabla componentes
alter table costos_componentes_periodos add constraint
costos_componentes_periodos_componentes_periodos_componente_fk
    foreign key (componente_id) references componentes;

-- Recrear las vistas que dependen de la tabla componentes
create or replace view core.v_info_componentes as
(
select distinct
    c.servicio_id,
    s.uuid servicio_uuid,
    s.nombre servicio,
    c.id componente_id,
    c.uuid componente_uuid,
    c.nombre componente
from core.servicios s
    join core.componentes c on s.id = c.servicio_id
);

create view core.v_info_costos_componentes as
(
select
    p.id periodo_id,
    p.uuid periodo_uuid,
    p.mes_facturacion,
    c.servicio_id,
    s.uuid servicio_uuid,
    s.nombre servicio,
    ccp.componente_id,
    c.uuid componente_uuid,
    c.nombre componente,
    ccp.costo
from
    core.costos_componentes_periodos ccp
    join core.periodos p on ccp.periodo_id = p.id
    join core.componentes c on ccp.componente_id = c.id
    join core.servicios s on c.servicio_id = s.id
);