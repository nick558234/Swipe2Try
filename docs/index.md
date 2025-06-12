# Welcome to Swipe2Try Documentation

Welcome to the comprehensive documentation for **Swipe2Try**, a modern web application built with ASP.NET Core following clean architecture principles.

## 🎯 Overview

Swipe2Try is a restaurant discovery and food ordering application that allows users to browse restaurants, view dishes, and place orders. The application implements a sophisticated three-layer architecture with robust authentication and authorization systems.

## 📚 Documentation Structure

### 🏗️ Architecture
- **[Architecture Overview](Architecture.md)** - Complete guide to the three-layer architecture implementation
- **[Diagrams](diagrams/index.md)** - Visual representations of system architecture and relationships

### 📊 Core Components
- **[Domain Models](DomainModels.md)** - Comprehensive documentation of core business entities

### 🔐 Security
- **[Authentication](Authentication.md)** - User authentication system using ASP.NET Core Identity
- **[Authorization](Authorization.md)** - Role-based access control and security policies

### 💻 Implementation Patterns
- **[Tuple Return Types](TupleReturnTypePattern.md)** - Error handling pattern used throughout the application

## 🚀 Quick Start

If you're new to the project, we recommend following this learning path:

1. **Start with [Architecture Overview](Architecture.md)** to understand the overall system design
2. **Review [Authentication](Authentication.md)** to understand user management
3. **Explore [Authorization](Authorization.md)** for role-based access control
4. **Study [Implementation Patterns](TupleReturnTypePattern.md)** for coding standards

## 🎨 Visual Guides

The documentation includes comprehensive diagrams to help visualize the system:

- **[Three-Layer Architecture](diagrams/three_layer_architecture.mmd)** - System architecture overview
- **[Authentication Flow](diagrams/auth_class_diagram.mmd)** - User authentication process
- **[User Relationships](diagrams/user_role_relationships.mmd)** - Role and permission structure
- **[Dish Management](diagrams/dish_class_diagram.mmd)** - Food item management system

## 🛠️ Technology Stack

- **Frontend**: ASP.NET Core Razor Pages
- **Backend**: ASP.NET Core with Clean Architecture
- **Authentication**: ASP.NET Core Identity with Cookie Authentication
- **Database**: SQL Server (via ADO.NET repositories)
- **Documentation**: MkDocs with Material theme

## 📖 Additional Resources

- **Project Setup**: See the root README.md in the project repository for setup and development instructions
- **Source Code**: Explore the well-documented codebase with clear separation of concerns

---

*This documentation is built with MkDocs and the Material theme. Navigate using the sidebar or use the search functionality to find specific topics.*
