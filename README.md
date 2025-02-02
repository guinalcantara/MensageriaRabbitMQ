# MensageriaRabbitMQ

Comando docker
docker run -d --hostname my-rabbit --name some-rabbit -p 5672:5672 -p 15672:15672 rabbitmq:3-management

CREATE DATABASE LogDb;

USE LogDb;

CREATE TABLE Logs (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Message VARCHAR(255),
    Data DATETIME
);
