CREATE DATABASE movelabu_escritorio CHARACTER SET utf8mb4 COLLATE utf8mb4_spanish_ci;
USE movelabu_escritorio;

CREATE TABLE usuarios (
  id INT AUTO_INCREMENT PRIMARY KEY,
  usuario VARCHAR(50) NOT NULL UNIQUE,
  contrasena CHAR(64) NOT NULL,          -- la contraseña esta encriptada con SHA-256
  rol VARCHAR(20) NOT NULL DEFAULT 'Administrador'
);

CREATE TABLE entrenadores (
  id INT AUTO_INCREMENT PRIMARY KEY,
  nombre VARCHAR(60) NOT NULL,
  apellido VARCHAR(60) NOT NULL,
  dni VARCHAR(10) NOT NULL UNIQUE,
  telefono VARCHAR(20),
  fecha_ingreso DATE NOT NULL,
  cargo VARCHAR(40) NOT NULL,
  estado VARCHAR(10) NOT NULL DEFAULT 'Activo'
);

CREATE TABLE clientes (
  id INT AUTO_INCREMENT PRIMARY KEY,
  nombre VARCHAR(60) NOT NULL,
  apellido VARCHAR(60) NOT NULL,
  dni VARCHAR(10) NOT NULL UNIQUE,
  telefono VARCHAR(20),
  fecha_ingreso DATE NOT NULL,
  estado VARCHAR(10) NOT NULL DEFAULT 'Activo',
  tipo_plan VARCHAR(30) NOT NULL,
  entrenador_id INT NULL,
  FOREIGN KEY (entrenador_id) REFERENCES entrenadores(id) ON DELETE SET NULL
);

CREATE TABLE cuotas (
  id INT AUTO_INCREMENT PRIMARY KEY,
  cliente_id INT NOT NULL,
  tipo_plan VARCHAR(30) NOT NULL,
  monto DECIMAL(10,2) NOT NULL,
  fecha_inicio DATE NOT NULL,
  fecha_vencimiento DATE NOT NULL,
  estado VARCHAR(10) NOT NULL DEFAULT 'Pendiente',
  FOREIGN KEY (cliente_id) REFERENCES clientes(id) ON DELETE CASCADE
);

CREATE TABLE pagos (
  id INT AUTO_INCREMENT PRIMARY KEY,
  cliente_id INT NOT NULL,
  cuota_id INT NOT NULL,
  usuario_id INT NOT NULL,
  fecha DATE NOT NULL,
  monto DECIMAL(10,2) NOT NULL,
  medio_pago VARCHAR(20) NOT NULL,
  observaciones VARCHAR(255),
  FOREIGN KEY (cliente_id) REFERENCES clientes(id) ON DELETE CASCADE,
  FOREIGN KEY (cuota_id)   REFERENCES cuotas(id)   ON DELETE CASCADE,
  FOREIGN KEY (usuario_id) REFERENCES usuarios(id)
);

-- Usuario para entrar: admin / admin123 (se guardo encriptada)
INSERT INTO usuarios (usuario, contrasena, rol)
VALUES ('admin', SHA2('admin123', 256), 'Administrador');