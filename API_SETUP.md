# Инструкция по настройке и запуску ShelterApp с API

## Обзор изменений

WPF приложение теперь работает через REST API вместо прямого подключения к PostgreSQL.

### Что изменено:

1. **ApiService** - новый сервис для HTTP запросов к API
2. **ApiModels** - модели данных для API (DTO)
3. **Все Repositories** - адаптированы для работы с API
4. **AuthService** - переработан для JWT аутентификации

## Настройка API на Linux

### 1. Установка и запуск API

```bash
cd /var/www/yebitch/ShelterAPI

# Создать виртуальное окружение
python3 -m venv venv
source venv/bin/activate

# Установить зависимости
pip install -r requirements.txt

# Применить миграции
alembic upgrade head

# Запустить API
uvicorn app.main:app --host 0.0.0.0 --port 8000
```

### 2. Проверка API

API будет доступно по адресу: http://localhost:8000

Документация Swagger: http://localhost:8000/docs

### 3. Проброс порта на Windows (если нужно)

Если API работает на Linux машине, а WPF приложение на Windows, используйте SSH туннель:

```bash
# На Windows в PowerShell или CMD
ssh -L 8000:localhost:8000 user@linux-server-ip
```

Или используйте WSL2 и настройте порт forwarding.

## Настройка WPF приложения на Windows

### 1. Требования

- .NET Framework 4.7.2+
- Visual Studio 2019+ или MSBuild
- Доступ к API на http://localhost:8000

### 2. Подключение к API

API URL по умолчанию: `http://localhost:8000/api/v1`

Если нужно изменить адрес API, отредактируйте файл:
`ShelterAppProduction/Services/ApiService.cs`

```csharp
private const string BASE_URL = "http://localhost:8000/api/v1";
```

### 3. Сборка проекта

```powershell
cd TourApp
msbuild ShelterAppProduction.sln /p:Configuration=Release
```

Или откройте в Visual Studio и соберите проект.

### 4. Запуск приложения

```powershell
cd ShelterAppProduction\bin\Release
.\ShelterAppProduction.exe
```

## Важные изменения в архитектуре

### Аутентификация

- Вместо прямой проверки паролей используется JWT токен
- Токен сохраняется в ApiService и автоматически добавляется ко всем запросам
- При выходе токен удаляется

### Асинхронность

Все методы репозиториев теперь асинхронные (`async/await`):

```csharp
// Было:
List<Animal> animals = animalRepo.GetAll();

// Стало:
List<Animal> animals = await animalRepo.GetAll();
```

### Guardian Repository

GuardianRepository пока работает напрямую с БД, так как в API нет отдельного эндпоинта для guardians. Это нормально для текущей реализации.

## Тестирование

### 1. Проверка подключения к API

- Запустите API
- Запустите WPF приложение
- Попробуйте войти с существующим пользователем

### 2. Регистрация нового пользователя

Создайте нового пользователя через окно регистрации

### 3. Основной функционал

- Просмотр каталога животных
- Добавление в избранное
- Создание заявок на опекунство
- Административные функции (для админов)

## Устранение проблем

### API недоступен

- Убедитесь, что API запущен: `curl http://localhost:8000/health`
- Проверьте firewall правила
- Проверьте SSH туннель (если используется)

### Ошибки аутентификации

- Убедитесь, что пользователь существует в БД
- Проверьте правильность логина/пароля
- Посмотрите логи API

### Ошибки сети в WPF

- Проверьте URL в ApiService.cs
- Убедитесь, что API доступен с Windows машины
- Проверьте антивирус/файрволл на Windows

## Дополнительная информация

### Структура проекта

```
TourApp/
├── ShelterAppProduction/
│   ├── Services/
│   │   ├── ApiService.cs          # HTTP клиент
│   │   ├── AuthService.cs         # Аутентификация через API
│   │   └── SessionManager.cs      # Управление сессией
│   ├── Models/
│   │   └── ApiModels.cs           # DTO модели
│   ├── Repositories/
│   │   ├── AnimalRepository.cs    # Работа с API
│   │   ├── ApplicationRepository.cs
│   │   └── FavoriteRepository.cs
│   └── ...
```

### API Endpoints

- `POST /api/v1/auth/login` - Вход
- `POST /api/v1/auth/register` - Регистрация
- `POST /api/v1/auth/logout` - Выход
- `GET /api/v1/users/me` - Текущий пользователь
- `GET /api/v1/animals/` - Список животных
- `GET /api/v1/favorites/` - Избранные животные
- `GET /api/v1/applications/` - Заявки на опекунство

Полная документация: http://localhost:8000/docs
