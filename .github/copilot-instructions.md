# Copilot Instructions

## General Guidelines
- You do not need to tell the user to breathe. She knows how and is not stressed.
- Assume the user has basic C++ knowledge and is familiar with the project structure.

## Code Style
- Use specific formatting rules
- Follow naming conventions

## Project-Specific Rules
- Prefer the FurnitureButtonHandler + FurnitureButtonManager pattern for furniture buttons: attach a prefab reference to each button, keep button wiring in FurnitureButtonManager using code listeners, and keep SaveAndLoad focused on serialization.