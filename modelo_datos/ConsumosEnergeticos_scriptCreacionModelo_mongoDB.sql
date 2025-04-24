-- Scripts de clase - Abril 15 de 2025 
-- Curso de Tópicos Avanzados de base de datos - UPB 202510
-- Juan Dario Rodas - juand.rodasm@upb.edu.co

-- Proyecto: ConsumosEnergéticos - Seguimiento de Servicios Públicos Domiciliarios
-- Motor de Base de datos: MongoDB 7.x

-- ***********************************
-- Abastecimiento de imagen en Docker
-- ***********************************
 
-- Descargar la imagen
docker pull mongodb/mongodb-community-server

-- Crear el contenedor
docker run --name mongodb_energia -e “MONGO_INITDB_ROOT_USERNAME=mongoadmin” -e MONGO_INITDB_ROOT_PASSWORD=unaClav3 -p 27017:27017 -d mongodb/mongodb-community-server:latest

-- ****************************************
-- Creación de base de datos y usuarios
-- ****************************************

-- Para conectarse al contenedor
mongodb://mongoadmin:unaClav3@localhost:27017/

-- Con usuario mongoadmin:

-- Para saber que versión de Mongo se está usando
db.version()

-- crear la base de datos
use servicios_db;

-- Crear el rol para el usuario de gestion de Documentos en las colecciones
db.createRole(
  {
    role: "GestorDocumentos",
    privileges: [
        {
            resource: { 
                db: "servicios_db", 
                collection: "" 
            }, 
            actions: [
                "find", 
                "insert", 
                "update", 
                "remove",
                "listCollections"
            ]
        }
    ],
    roles: []
  }
);

-- Crear usuario para gestionar el modelo

db.createUser({
  user: "servicios_app",
  pwd: "unaClav3",  
  roles: [
    { role: "readWrite", db: "servicios_db" },
    { role: "dbAdmin", db: "servicios_db" }
  ],
    mechanisms: ["SCRAM-SHA-256"]
  }
);

db.createUser(
  {
    user: "servicios_usr",
    pwd: "unaClav3",
    roles: [ 
    { role: "GestorDocumentos", db: "servicios_db" }
    ],
    mechanisms: ["SCRAM-SHA-256"]
  }
);


-- Para saber que usuarios hay creados en la base de datos
db.getUsers()

-- Con el usuario servicios_app

-- ****************************************
-- Creación de Colecciones
-- ****************************************

-- Colección: Servicios
db.createCollection("servicios",{
        validator: {
            $jsonSchema: {
                bsonType: 'object',
                title: 'Los servicios a los que se les gestionarán consumos',
                required: [
                    "_id",
                    "nombre",
                    "unidad_medida"
                ],
                properties: {
                    _id: {
                        bsonType: 'objectId'
                    },
                    nombre: {
                        bsonType: 'string',
                        description: "'nombre' Debe ser una cadena de caracteres y no puede ser nulo",
                        minLength: 3
                    },
                    unidad_medida: {
                        bsonType: 'string',
                        description: "'unidad_medida' Debe ser una cadena de caracteres y no puede ser nulo",
                        minLength: 1
                    }
                },
                additionalProperties: false
            }
        }
    } 
);

-- Colección: Periodos
db.createCollection("periodos",{
        validator: {
            $jsonSchema: {
                bsonType: 'object',
                title: 'Los periodos a los que se les registrarán consumos de servicios',
                required: [
                    "_id",
                    "mes_facturacion",
                    "fecha_inicio",
                    "fecha_final",
                    "total_dias"
                ],
                properties: {
                    _id: {
                        bsonType: 'objectId'
                    },
                    mes_facturacion: {
                        bsonType: 'string',
                        description: 'Mes de facturación en formato MON-YYYY, ej (Octubre-2024)'
                    },
                    fecha_inicio: {
                        bsonType: 'string',
                        description: 'Fecha de inicio del período en formato DD/MM/YYYY'
                    },
                    fecha_final: {
                        bsonType: 'string',
                        description: 'Fecha de fin del período en formato DD/MM/YYYY'
                    },
                    total_dias: {
                        bsonType: 'int',
                        minimum: 1,
                        maximum: 32,
                        description: 'Total de días del período (entre 1 y 32)'
                    }
                },
                additionalProperties: false
            }
        }
    } 
);


-- Colección: Componentes
db.createCollection("componentes",{
        validator: {
            "$jsonSchema": {
                bsonType: 'object',
                title: 'Componentes de la tarifa de cada uno de los servicios',
                required: [
                    "_id",
                    "nombre",
                    "servicio",
                    "servicio_id"
                ],
                properties: {
                    _id: {
                        bsonType: 'objectId'
                    },
                    nombre: {
                        bsonType: "string",
                        description: 'Nombre del componente tarifario'
                    },
                    servicio: {
                        bsonType: "string",
                        description: 'Nombre del servicio que utiliza este componente tarifario'
                    },
                    servicio_id: {
                        bsonType: ["objectId","string"],
                        description: 'ID del servicio que utiliza este componente tarifario'
                    }
                },
                additionalProperties: false
            }
        }
    } 
);


-- Colección: Consumos
db.createCollection("consumos",{
        validator: {
            "$jsonSchema": {
                bsonType: "object",
                title: 'Registros los consumos por servicio por periodo',
                required: [
                    "_id",
                    "constante",
                    "lectura_actual",
                    "lectura_anterior",
                    "mes_facturacion",
                    "periodo_id",
                    "servicio",
                    "servicio_id"
                ],
                properties: {
                    _id: {
                        bsonType: "objectId"
                    },
                    constante: {
                        bsonType:  ["double","int"],
                        description: 'Factor de multiplicación utilizada para el servicio durante el periodo'                    
                    },
                    lectura_actual: {
                        bsonType: "int",
                        description: 'Lectura actual del medidor en el periodo '                    
                    },
                    lectura_anterior: {
                        bsonType: "int",
                        description: 'Lectura anterior del medidor en el periodo'                    
                    },
                    "mes_facturacion": {
                        bsonType: "string",
                        description: 'Mes para el cual se genera la factura del periodo'                    
                    },
                    "periodo_id": {
                        bsonType: ["objectId","string"],
                        description: 'Id del periodo en el cual se está registrando el consumo'                    
                    },
                    "servicio": {
                        bsonType: "string",
                        description: 'Nombre del servicio que utiliza este componente tarifario'                    
                    },
                    "servicio_id": {
                        bsonType: ["objectId","string"],
                        description: 'Id del servicio para el cual se está registrando el consumo en el periodo'                    
                    },
                },
                additionalProperties: false
            }
        }
    } 
);