# eVOL - Advanced Chat Messaging App

A modern **backend chat messaging application** built with .NET 10,

---

## ⚡ Features

- Real-time messaging with **SignalR**
- **JWT authentication** and password hashing (BCrypt)
- **AES encryption** for sensitive data
- **Caching with Redis**
- **User profiles and more in MySQL**
- **Chat messages in MongoDB**
- **Unit Testing using xUnit**
- **RabbitMQ support for messaging queue**
- **Scalable API layer** with multiple instances behind **NGINX load balancer**
- **Containerized services** managed with **Docker Compose**

---

## 🛠 Technologies

| Layer            | Technology                                               |
|------------------|----------------------------------------------------------|
| Backend           | .NET 10, ASP.NET Core Web API, Clean Architecture        |
| Database          | MySQL (users), MongoDB (chat messages)                  |
| Caching           | Redis                                                    |
| Security          | JWT Tokens, AES Encryption, BCrypt Hashing              |
| Real-time         | SignalR                                                  |
| Message Brokler   | RabbitMQ                                |
| Load Balancing    | NGINX (reverse proxy for multiple API instances)        |
| Logging           | Serilog - Seq                                            |
| Unit Testing      | xUnit                                                    |
| Containerization  | Docker, Docker Compose                                   |
| CI/CD             | GitHub Actions Pipeline                                  |

---

## 🏗 Project Architecture
 
- **Backend:** Api server using **NGINX** for load balancing  
- **Databases:** MySQL for users, MongoDB for chat messages, Redis for caching  
- **Real Time Communication:** SignalR & RabbitMQ for latest and modern messaging
- **Reverse Proxy:** NGINX manages incoming requests and distributes them across multiple API instances  
- **Docker Compose:** Orchestrates all services, networking, and volumes for persistent storage  

---

