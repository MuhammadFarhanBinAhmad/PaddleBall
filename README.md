# PaddleBall

**PaddleBall** is a 2D roguelite arcade project built in **Unity (C#)**.  
The game focuses on modular gameplay systems, event-driven abilities, enemy progression, and responsive combat feedback.

> **Status:** Work in progress

## Overview

In PaddleBall, the player builds strength through abilities and progression while facing enemies that scale in difficulty over time.  
The project was designed to explore gameplay systems that are flexible, data-driven, and easy to iterate on.

## Features

- **Modular ability system** using polymorphism for scalable and reusable gameplay logic
- **Event-driven design** to trigger abilities based on gameplay events
- **ScriptableObject-based data setup** for enemy stats, player abilities, and projectile values
- **Enemy director system** inspired by dynamic difficulty pacing, using point budgets and preset formations
- **Object pooling** for efficient projectile management and better runtime performance
- **Combat feedback systems** built to improve game feel and responsiveness

## Core Systems

### Ability System
Abilities are built around a base class with derived ability types, allowing different effects to be triggered cleanly through an ability manager.  
This makes it easier to add new abilities without rewriting the core structure.

### Enemy Spawning
Enemy spawning uses preset formations and a point-budget system.  
As the game progresses, the available budget increases and stronger enemy types become available, creating natural difficulty scaling.

### Data-Driven Design
ScriptableObjects are used to manage gameplay values such as enemy health, attack rate, projectile data, and ability stats.  
This keeps tuning separate from logic and makes balancing faster.

### Optimization
Projectiles are managed with object pooling to reduce unnecessary instantiation and improve performance during gameplay.

## Tech Stack

- Unity
- C#
- ScriptableObjects
- Object Pooling
- Event System / Observer Pattern
- Polymorphism

## Design Reference

- Figma: https://www.figma.com/board/9ix8CV4kZQZYZ8D7n5vbuZ/Breakout-Board?node-id=436-2445&t=j8lbxhoCAvkQSyzS-1

## Notes

PaddleBall is an ongoing personal project focused on gameplay systems, clean architecture, iteration, and responsive player experience.
