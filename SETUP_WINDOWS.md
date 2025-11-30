# Инструкция по настройке проекта на Windows

## Решение проблем при первом запуске

### Проблема 1: "Не найден компонент BCrypt.Net-Next"

Это происходит из-за того, что NuGet пакеты не были восстановлены.

**Решение (Шаг за шагом):**

1. **Закройте Visual Studio** (если открыт)

2. **Удалите старые пакеты и кэш:**
   - Удалите папку `packages` в корне решения (если существует)
   - Удалите папки `bin` и `obj` внутри `ShelterAppProduction`
   - Очистите NuGet кэш через командную строку:
     ```cmd
     nuget locals all -clear
     ```
     Или:
     ```cmd
     dotnet nuget locals all --clear
     ```

3. **Откройте Visual Studio** и откройте Solution

4. **Восстановите пакеты:**
   - Tools → NuGet Package Manager → Package Manager Console
   - Выполните команды:
     ```powershell
     Update-Package -reinstall
     ```

   Или через GUI:
   - Правой кнопкой на Solution → Restore NuGet Packages
   - Дождитесь завершения (смотрите Output окно)

5. **Пересоберите решение:**
   - Build → Clean Solution
   - Build → Rebuild Solution

6. **Если проблема не решилась:**
   - Откройте: Tools → NuGet Package Manager → Manage NuGet Packages for Solution
   - Перейдите на вкладку "Installed"
   - Найдите BCrypt.Net-Next
   - Нажмите "Uninstall"
   - Перейдите на вкладку "Browse"
   - Найдите "BCrypt.Net-Next"
   - Установите версию **4.0.2** (не 4.0.3!)
   - Нажмите Install и выберите проект ShelterAppProduction

---

### Проблема 2: "Не удалось найти тип или имя пространства имен BCrypt"

Эта ошибка появляется после восстановления пакетов.

**Решение:**

1. Убедитесь, что в файле `packages.config` есть строка:
   ```xml
   <package id="BCrypt.Net-Next" version="4.0.3" targetFramework="net472" />
   ```

2. Проверьте, что в начале файла `AuthService.cs` есть директива:
   ```csharp
   using BCrypt.Net;
   ```

3. Если проблема не исчезла:
   - Закройте Visual Studio
   - Удалите папку `packages` в корне решения
   - Удалите папки `bin` и `obj` в проекте ShelterAppProduction
   - Откройте Visual Studio снова
   - Восстановите пакеты (Restore NuGet Packages)
   - Пересоберите проект

---

### Проблема 3: SQL ошибки "Неверный синтаксис около ON" и "Неверный синтаксис около Username"

Это НЕ реальные ошибки! Visual Studio показывает их потому, что встроенный валидатор SQL ожидает синтаксис MS SQL Server, а мы используем PostgreSQL.

**Почему это не проблема:**
- SQL скрипты выполняются напрямую в PostgreSQL через psql или pgAdmin, а не через Visual Studio
- Синтаксис полностью корректен для PostgreSQL
- Эти "ошибки" не влияют на компиляцию и работу приложения

**Как убрать эти предупреждения (опционально):**

1. В Visual Studio откройте: Tools → Options
2. Перейдите в: Text Editor → File Extension
3. Добавьте расширение `pgsql` с редактором "Plain Text" вместо SQL редактора
4. Или просто игнорируйте эти сообщения - они не критичны

**SQL файлы уже настроены правильно:**
- Добавлены в .csproj как Content файлы
- Будут копироваться в выходную директорию при сборке
- Готовы к использованию с PostgreSQL

---

## Настройка базы данных PostgreSQL

### 1. Установка PostgreSQL

1. Скачайте PostgreSQL 12+ с официального сайта: https://www.postgresql.org/download/windows/
2. Установите PostgreSQL
3. Запомните пароль для пользователя `postgres`

### 2. Создание базы данных

Откройте **pgAdmin** или **psql** и выполните:

```sql
CREATE DATABASE shelter_db;
```

### 3. Выполнение SQL скриптов

**Вариант 1: Через psql (Command Line)**

```bash
cd path\to\TourApp\ShelterAppProduction\Database
psql -U postgres -d shelter_db -f create_tables.pgsql
psql -U postgres -d shelter_db -f insert_test_data.pgsql
```

**Вариант 2: Через pgAdmin**

1. Откройте pgAdmin
2. Подключитесь к серверу PostgreSQL
3. Откройте базу данных `shelter_db`
4. Нажмите Tools → Query Tool
5. Откройте файл `create_tables.pgsql` и выполните (F5)
6. Затем откройте `insert_test_data.pgsql` и выполните (F5)

### 4. Проверка строки подключения

Убедитесь, что в файле `DatabaseHelper.cs` правильная строка подключения:

```csharp
Host=localhost;Port=5432;Database=shelter_db;Username=postgres;Password=ваш_пароль
```

---

## Тестовые данные

После выполнения скриптов будут доступны тестовые пользователи:

- **Администратор**: `admin` / `admin`
- **Пользователь**: `user1` / `user1`

---

## Запуск приложения

1. Убедитесь, что PostgreSQL запущен
2. Нажмите F5 в Visual Studio или кнопку "Start"
3. Войдите используя тестовые данные

---

## Частые проблемы

### "Could not connect to server"

- Проверьте, что PostgreSQL запущен (Services → postgresql-x64-XX)
- Проверьте строку подключения в DatabaseHelper.cs
- Проверьте, что база данных `shelter_db` существует

### "Password authentication failed"

- Проверьте пароль в DatabaseHelper.cs
- Убедитесь, что используете правильного пользователя (`postgres`)

### Приложение не запускается

- Убедитесь, что установлен .NET Framework 4.7.2
- Проверьте, что все NuGet пакеты восстановлены
- Пересоберите решение (Build → Rebuild Solution)
