-- Scripts de clase - Marzo 24 de 2025 
-- Curso de Tópicos Avanzados de base de datos - UPB 202510
-- Juan Dario Rodas - juand.rodasm@upb.edu.co

-- Proyecto: ConsumosEnergéticos - Seguimiento de Servicios Públicos Domiciliarios
-- Motor de Base de datos: PostgreSQL 17.x

-- Con el usuario servicios_app

-- ****************************************
-- Creación de procedimientos
-- ****************************************


-- ### Servicios ####

-- p_inserta_servicio
create or replace procedure core.p_inserta_servicio(
                            in p_nombre             text,
                            in p_unidad_medida      text)
    language plpgsql as
$$
    declare
        l_total_registros integer;

    begin
        if p_nombre is null or
           p_unidad_medida is null or
           length(p_nombre) = 0 or
           length(p_unidad_medida) = 0 then
               raise exception 'El nombre del servicio o su unidad de medida no pueden ser nulos';
        end if;

        -- Validación de cantidad de registros con esa unidad de medida
        select count(id) into l_total_registros
        from core.servicios
        where lower(unidad_medida) = lower(p_unidad_medida)
        and lower(nombre) = lower(p_nombre);

        if l_total_registros != 0  then
            raise exception 'ya existe ese servicio con esa unidad de medida';
        end if;

        -- Validación de cantidad de servicios con ese nombre
        select count(id) into l_total_registros
        from core.servicios
        where lower(nombre) = lower(p_nombre);

        if l_total_registros > 0  then
            raise exception 'Ya existe un servicio registrado con ese nombre';
        end if;

        insert into core.servicios(nombre, unidad_medida)
        values (initcap(p_nombre), lower(p_unidad_medida));
    end;
$$;


-- p_actualiza_servicio
create or replace procedure core.p_actualiza_servicio(
                            in p_uuid               uuid,
                            in p_nombre             text,
                            in p_unidad_medida      text)
    language plpgsql as
$$
    declare
        l_total_registros integer;

    begin
        select count(id) into l_total_registros
        from core.servicios
        where uuid = p_uuid;

        if l_total_registros = 0  then
            raise exception 'No existe un servicio registrado con ese Guid';
        end if;

        if p_nombre is null or
           p_unidad_medida is null or
           length(p_nombre) = 0 or
           length(p_unidad_medida) = 0 then
               raise exception 'El nombre del servicio o la unidad de medida no pueden ser nulos';
        end if;

        -- Validación de cantidad de servicios con ese nombre
        select count(id) into l_total_registros
        from core.servicios
        where lower(nombre) = lower(p_nombre)
        and lower(unidad_medida) = lower(p_unidad_medida)
        and uuid != p_uuid;

        if l_total_registros > 0  then
            raise exception 'Ya existe un servicio registrado con ese nombre';
        end if;

        update core.servicios
        set nombre = initcap(p_nombre), unidad_medida = lower(p_unidad_medida)
        where uuid = p_uuid;
    end;
$$;

-- p_elimina_servicio
create or replace procedure core.p_elimina_servicio(
                            in p_uuid           uuid)
    language plpgsql as
$$
    declare
        l_total_registros integer;

    begin

        -- Cuantos servicios existen con el uuid proporcionado
        select count(id) into l_total_registros
        from core.servicios
        where uuid = p_uuid;

        if l_total_registros = 0  then
            raise exception 'No existe un servicio registrado con ese Guid';
        end if;

        -- Cuantos Componentes están asociados a este servicio
        select count(componente_id) into l_total_registros
        from core.v_info_componentes
        where servicio_uuid = p_uuid;

        if l_total_registros != 0  then
            raise exception 'No se puede eliminar, hay componentes que dependen de este servicio.';
        end if;

        -- Cuantos Consumos están asociados a este servicio
        select count(periodo_id) into l_total_registros
        from core.v_info_consumos
        where servicio_uuid = p_uuid;

        if l_total_registros != 0  then
            raise exception 'No se puede eliminar, hay consumos que dependen de este servicio.';
        end if;        

        delete from core.servicios
        where uuid = p_uuid;

    end;
$$;


-- ### Periodos ####

-- ### Componentes ####

-- ### Consumos ####