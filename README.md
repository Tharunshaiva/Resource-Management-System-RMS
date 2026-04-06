# Resource-Management-System-RMS

An end-to-end, production-ready Resource Management System (RMS) designed to centralize the planning, allocation, and monitoring of organizational assets. This system provides a "single source of truth" for managing human capital, hardware, and digital resources through a scalable, data-driven architecture.

## 📂 Project Structure

- **`FINAL PROJECT FRONTEND/`**: The client-side user interface (HTML, CSS, JS).
- **`Final v1/`**: The core .NET Backend Application (.NET core API).
- **`dtabase_sql.sql`**: The database schema and initialization script.
- **`Major project Microservice/`**: Standalone / optional supplementary microservices.

## ⚠️ Important Note regarding Microservices

The microservice folder (`Major project Microservice`) is treated as an independent project and interacts separately from the primary `.NET` core API:

- **If you do not need the microservices**: You can safely ignore or completely remove the microservices folder from your local environment. It does not impact the core functionality of the main frontend or `Final v1` backend API.
- **If you intend to use it**: You must open, build, and configure the microservices independently. It requires its own separate build process, separate instance configurations, and runs fully decoupled from the main workspace environment.
