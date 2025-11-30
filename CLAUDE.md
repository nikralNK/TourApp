# Инструкции для Claude Code

## Язык общения
- Отвечать на русском языке

## Работа с кодом
- Не оставлять комментариев в коде

## Git коммиты
- Не указывать себя в соавторстве в коммитах
- Не добавлять строки типа "Co-Authored-By: Claude <noreply@anthropic.com>"
- Не добавлять упоминания "Generated with Claude Code"

## Git Push
- При выполнении `git push` ВСЕГДА использовать SSH ключ `id_ed25519_shelter`
- Команда для push:
  ```bash
  GIT_SSH_COMMAND='ssh -i ~/.ssh/id_ed25519_shelter -o IdentitiesOnly=yes' git push origin main
  ```
- Для других веток заменить `main` на название нужной ветки

---

## О проекте

**ShelterApp** - десктопное приложение для управления приютом животных.

### Основная функциональность:
- Управление каталогом животных
- Система заявок на опекунство
- Профили пользователей и администраторов
- Управление избранными животными
- Административная панель для одобрения заявок

### Структура проекта:
```
ShelterAppProduction/
├── Database/           # SQL скрипты и DatabaseHelper
├── Models/             # Модели данных (Animal, User, Application, и т.д.)
├── Pages/              # Страницы WPF (Catalog, Admin, Profile, и т.д.)
├── Repositories/       # Репозитории для работы с БД
├── Services/           # Сервисы (Auth, Session)
├── Manager.cs          # Менеджер навигации
├── LoginWindow.xaml    # Окно авторизации
└── MainWindow.xaml     # Главное окно с Frame
```

---

## Технический стек

### Frontend:
- **WPF (Windows Presentation Foundation)** с XAML
- **.NET Framework 4.7.2**
- Навигация через Frame (паттерн WorldSkills)

### Backend:
- **C# .NET Framework 4.7.2**
- Архитектура: Repository pattern
- Статический менеджер для навигации между страницами

### База данных:
- **PostgreSQL 12+**
- Библиотека подключения: **Npgsql 4.1.14** (ADO.NET)
- Параметризованные SQL запросы

### Безопасность:
- **BCrypt.Net-Next 4.0.2** для хеширования паролей
- Защита от SQL-инъекций через параметризованные запросы
- Проверка прав доступа

### Дополнительные библиотеки:
- System.Text.Json 4.7.2
- System.Buffers 4.5.1
- System.Memory 4.5.5
- Microsoft.Bcl.AsyncInterfaces 1.1.1

### Дизайн:
- Минималистичный стиль
- Цветовая схема: зеленый (#4A7C59) и бежевый (#F5F5DC)
- Скругленные углы и тени для UI элементов

---

## Стандарты разработки

Проект следует стандартам **WorldSkills** для десктопных приложений:
- Page-based архитектура с Frame навигацией
- Разделение на слои: Models, Repositories, Services, Pages
- Централизованное управление навигацией через Manager
- Глобальные стили в App.xaml
