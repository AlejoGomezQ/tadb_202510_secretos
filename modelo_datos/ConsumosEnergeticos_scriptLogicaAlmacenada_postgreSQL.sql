-- Scripts de clase - Marzo 24 de 2025 
-- Curso de Tópicos Avanzados de base de datos - UPB 202510
-- Juan Dario Rodas - juand.rodasm@upb.edu.co

-- Proyecto: ConsumosEnergéticos - Seguimiento de Servicios Públicos Domiciliarios
-- Motor de Base de datos: PostgreSQL 17.x

-- Con el usuario servicios_app


-- ****************************************
-- Creación de funciones
-- ****************************************

create or replace function core.f_obtiene_intervalo_fechas(
                                                        p_fecha_inicio date,
                                                        p_fecha_final date)
    returns int as
$$
    declare
        l_total_dias int :=0;

    begin
        select
            date_part('month',age(p_fecha_final, p_fecha_inicio)) * 30 +
            date_part('day',age(p_fecha_final,p_fecha_inicio))
        into l_total_dias;

        return l_total_dias;
    end;
$$ language plpgsql;

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

-- p_inserta_periodo
create or replace procedure core.p_inserta_periodo(
                            in p_fecha_inicial      text,
                            in p_fecha_final        text,
                            in p_total_dias         int,
                            in p_mes_facturacion    text)
    language plpgsql as
$$
    declare
        l_total_registros   integer;
        l_total_dias        integer;
    begin
        if p_fecha_inicial is null or
           p_fecha_final is null or
           p_mes_facturacion is null or
           length(p_fecha_inicial) = 0 or
           length(p_fecha_final) = 0 or
           length(p_mes_facturacion) = 0 then
               raise exception 'El mes de facturación o las fechas del periodo no pueden ser datos nulos';
        end if;

        -- Validación de cantidad de registros con esa fecha inicial y final
        select count(id) into l_total_registros
        from core.periodos
        where fecha_inicio = to_date(p_fecha_inicial,'DD/MM/YYYY')
        and fecha_final = to_date(p_fecha_final,'DD/MM/YYYY');

        if l_total_registros != 0  then
            raise exception 'ya existe ese periodo asociado a esas fechas de inicio y final';
        end if;

        -- Validación de unicidad de la especificación del mes de facturación
        select count(id) into l_total_registros
        from core.periodos
        where lower(mes_facturacion) = lower(p_mes_facturacion);

        if l_total_registros != 0  then
            raise exception 'ya existe ese periodo asociado a ese mes de facturación';
        end if;

        -- Validacion de que las fechas formen un intervalo
        if to_date(p_fecha_inicial,'DD/MM/YYYY') > to_date(p_fecha_final,'DD/MM/YYYY') then
            raise exception 'Las fechas indicadas no definen un intervalo';
        end if;

        -- Verificacion duracion en dias
        l_total_dias := to_date(p_fecha_final,'DD/MM/YYYY') -
                         to_date(p_fecha_inicial,'DD/MM/YYYY');

        if l_total_dias = p_total_dias then
            l_total_dias := p_total_dias;
        end if;

    insert into core.periodos(
        fecha_inicio,
        fecha_final,
        total_dias,
        mes_facturacion)
    values (
        to_date(p_fecha_inicial,'DD/MM/YYYY'),
        to_date(p_fecha_final,'DD/MM/YYYY'),
        l_total_dias,
        initcap(p_mes_facturacion));
    end;
$$;

-- p_actualiza_periodo
create or replace procedure core.p_actualiza_periodo(
                            in p_uuid               uuid,
                            in p_fecha_inicial      text,
                            in p_fecha_final        text,
                            in p_total_dias         int,
                            in p_mes_facturacion    text)
    language plpgsql as
$$
    declare
        l_total_registros   integer;
        l_total_dias        integer;
    begin
        select count(id) into l_total_registros
        from core.periodos
        where uuid = p_uuid;

        if l_total_registros = 0  then
            raise exception 'No existe un periodo registrado con ese Guid';
        end if;

        if p_fecha_inicial is null or
           p_fecha_final is null or
           p_mes_facturacion is null or
           length(p_fecha_inicial) = 0 or
           length(p_fecha_final) = 0 or
           length(p_mes_facturacion) = 0 then
               raise exception 'El mes de facturación o las fechas del periodo no pueden ser datos nulos';
        end if;

        -- Validación de cantidad de registros con esa fecha inicial y final
        select count(id) into l_total_registros
        from core.periodos
        where fecha_inicio = to_date(p_fecha_inicial,'DD/MM/YYYY')
        and fecha_final = to_date(p_fecha_final,'DD/MM/YYYY');

        if l_total_registros != 0  then
            raise exception 'ya existe ese periodo asociado a esas fechas de inicio y final';
        end if;

        -- Validación de unicidad de la especificación del mes de facturación
        select count(id) into l_total_registros
        from core.periodos
        where lower(mes_facturacion) = lower(p_mes_facturacion)
        and uuid != p_uuid;

        if l_total_registros != 0  then
            raise exception 'ya existe ese periodo asociado a ese mes de facturación';
        end if;

        -- Validacion de que las fechas formen un intervalo
        if to_date(p_fecha_inicial,'DD/MM/YYYY') > to_date(p_fecha_final,'DD/MM/YYYY') then
            raise exception 'Las fechas indicadas no definen un intervalo';
        end if;

        -- Verificacion duracion en dias
        l_total_dias := to_date(p_fecha_final,'DD/MM/YYYY') -
                         to_date(p_fecha_inicial,'DD/MM/YYYY');

        if l_total_dias = p_total_dias then
            l_total_dias := p_total_dias;
        end if;

    update core.periodos set
        fecha_inicio = to_date(p_fecha_inicial,'DD/MM/YYYY'),
        fecha_final = to_date(p_fecha_final,'DD/MM/YYYY'),
        total_dias = l_total_dias,
        mes_facturacion = initcap(p_mes_facturacion)
    where uuid = p_uuid;

    end;
$$;

create or replace procedure core.p_elimina_periodo(
                            in p_uuid           uuid)
    language plpgsql as
$$
    declare
        l_total_registros integer;

    begin
        select count(id) into l_total_registros
        from core.periodos
        where uuid = p_uuid;

        if l_total_registros = 0  then
            raise exception 'No existe un periodo registrado con ese Guid';
        end if;

        -- Cuantos costos asociados al componente están registrados
        select count(periodo_id) into l_total_registros
        from core.v_info_costos_componentes
        where periodo_uuid = p_uuid;

        if l_total_registros != 0  then
            raise exception 'No se puede eliminar, hay costos asociados a este periodo.';
        end if;

        -- Cuantos consumos están asociados al periodo
        select count(periodo_id) into l_total_registros
        from core.v_info_consumos
        where periodo_uuid = p_uuid;

        if l_total_registros != 0  then
            raise exception 'No se puede eliminar, hay consumos asociados a este periodo.';
        end if;

        delete from core.periodos
        where uuid = p_uuid;
    end;
$$;

-- ### Componentes ####

-- p_inserta_componente
create or replace procedure core.p_inserta_componente(
                            in p_nombre         text,
                            in p_servicio       text)
    language plpgsql as
$$
    declare
        l_total_registros   integer;
        l_servicio_id       integer :=0;

    begin
        if p_nombre is null or
           p_servicio is null or
           length(p_nombre) = 0 or
           length(p_servicio) = 0 then
               raise exception 'El nombre del componente o su servicio no pueden ser nulos';
        end if;

        -- Validación de cantidad de registros con ese nombre y servicio
        select count(componente_id) into l_total_registros
        from core.v_info_componentes
        where lower(componente) = lower(p_nombre)
        and lower(servicio) = lower(p_servicio);

        if l_total_registros != 0  then
            raise exception 'ya existe ese componente asociado a ese servicio';
        end if; 

        -- Validación de servicio existente
        select count(id) into l_total_registros
        from core.servicios 
        where lower(nombre) = lower(p_servicio);    

        if l_total_registros = 0  then
            raise exception 'No existe servicio con ese nombre';
        end if; 

        -- Obtenemos el id del servicio
        select id into l_servicio_id
        from core.servicios
        where lower(nombre) = lower(p_servicio);    

        insert into core.componentes (nombre, servicio_id)
        values (initcap(p_nombre),l_servicio_id);
    end;
$$;

-- p_actualiza_componente
create or replace procedure core.p_actualiza_componente(
                            in p_uuid               uuid,
                            in p_nombre             text,
                            in p_servicio           text)
    language plpgsql as
$$
    declare
        l_total_registros   integer;
        l_servicio_id       integer :=0;

    begin
        select count(id) into l_total_registros
        from core.componentes
        where uuid = p_uuid;

        if l_total_registros = 0  then
            raise exception 'No existe un componente registrado con ese Guid';
        end if;

        if p_nombre is null or
           p_servicio is null or
           length(p_nombre) = 0 or
           length(p_servicio) = 0 then
               raise exception 'El nombre del componente o su servicio no pueden ser nulos';
        end if;

        -- Validación de cantidad de registros con ese nombre y servicio
        select count(componente_id) into l_total_registros
        from core.v_info_componentes
        where lower(componente) = lower(p_nombre)
        and lower(servicio) = lower(p_servicio);

        if l_total_registros != 0  then
            raise exception 'ya existe ese componente asociado a ese servicio';
        end if;
        
        -- Validación de servicio existente
        select count(id) into l_total_registros
        from core.servicios 
        where lower(nombre) = lower(p_servicio);    

        if l_total_registros = 0  then
            raise exception 'No existe servicio con ese nombre';
        end if; 

        -- Obtenemos el id del servicio
        select id into l_servicio_id
        from core.servicios
        where lower(nombre) = lower(p_servicio);          


        update core.componentes
        set nombre = initcap(p_nombre), servicio_id = l_servicio_id
        where uuid = p_uuid;
    end;
$$;

-- p_elimina_componente
create or replace procedure core.p_elimina_componente(
                            in p_uuid           uuid)
    language plpgsql as
$$
    declare
        l_total_registros integer;

    begin
        select count(id) into l_total_registros
        from core.componentes
        where uuid = p_uuid;

        if l_total_registros = 0  then
            raise exception 'No existe un componente registrado con ese Guid';
        end if;

        -- Cuantos costos asociados al componente están registrados
        select count(componente_id) into l_total_registros
        from core.v_info_costos_componentes
        where componente_uuid = p_uuid;

        if l_total_registros != 0  then
            raise exception 'No se puede eliminar, hay costos asociados a este componente para varios períodos.';
        end if;

        delete from core.componentes
        where uuid = p_uuid;

    end;
$$;

-- ### Consumos ####